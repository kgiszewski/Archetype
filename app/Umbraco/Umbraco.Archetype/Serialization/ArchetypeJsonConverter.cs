using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Archetype.Umbraco.Serialization
{
    public class ArchetypeJsonConverter<T> : JsonConverter
        where T : class, new()
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var fieldsets = (value as IList) ?? new List<T> { value as T };

            if (fieldsets.Count == 1 && fieldsets[0] == null) 
                return;

            var jObj = new JObject
            {
                {
                    "fieldsets", 
                     new JArray( new JRaw(SerializeFieldsets(fieldsets)))
                }
            };

            writer.WriteRaw(ApplyFormatting(jObj.ToString(), writer.Formatting));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jToken = JToken.ReadFrom(reader);

            if (jToken == null)
                return null;

            var obj = Activator.CreateInstance(objectType);

            if (null != obj as IEnumerable<object> 
                && jToken["fieldsets"] != null && jToken["fieldsets"].Any())
            {
                var model = obj as IEnumerable<object>;
                var fieldsets = jToken["fieldsets"];

                var itemType = model.GetType().BaseType.GetGenericArguments().First();
                foreach (var fs in fieldsets.Where(fs => fs["alias"].ToString().Equals(GetFieldsetName(itemType))))
                {
                    var item = JsonConvert.DeserializeObject(
                        fs["properties"].ToString(), itemType, GetArchetypeDatatypeConverter(itemType));

                    obj.GetType().GetMethod("Add").Invoke(obj, new [] {item});
                }

                return obj;
            }

            if (null == jToken as JArray)
            {
                jToken = jToken.SelectToken("fieldsets[0].properties");
            }

            var properties = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var propertyInfo in properties)
            {
                var propAlias = GetJsonPropertyName(propertyInfo);
                var propJToken = jToken.Single(p => p.SelectToken("alias").ToString().Equals(propAlias));
                var propValue = IsValueArchetypeDatatype(propertyInfo.PropertyType)
                    ? JsonConvert.DeserializeObject(propJToken["value"].ToString(), propertyInfo.PropertyType, GetArchetypeDatatypeConverter(propertyInfo.PropertyType))
                    : propJToken["value"].ToObject(propertyInfo.PropertyType);

                propertyInfo.SetValue(obj, propValue);
            }

            return obj;
        }

        public override bool CanConvert(Type objectType)
        {
            return IsValueArchetypeDatatype(objectType);
        }

        #region private methods

        private string ApplyFormatting(string json, Formatting formatting)
        {
            return JToken.Parse(json).ToString(formatting);
        }

        private IEnumerable SerializeFieldsets(IEnumerable fieldSets)
        {
            var fieldsetJson = (from object fs in fieldSets where null != fs select SerializeObject(fs)).ToList();

            return String.Join(",", fieldsetJson);
        }

        private string SerializeObject(object value)
        {
            if (value == null)
                return null;

            var jObj = GetFieldsetJObject(value);

            var fieldsetJson = new StringBuilder();
            var fieldsetWriter = new StringWriter(fieldsetJson);

            using (var jsonWriter = new JsonTextWriter(fieldsetWriter))
            {
                jObj.WriteTo(jsonWriter);
            }

            return fieldsetJson.ToString();
        }

        private JObject GetFieldsetJObject(object obj)
        {
            var jObj = new JObject
                {
                    {
                        "alias",
                        new JValue(GetFieldsetName(obj.GetType()))
                    }
                };

            var properties = obj.GetType()
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var fsProperties = new List<JObject>();

            foreach (var propertyInfo in properties)
            {
                var fsProperty = new JObject();
                var jProperty = new JProperty("alias", GetJsonPropertyName(propertyInfo));
                fsProperty.Add(jProperty);

                var propValue = propertyInfo.GetValue(obj, null);

                fsProperty.Add(
                    new JProperty("value",
                                  IsValueArchetypeDatatype(propValue)
                                      ? new JRaw(JsonConvert.SerializeObject(propValue,
                                                                             GetArchetypeDatatypeConverter(propValue)))
                                      : new JValue(GetPropertyValue(propValue))));

                fsProperties.Add(fsProperty);
            }

            jObj.Add("properties", new JRaw(JsonConvert.SerializeObject(fsProperties)));

            return jObj;
        }

        private string GetJsonPropertyName(PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(true);
            var jsonPropAttribute = (JsonPropertyAttribute)attributes.FirstOrDefault(attr => attr is JsonPropertyAttribute);

            return jsonPropAttribute != null ? jsonPropAttribute.PropertyName : property.Name;
        }

        private string GetFieldsetName(Type type)
        {
            var attributes = type.GetCustomAttributes(true);
            var archetypeDatatypeAttribute = (ArchetypeDatatypeAttribute)attributes.FirstOrDefault(attr => attr is ArchetypeDatatypeAttribute);

            return archetypeDatatypeAttribute != null ? archetypeDatatypeAttribute.FieldsetName : type.Name;
        }

        private bool IsValueArchetypeDatatype(object value)
        {
            return value != null &&
                IsValueArchetypeDatatype(value.GetType());
        }

        private bool IsValueArchetypeDatatype(Type type)
        {
            return type.GetCustomAttributes(typeof(ArchetypeDatatypeAttribute), true).Length > 0;
        }

        private JsonConverter GetArchetypeDatatypeConverter(object value)
        {
            return GetArchetypeDatatypeConverter(value.GetType());
        }

        private JsonConverter GetArchetypeDatatypeConverter(Type type)
        {
            var converterType = typeof(ArchetypeJsonConverter<>);
            Type[] typeArgs = { type };
            var genericType = converterType.MakeGenericType(typeArgs);

            return (JsonConverter)Activator.CreateInstance(genericType);
        }

        private string GetPropertyValue(object propValue)
        {
            if (propValue == null)
                return String.Empty;

            if (propValue is bool)
                return (bool)propValue ? GetPropertyValue(1) : GetPropertyValue(0);

            return String.Format("{0}", propValue);
        }

        #endregion
    }
}