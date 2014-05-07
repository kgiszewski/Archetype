using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Archetype.Umbraco.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Archetype.Umbraco.Serialization
{
    public class ArchetypeJsonConverter : JsonConverter
    {
        #region public methods

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var models = GenerateModels(value);

            if (models.Count < 1)
                return;

            if (models.Count == 1 && models[0] == null) 
                return;

            var jObj = SerializeModelToFieldset(models);

            writer.WriteRaw(ApplyFormatting(jObj.ToString(), writer.Formatting));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jToken = JToken.ReadFrom(reader);

            if (jToken == null)
                return null;

            var obj = Activator.CreateInstance(objectType);

            if (null != obj as IEnumerable<object>)
            {
                JToken enumerableToken;

                if (TryParseJTokenAsEnumerable(jToken, out enumerableToken) 
                    || TryParseJTokenAsEnumerable(jToken["value"], out enumerableToken))
                return DeserializeEnumerableObject(obj, enumerableToken);
            }

            return null == jToken as JArray 
                        ? DeserializeObject(obj, jToken) 
                        : PopulateProperties(obj, jToken);
        }

        public override bool CanConvert(Type objectType)
        {
            return IsTypeArchetypeDatatype(objectType);
        }

        #endregion

        #region private methods - deserialization

        private object DeserializeEnumerableObject(object obj, JToken jToken)
        {
            var model = obj as IEnumerable<object>;

            var itemType = model.GetType().BaseType.GetGenericArguments().First();
            foreach (var fs in jToken["fieldsets"].Where(fs => fs["alias"].ToString().Equals(GetFieldsetName(itemType))))
            {
                var item = JsonConvert.DeserializeObject(
                    fs["properties"].ToString(), itemType, this);

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
                var fsJToken = GetPropertyToken(propInfo.PropertyType, jToken);
                var propJToken =
                    fsJToken["properties"].Single(p => p.SelectToken("alias").ToString().Equals(propAlias));

                if (propJToken == null)
                    continue;

                var propValue = GetPropertyValue(propInfo, propJToken);

                propInfo.SetValue(obj, propValue);
            }

            var propToken = GetPropertyToken(obj.GetType(), jToken);

            return propToken == null 
                    ? obj 
                    : PopulateProperties(obj, propToken["properties"] ?? new JArray(propToken));
        }

        private JToken GetPropertyToken(Type objType, JToken jToken)
        {
            var objAlias = GetFieldsetName(objType);
            JToken propJToken;

            if (TryParseJTokenFromFieldset(jToken, out propJToken))
                return propJToken.Single(p => p.SelectToken("alias").ToString().Equals(objAlias));

            if (!TryParseJTokenFromObject(jToken, out propJToken)) 
                return ParseJTokenFromItem(jToken, objAlias);

            return propJToken.SingleOrDefault(p => p.SelectToken("alias").ToString().Equals(objAlias)) 
                        ?? ParseJTokenFromItem(jToken, objAlias);
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
            return IsTypeArchetypeDatatype(propertyInfo.PropertyType)
                ? JsonConvert.DeserializeObject(propJToken.ToString(), propertyInfo.PropertyType,
                    this)
                    : IsTypeIEnumerableArchetypeDatatype(propertyInfo.PropertyType)
                    ? JsonConvert.DeserializeObject(propJToken["value"].SelectToken("fieldsets").ToString(), propertyInfo.PropertyType,
                    this)
                : GetDeserializedPropertyValue(propJToken, propertyInfo.PropertyType);
        }


        private JToken ParseJTokenFromItem(JToken jToken, string itemAlias)
        {
            return (jToken["alias"] != null && jToken["alias"].ToString().Equals(itemAlias))
                    ? jToken
                    : null;
        }

        private bool TryParseJTokenFromFieldset(JToken jToken, out JToken resultToken)
        {
            resultToken = null;
            var isJTokenFieldset = (jToken["fieldsets"] != null);

            if (isJTokenFieldset)
                resultToken = jToken["fieldsets"];

            return isJTokenFieldset;
        }

        private bool TryParseJTokenFromObject(JToken jToken, out JToken resultToken)
        {
            resultToken = null;
            var isJTokenObject = (jToken["value"] != null && jToken["value"].HasValues
                                    && jToken["value"]["fieldsets"] != null);

            if (isJTokenObject)
                resultToken = jToken["value"]["fieldsets"];

            return isJTokenObject;
        }

        private bool TryParseJTokenAsEnumerable(JToken jToken, out JToken resultToken)
        {
            resultToken = null;
            var jTokenEnumerable = jToken != null && jToken["fieldsets"] != null && jToken["fieldsets"].Any();

            if (jTokenEnumerable)
                resultToken = jToken;

            return jTokenEnumerable;
        }

        #endregion

        #region private methods - serialization

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

            models = new List<object>();

            if (dynamicModel.Any())
                models.Add(dynamicModel);

            models = ((IEnumerable<object>)models).Concat(fieldsetModels).ToList();

            return models;
        }

        private JObject SerializeModelToFieldset(IEnumerable models)
        {
            var jObj = new JObject
            {
                {
                    "fieldsets",
                    new JArray(new JRaw(SerializeModels(models)))
                }
            };
            return jObj;
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

        private bool PropertyLayoutHasFieldsets(IEnumerable<PropertyInfo> properties)
        {
            return properties.Any(HasAsFieldsetAttribute);
        }

        private static bool HasAsFieldsetAttribute(PropertyInfo pInfo)
        {
            return pInfo
                .GetCustomAttributes(typeof(AsFieldsetAttribute), true).Length > 0;
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
                                    ? new JRaw(JsonConvert.SerializeObject(propValue))
                                    : IsValueIEnumerableArchetypeDatatype(propValue)
                                        ? new JRaw(SerializeModelToFieldset(propValue as IEnumerable))
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
            return String.IsNullOrEmpty(jToken.ToString()) 
                        ? GetDefault(type) 
                        : GetTypedValue(jToken, type);
        }

        private object GetTypedValue(JToken jToken, Type type)
        {
            var property = JsonConvert.DeserializeObject<Property>(jToken.ToString());
            
            var method = typeof(Property).GetMethod("GetValue");
            var getValue = method.MakeGenericMethod(type);

            return getValue.Invoke(property, null);            
        }

        private object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        #endregion

        #region private methods - common

        private bool IsValueIEnumerableArchetypeDatatype(object value)
        {
            if (value as IEnumerable != null && value.GetType().IsGenericType)
                return IsTypeArchetypeDatatype(value.GetType().GetGenericArguments()[0]);

            return false;
        }

        private bool IsTypeIEnumerableArchetypeDatatype(Type type)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
                return IsTypeArchetypeDatatype(type.GetGenericArguments()[0]);

            return false;
        }

        private bool IsValueArchetypeDatatype(object value)
        {
            return value != null &&
                IsTypeArchetypeDatatype(value.GetType());
        }

        private bool IsTypeArchetypeDatatype(Type type)
        {
            return type.GetCustomAttributes(typeof(AsArchetypeAttribute), true).Length > 0;
        }

        #endregion
    }
}