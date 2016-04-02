using System;
using System.Linq;
using Archetype.Models;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Archetype.Extensions
{
    /// <summary>
    /// Helper class that handles several Archetype related interactions.
    /// </summary>
    public class ArchetypeHelper
    {
        protected JsonSerializerSettings _jsonSettings;
        protected ApplicationContext _app;

        private static readonly ArchetypeHelper _instance = new ArchetypeHelper();

        internal static ArchetypeHelper Instance { get { return _instance; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchetypeHelper"/> class.
        /// </summary>
        internal ArchetypeHelper()
        {
            var dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;

            _jsonSettings = new JsonSerializerSettings { ContractResolver = dcr };
            _app = ApplicationContext.Current;
        }

        /// <summary>
        /// Gets the json serializer settings.
        /// </summary>
        /// <value>
        /// The json serializer settings.
        /// </value>
        internal JsonSerializerSettings JsonSerializerSettings { get { return _jsonSettings; } }

        /// <summary>
        /// Deserializes the JSON to archetype.
        /// </summary>
        /// <param name="sourceJson">The source JSON.</param>
        /// <param name="dataTypePreValues">The data type pre values.</param>
        /// <returns></returns>
        internal ArchetypeModel DeserializeJsonToArchetype(string sourceJson, PreValueCollection dataTypePreValues)
        {
            try
            {
                var archetype = JsonConvert.DeserializeObject<ArchetypeModel>(sourceJson, _jsonSettings);

                try
                {
                    // Get list of configured properties and their types and map them to the deserialized archetype model
                    var preValue = GetArchetypePreValueFromPreValuesCollection(dataTypePreValues);
                    RetrieveAdditionalProperties(ref archetype, preValue);
                }
                catch (Exception ex)
                {
                    LogHelper.Error<ArchetypeHelper>("DeserializeJsonToArchetype", ex);
                }

                return archetype;
            }
            catch
            {
                return new ArchetypeModel();
            }
        }

        /// <summary>
        /// Deserializes the JSON to archetype.
        /// </summary>
        /// <param name="sourceJson">The source JSON.</param>
        /// <param name="dataTypeId">The data type identifier.</param>
        /// <param name="hostContentType">Type of the host content.</param>
        /// <returns></returns>
        internal ArchetypeModel DeserializeJsonToArchetype(string sourceJson, int dataTypeId, PublishedContentType hostContentType = null)
        {
            try
            {
                var archetype = JsonConvert.DeserializeObject<ArchetypeModel>(sourceJson, _jsonSettings);

                try
                {
                    // Get list of configured properties and their types and map them to the deserialized archetype model
                    var preValue = GetArchetypePreValueFromDataTypeId(dataTypeId);
                    RetrieveAdditionalProperties(ref archetype, preValue, hostContentType);
                }
                catch (Exception ex)
                {
                    LogHelper.Error<ArchetypeHelper>(string.Format("DeserializeJsonToArchetype Error DatatypeId=>{0} SourceJson=>{1}", dataTypeId, sourceJson), ex);
                }

                return archetype;
            }
            catch
            {
                return new ArchetypeModel();
            }
        }

        /// <summary>
        /// Determines whether datatypeId has had it's PVC overridden.
        /// </summary>
        /// <param name="dataTypeId">The data type identifier.</param>
        /// <returns></returns>
        internal bool IsPropertyValueConverterOverridden(int dataTypeId)
        {
            var prevalues = GetArchetypePreValueFromDataTypeId(dataTypeId);
            if (prevalues == null)
                return false;

            return prevalues.OverrideDefaultPropertyValueConverter;
        }

        /// <summary>
        /// Gets the archetype pre value from data type identifier.
        /// </summary>
        /// <param name="dataTypeId">The data type identifier.</param>
        /// <returns></returns>
        private ArchetypePreValue GetArchetypePreValueFromDataTypeId(int dataTypeId)
        {
            return _app.ApplicationCache.RuntimeCache.GetCacheItem(
                Constants.CacheKey_PreValueFromDataTypeId + dataTypeId,
                () =>
                {
                    var preValues = _app.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId);

                    var configJson = preValues.IsDictionaryBased
                        ? preValues.PreValuesAsDictionary[Constants.PreValueAlias].Value
                        : preValues.PreValuesAsArray.First().Value;

                    var config = JsonConvert.DeserializeObject<ArchetypePreValue>(configJson, _jsonSettings);
                    RetrieveAdditionalProperties(ref config);

                    return config;

                }) as ArchetypePreValue;
        }

        /// <summary>
        /// Gets the archetype pre value from pre values collection.
        /// </summary>
        /// <param name="dataTypePreValues">The data type pre values.</param>
        /// <returns></returns>
        private ArchetypePreValue GetArchetypePreValueFromPreValuesCollection(PreValueCollection dataTypePreValues)
        {
            var preValueAsString = dataTypePreValues.PreValuesAsDictionary.First().Value.Value;
            var preValue = JsonConvert.DeserializeObject<ArchetypePreValue>(preValueAsString, _jsonSettings);
            return preValue;
        }

        /// <summary>
        /// Gets the data type by unique identifier.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns></returns>
        internal IDataTypeDefinition GetDataTypeByGuid(Guid guid)
        {
            return (IDataTypeDefinition)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                Constants.CacheKey_DataTypeByGuid + guid,
                () => _app.Services.DataTypeService.GetDataTypeDefinitionById(guid));
        }

        /// <summary>
        /// Retrieves additional metadata that isn't available on the stored model of an Archetype
        /// </summary>
        /// <param name="archetype">The Archetype to add the additional metadata to</param>
        /// <param name="preValue">The configuration of the Archetype</param>
        private void RetrieveAdditionalProperties(ref ArchetypeModel archetype, ArchetypePreValue preValue, PublishedContentType hostContentType = null)
        {
            foreach (var fieldset in preValue.Fieldsets)
            {
                var fieldsetAlias = fieldset.Alias;
                foreach (var fieldsetInst in archetype.Fieldsets.Where(x => x.Alias == fieldsetAlias))
                {
                    foreach (var property in fieldset.Properties)
                    {
                        var propertyAlias = property.Alias;
                        foreach (var propertyInst in fieldsetInst.Properties.Where(x => x.Alias == propertyAlias))
                        {
                            propertyInst.DataTypeGuid = property.DataTypeGuid.ToString();
                            propertyInst.DataTypeId = GetDataTypeByGuid(property.DataTypeGuid).Id;
                            propertyInst.PropertyEditorAlias = property.PropertyEditorAlias;
                            propertyInst.HostContentType = hostContentType;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves additional metadata that isn't available on the stored model of an ArchetypePreValue
        /// </summary>
        /// <param name="archetype">The Archetype to add the additional metadata to</param>
        /// <param name="preValue">The configuration of the Archetype</param>
        private void RetrieveAdditionalProperties(ref ArchetypePreValue preValue)
        {
            foreach (var fieldset in preValue.Fieldsets)
            {
                foreach (var property in fieldset.Properties)
                {
                    property.PropertyEditorAlias = GetDataTypeByGuid(property.DataTypeGuid).PropertyEditorAlias;
                }
            }
        }
    }
}