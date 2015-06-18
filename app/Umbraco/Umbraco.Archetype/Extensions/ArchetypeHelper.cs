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
    public class ArchetypeHelper
    {
        protected JsonSerializerSettings _jsonSettings;
        protected ApplicationContext _app;

        private static readonly ArchetypeHelper _instance = new ArchetypeHelper();

        internal static ArchetypeHelper Instance { get { return _instance; } }

        internal ArchetypeHelper()
        {
            var dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;

            _jsonSettings = new JsonSerializerSettings { ContractResolver = dcr };
            _app = ApplicationContext.Current;
        }

        internal JsonSerializerSettings JsonSerializerSettings { get { return _jsonSettings; } }

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
                    LogHelper.Error<ArchetypeHelper>("DeserializeJsonToArchetype", ex);
                }

                return archetype;
            }
            catch
            {
                return new ArchetypeModel();
            }
        }

        internal bool IsPropertyValueConverterOverridden(int dataTypeId)
        {
            var prevalues = GetArchetypePreValueFromDataTypeId(dataTypeId);
            if (prevalues == null)
                return false;

            return prevalues.OverrideDefaultPropertyValueConverter;
        }

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

        private ArchetypePreValue GetArchetypePreValueFromPreValuesCollection(PreValueCollection dataTypePreValues)
        {
            var preValueAsString = dataTypePreValues.PreValuesAsDictionary.First().Value.Value;
            var preValue = JsonConvert.DeserializeObject<ArchetypePreValue>(preValueAsString, _jsonSettings);
            return preValue;
        }

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