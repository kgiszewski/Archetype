using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Archetype.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Archetype.Serialization
{
    public class ArchetypeJsonConverter : JsonConverter
    {
        private const string _ROOT_FS_ALIAS = "rootFs";
        
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
                    fs["properties"].ToString().DelintArchetypeJson(), itemType, this);

                obj.GetType().GetMethod("Add").Invoke(obj, new[] { item });
            }

            return obj;
        }

        private object DeserializeObject(object obj, JToken jToken)
        {
            if (jToken.SelectToken("value") != null
                && String.IsNullOrEmpty(jToken.SelectToken("value").ToString()))
                return obj;
            
            var properties = GetSerialiazableProperties(obj).ToList();
            var asFieldset = properties.Where(HasAsFieldsetAttribute).ToList();

            foreach (var propInfo in asFieldset)
            {
                var propAlias = GetJsonPropertyName(propInfo);
                var fsJToken = propInfo.PropertyType.Namespace.Equals("System") 
                                ? GetFieldsetJTokenFromAlias(propAlias, jToken)
                                : GetFieldsetJTokenFromPropertyAlias(propInfo, jToken);

                var propJToken = GetPropertyJToken(fsJToken, propAlias);

                if (propJToken == null)
                    continue;

                var propValue = GetPropertyValue(propInfo, propJToken);

                propInfo.SetValue(obj, propValue);
            }

            if (properties.All(HasAsFieldsetAttribute))
                return obj;

            var objToken = GetFieldsetJTokenFromTypeAlias(obj.GetType(), jToken);

            if (objToken == null)
                return obj;

            if (objToken.SelectToken("properties") != null)
                return PopulateProperties(obj, objToken["properties"]);

            if (objToken is JArray) 
            {
                var defaultFsProperties = properties.Where(pInfo => !HasAsFieldsetAttribute(pInfo)).ToList();                

                foreach (var property in defaultFsProperties)
                {
                    var propJToken = ParseJTokenFromItems(objToken, GetJsonPropertyName(property));
                    PopulateProperty(obj, propJToken["properties"], property);
                }                
            }

            return PopulateProperties(obj, new JArray(objToken));
        }

        private JToken GetPropertyJToken(JToken fsJToken, string propAlias)
        {
            if (fsJToken == null)
                return null;
            
            //make recursive
            if (fsJToken.SelectToken("properties") != null)
                return GetPropertyAliasJToken(propAlias, fsJToken["properties"]);

            var nestedPropJToken = fsJToken
                .SingleOrDefault(p => p.SelectToken("alias").ToString().Equals(propAlias));

            return nestedPropJToken != null ? GetPropertyAliasJToken(propAlias, nestedPropJToken["properties"]) : null;
        }

        private JToken GetFieldsetJTokenFromTypeAlias(Type objType, JToken jToken)
        {
            var objAlias = GetFieldsetName(objType);
            return GetFieldsetJTokenFromAlias(objAlias, jToken);
        }

        private JToken GetFieldsetJTokenFromPropertyAlias(PropertyInfo propInfo, JToken jToken)
        {
            var objAlias = GetJsonPropertyName(propInfo);
            return GetFieldsetJTokenFromAlias(objAlias, jToken);
        }

        private JToken GetFieldsetJTokenFromAlias(string objAlias, JToken jToken)
        {
            JToken propJToken;

            if (TryParseJTokenFromNamedFieldset(jToken, objAlias, out propJToken))
                return propJToken;

            if (TryParseJTokenFromNestedFieldset(jToken, objAlias, out propJToken))
                return propJToken;

            if (TryParseJTokenFromDefaultFieldset(jToken, out propJToken))
                return propJToken;

            return propJToken
                        ?? ParseJTokenFromItem(jToken, objAlias);
        }

        private object PopulateProperties(object obj, JToken jToken)
        {
            var properties = GetSerialiazableProperties(obj);

            foreach (var propertyInfo in properties)
            {
                PopulateProperty(obj, jToken, propertyInfo);
            }

            return obj;
        }

        private void PopulateProperty(object obj, JToken jToken, PropertyInfo propertyInfo)
        {
            var propAlias = GetJsonPropertyName(propertyInfo);
            var propJToken = GetPropertyAliasJToken(propAlias, jToken);

            if (propJToken == null)
                return;

            var propValue = GetPropertyValue(propertyInfo, propJToken);
            propertyInfo.SetValue(obj, propValue);
        }

        private JToken GetPropertyAliasJToken(string propAlias, JToken jToken)
        {
            return jToken.SingleOrDefault(p => p.SelectToken("alias").ToString().Equals(propAlias));
        }

        private object GetPropertyValue(PropertyInfo propertyInfo, JToken propJToken)
        {
            return IsTypeArchetypeDatatype(propertyInfo.PropertyType)
                ? JsonConvert.DeserializeObject(propJToken.ToString().DelintArchetypeJson(), propertyInfo.PropertyType,
                    this)
                    : IsTypeIEnumerableArchetypeDatatype(propertyInfo.PropertyType)
                    ? JsonConvert.DeserializeObject(propJToken["value"].SelectToken("fieldsets").ToString(), propertyInfo.PropertyType,
                    this)
                : GetDeserializedPropertyValue(propJToken, propertyInfo.PropertyType);
        }


        private JToken ParseJTokenFromItem(JToken jToken, string itemAlias)
        {
            return (jToken.SelectToken("alias") != null && jToken["alias"].ToString().Equals(itemAlias))
                    ? jToken
                    : null;
        }

        private JToken ParseJTokenFromItems(JToken jToken, string itemAlias)
        {
            return jToken.SingleOrDefault(jT => ParseJTokenFromItem(jT, itemAlias) != null);
        }

        private bool TryParseJTokenFromNamedFieldset(JToken jToken, string objAlias, out JToken resultToken)
        {
            resultToken = null;

            if (jToken.SelectToken("fieldsets") == null)
                return false;

            resultToken = jToken["fieldsets"].SingleOrDefault(p => p.SelectToken("alias").ToString().Equals(objAlias));

            return resultToken != null;
        }

        private bool TryParseJTokenFromNestedFieldset(JToken jToken, string objAlias, out JToken resultToken)
        {
            resultToken = null;

            if (jToken.SelectToken("value") == null || !jToken["value"].HasValues
                                    || jToken["value"]["fieldsets"] == null)
                return false;

            resultToken = jToken["value"]["fieldsets"].SingleOrDefault(p => p.SelectToken("alias").ToString().Equals(objAlias));

            return resultToken != null;
        }

        private bool TryParseJTokenFromDefaultFieldset(JToken jToken, out JToken resultToken)
        {
            resultToken = jToken.SelectToken("fieldsets");

            return resultToken != null;
        }

        private bool TryParseJTokenAsEnumerable(JToken jToken, out JToken resultToken)
        {
            resultToken = null;
            
            //To Do: Strange newtonsoft behaviour
            var fsToken = jToken.Parent == null ? jToken["fieldsets"] : jToken.Parent.Children().First();
            var jTokenEnumerable = jToken != null && fsToken != null && fsToken.Any();

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

            models = new List<object>();

            var fieldsetModels = properties.Where(HasAsFieldsetAttribute)
                .Select(pInfo =>
                {
                    var fsModel = GetDynamicModel(GetJsonPropertyName(pInfo));
                    fsModel.Add(GetJsonPropertyName(pInfo), pInfo.GetValue(value));
                    return fsModel;
                });

            models = ((IEnumerable<object>)models).Concat(fieldsetModels).ToList();

            var remainingProps = properties.Where(pInfo => !HasAsFieldsetAttribute(pInfo)).ToList();

            if (!remainingProps.Any())
                return models;

            var dynamicModel = GetDynamicModel(GetFieldsetName(value.GetType()));

            foreach (var pInfo in properties.Where(pInfo => !HasAsFieldsetAttribute(pInfo)))
            {
                dynamicModel.Add(GetJsonPropertyName(pInfo), pInfo.GetValue(value));
            }

            models.Add(dynamicModel);

            return models;
        }

        private IDictionary<string, object> GetDynamicModel(string rootFsAlias)
        {
            var dynamicModel = new ExpandoObject() as IDictionary<string, object>;
            dynamicModel.Add(_ROOT_FS_ALIAS, rootFsAlias);
            return dynamicModel;
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

        private JObject GetJObjectFromExpandoObject(IDictionary<string, object> expandoObj)
        {
            var jObj = new JObject
            {
                {
                    "alias",
                    new JValue(expandoObj[_ROOT_FS_ALIAS])
                }
            };

            var fsProperties = new List<JObject>();

            foreach (var item in expandoObj.SkipWhile(i => i.Key.Equals(_ROOT_FS_ALIAS)))
            {
                var property = item;
                var alias = property.Key;
                var value = property.Value;

                fsProperties.Add(new JObject 
                {
                    new JProperty("alias", alias), 
                    new JProperty("value", value != null ? JToken.FromObject(value) : value)
                });                
            }

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

            var properties = GetProperties(obj);

            var fsProperties = new List<JObject>();

            foreach (var propertyInfo in properties)
            {
                var fsProperty = new JObject();
                var jProperty = new JProperty("alias", GetJsonPropertyName(propertyInfo));
                fsProperty.Add(jProperty);

                var propValue = propertyInfo.GetIndexParameters().Length == 0 
                                    ? propertyInfo.GetValue(obj, null)
                                    : obj as string;

                fsProperty.Add(
                    new JProperty("value", GetJPropertyValue(propValue)));

                fsProperties.Add(fsProperty);
            }

            jObj.Add("properties", new JRaw(JsonConvert.SerializeObject(fsProperties)));

            return jObj;
        }

        private object GetJPropertyValue(object value)
        {
            return IsValueArchetypeDatatype(value)
                ? new JRaw(JsonConvert.SerializeObject(value))
                : IsValueIEnumerableArchetypeDatatype(value)
                    ? new JRaw(SerializeModelToFieldset(value as IEnumerable))
                    : new JValue(GetSerializedPropertyValue(value));
        }

        private IEnumerable<PropertyInfo> GetProperties(object obj)
        {
            if (obj.GetType() !=  typeof(string))
                return GetSerialiazableProperties(obj);

            var indexer = ((DefaultMemberAttribute)obj.GetType()
                .GetCustomAttributes(typeof(DefaultMemberAttribute), true)[0]).MemberName;

            return new List<PropertyInfo> { obj.GetType().GetProperty(indexer) };
        }

        private IEnumerable<PropertyInfo> GetSerialiazableProperties(object obj)
        {
            return obj.GetSerialiazableProperties();
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
            var property = JsonConvert.DeserializeObject<ArchetypePropertyModel>(jToken.ToString());

            var method = typeof(ArchetypePropertyModel).GetMethod("GetValue");
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