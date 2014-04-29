using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Archetype.Umbraco.Serialization
{
    public class ArchetypeJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var models = GenerateModels(value);

            if (models.Count < 1)
                return;

            if (models.Count == 1 && models[0] == null) 
                return;

            var jObj = new JObject
            {
                {
                    "fieldsets", 
                     new JArray( new JRaw(SerializeModels(models)))
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
                return DeserializeEnumerableObject(obj, jToken);
            }

            return null == jToken as JArray 
                        ? DeserializeObject(obj, jToken) 
                        : PopulateProperties(obj, jToken);
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

        private IList GenerateModels(object value)
        {
            var models = value as IList;

            if (null != models)
                return models;

            var properties = GetSerialiazableProperties(value).ToList();

            if (!PropertyLayoutHasFieldsets(properties))
                return new List<object>() { value };

            var dynamicModel = new ExpandoObject() as IDictionary<string, object>;

            foreach (var pInfo in properties.Where(pInfo => !HasAsFieldsetAttribute(pInfo)))
            {
                dynamicModel.Add(GetJsonPropertyName(pInfo), pInfo.GetValue(value));
            }

            var fieldsetModels = properties.Where(HasAsFieldsetAttribute)
                .Select(pInfo => pInfo.GetValue(value));

            models = new List<object>
            {
                dynamicModel,                
            };

            models = ((IEnumerable<object>)models).Concat(fieldsetModels).ToList();

            return models;
        }

        private object DeserializeEnumerableObject(object obj, JToken jToken)
        {
            var model = obj as IEnumerable<object>;

            var itemType = model.GetType().BaseType.GetGenericArguments().First();
            foreach (var fs in jToken["fieldsets"].Where(fs => fs["alias"].ToString().Equals(GetFieldsetName(itemType))))
            {
                var item = JsonConvert.DeserializeObject(
                    fs["properties"].ToString(), itemType, GetArchetypeDatatypeConverter());

                obj.GetType().GetMethod("Add").Invoke(obj, new[] { item });
            }

            return obj;
        }

        private object DeserializeObject(object obj, JToken jToken)
        {
            var properties = GetSerialiazableProperties(obj).ToList();
            var asFieldset = properties.Where(HasAsFieldsetAttribute).ToList();

            foreach (var propInfo in asFieldset)
            {
                var propAlias = GetJsonPropertyName(propInfo);
                var fsJToken = GetFieldsetProperties(propInfo.PropertyType, jToken);
                var propJToken =
                    fsJToken.Single(p => p.SelectToken("alias").ToString().Equals(propAlias));

                if (propJToken == null)
                    continue;

                var propValue = IsValueArchetypeDatatype(propInfo.PropertyType)
                ? JsonConvert.DeserializeObject(fsJToken.ToString(), propInfo.PropertyType,
                    GetArchetypeDatatypeConverter())
                : GetDeserializedPropertyValue(fsJToken, propInfo.PropertyType); ;

                propInfo.SetValue(obj, propValue);
            }

            return PopulateProperties(obj, GetFieldsetProperties(obj.GetType(), jToken));
        }

        private JToken GetFieldsetProperties(Type objType, JToken jToken)
        {
            var objAlias = GetFieldsetName(objType);
            return
                jToken["fieldsets"].Single(p => p.SelectToken("alias").ToString().Equals(objAlias)).SelectToken("properties");
        }

        private object PopulateProperties(object obj, JToken jToken)
        {
            var properties = GetSerialiazableProperties(obj);

            foreach (var propertyInfo in properties)
            {
                var propAlias = GetJsonPropertyName(propertyInfo);

                var propJToken = jToken.SingleOrDefault(p => p.SelectToken("alias").ToString().Equals(propAlias));

                if (propJToken == null)
                    continue;

                var propValue = GetPropertyValue(propertyInfo, propJToken);

                propertyInfo.SetValue(obj, propValue);
            }

            return obj;
        }

        private object GetPropertyValue(PropertyInfo propertyInfo, JToken propJToken)
        {
            return IsValueArchetypeDatatype(propertyInfo.PropertyType)
                ? JsonConvert.DeserializeObject(propJToken["value"].ToString(), propertyInfo.PropertyType,
                    GetArchetypeDatatypeConverter())
                : GetDeserializedPropertyValue(propJToken["value"], propertyInfo.PropertyType);
        }

        private bool PropertyLayoutHasFieldsets(IEnumerable<PropertyInfo> properties)
        {
            return properties.Any(HasAsFieldsetAttribute);
        }

        private static bool HasAsFieldsetAttribute(PropertyInfo pInfo)
        {
            return pInfo
                .GetCustomAttributes(typeof(AsFieldsetAttribute), true).Length > 0;
        }

        private IEnumerable SerializeModels(IEnumerable models)
        {
            var fieldsetJson = (from object model in models where null != model select SerializeModel(model)).ToList();

            return String.Join(",", fieldsetJson);
        }

        private string SerializeModel(object value)
        {
            if (value == null)
                return null;

            var jObj = IsExpandoObject(value) 
                ? GetJObjectFromExpandoObject(value as IDictionary<string, object>) 
                : GetJObject(value);

            var fieldsetJson = new StringBuilder();
            var fieldsetWriter = new StringWriter(fieldsetJson);

            using (var jsonWriter = new JsonTextWriter(fieldsetWriter))
            {
                jObj.WriteTo(jsonWriter);
            }

            return fieldsetJson.ToString();
        }

        private bool IsExpandoObject(object value)
        {
            return value.GetType().Name.Equals(typeof (ExpandoObject).Name);
        }

        private JObject GetJObjectFromExpandoObject(IEnumerable<KeyValuePair<string, object>> obj)
        {
            var property = obj.ElementAt(0);
            var alias = property.Key;
            var value = property.Value;

            var jObj = new JObject
                {
                    {
                        "alias",
                        new JValue(alias)
                    }
                };

            var fsProperties = new List<JObject>
            {
                new JObject {new JProperty("alias", alias), new JProperty("value", value)}
            };
            jObj.Add("properties", new JRaw(JsonConvert.SerializeObject(fsProperties)));

            return jObj;
        }

        private JObject GetJObject(object obj)
        {
            var jObj = new JObject
                {
                    {
                        "alias",
                        new JValue(GetFieldsetName(obj.GetType()))
                    }
                };

            var properties = GetSerialiazableProperties(obj);

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
                                                                             GetArchetypeDatatypeConverter()))
                                      : new JValue(GetSerializedPropertyValue(propValue))));

                fsProperties.Add(fsProperty);
            }

            jObj.Add("properties", new JRaw(JsonConvert.SerializeObject(fsProperties)));

            return jObj;
        }

        private IEnumerable<PropertyInfo> GetSerialiazableProperties(object obj)
        {
            return obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => !Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)));
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
            var archetypeDatatypeAttribute = (AsArchetypeAttribute)attributes.FirstOrDefault(attr => attr is AsArchetypeAttribute);

            return archetypeDatatypeAttribute != null ? archetypeDatatypeAttribute.FieldsetName : type.Name;
        }

        private bool IsValueArchetypeDatatype(object value)
        {
            return value != null &&
                IsValueArchetypeDatatype(value.GetType());
        }

        private bool IsValueArchetypeDatatype(Type type)
        {
            return type.GetCustomAttributes(typeof(AsArchetypeAttribute), true).Length > 0;
        }

        private JsonConverter GetArchetypeDatatypeConverter()
        {
            return (JsonConverter)Activator.CreateInstance(GetType());
        }

        private string GetSerializedPropertyValue(object propValue)
        {
            if (propValue == null)
                return String.Empty;

            if (propValue is bool)
                return (bool)propValue ? GetSerializedPropertyValue(1) : GetSerializedPropertyValue(0);

            return String.Format("{0}", propValue);
        }

        private object GetDeserializedPropertyValue(JToken jToken, Type type)
        {            
            if (String.IsNullOrEmpty(jToken.ToString()))
                return GetDefault(type);

            var localType = Nullable.GetUnderlyingType(type) ?? type;

            if (localType == typeof(bool))
                return jToken.ToString() == "1";

            return localType == typeof(DateTime) 
                ? Convert.ToDateTime(jToken.ToString())
                : jToken.ToObject(localType);
        }

        private object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        #endregion
    }
}