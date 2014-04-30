﻿using System;
using System.Linq;
using Archetype.Umbraco.Models;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Archetype.Umbraco.Database;

namespace Archetype.Umbraco.Extensions
{
    public class ArchetypeHelper
    {

        protected JsonSerializerSettings _jsonSettings;
        protected ApplicationContext _app; 

        internal ArchetypeHelper()
        {
            var dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;

            _jsonSettings = new JsonSerializerSettings { ContractResolver = dcr };
            _app = ApplicationContext.Current;
        }

        internal Models.Archetype DeserializeJsonToArchetype(string sourceJson, PreValueCollection dataTypePreValues)
        {
            try
            {
                var archetype = JsonConvert.DeserializeObject<Models.Archetype>(sourceJson, _jsonSettings);

                try
                {
                    // Get list of configured properties and their types and map them to the deserialized archetype model
                    var preValue = GetArchetypePreValueFromPreValuesCollection(dataTypePreValues);
                    RetrieveAdditionalProperties(ref archetype, preValue);
                }
                catch (Exception ex)
                {
                }

                return archetype;
            }
            catch
            {
                return new Models.Archetype();
            }         
        }

        internal Models.Archetype DeserializeJsonToArchetype(string sourceJson, int dataTypeId)
        {
            try
            {
                var archetype = JsonConvert.DeserializeObject<Models.Archetype>(sourceJson, _jsonSettings);

                try
                {
                    // Get list of configured properties and their types and map them to the deserialized archetype model
                    var preValue = GetArchetypePreValueFromDataTypeId(dataTypeId);
                    RetrieveAdditionalProperties(ref archetype, preValue);
                }
                catch (Exception ex)
                {
                }

                return archetype;
            }
            catch
            {
                return new Models.Archetype();
            }
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

                    var config = DeserializeArchetypePreValue(configJson);
                    RetrieveAdditionalProperties(ref config);

                    return config;

                }) as ArchetypePreValue;
        }  

        private ArchetypePreValue GetArchetypePreValueFromPreValuesCollection(PreValueCollection dataTypePreValues)
        {
            var preValueAsString = dataTypePreValues.PreValuesAsDictionary.First().Value.Value;
            var preValue = DeserializeArchetypePreValue(preValueAsString);
            return preValue;
        }

        private ArchetypePreValue DeserializeArchetypePreValue(string preValueAsString)
        {
            // if the prevalue is a GUID, fetch the configuration from DB and deserialize that 
            Guid configurationId;
            if(Guid.TryParse(preValueAsString, out configurationId))
            {
                var configuration = DatabaseHelper.Get(configurationId);
                if(configuration == null)
                {
                    throw new ArgumentException(string.Format("Could not find Archetype configuration for ID {0}", configurationId));
                }
                preValueAsString = configuration.Configuration;
            }
            var preValue = JsonConvert.DeserializeObject<ArchetypePreValue>(preValueAsString, _jsonSettings);
            return preValue;
        }

        private IDataTypeDefinition GetDataTypeByGuid(Guid guid)
        {
            return (IDataTypeDefinition) ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                Constants.CacheKey_DataTypeByGuid + guid,
                () => _app.Services.DataTypeService.GetDataTypeDefinitionById(guid));
        }

        /// <summary>
        /// Retrieves additional metadata that isn't available on the stored model of an Archetype
        /// </summary>
        /// <param name="archetype">The Archetype to add the additional metadata to</param>
        /// <param name="preValue">The configuration of the Archetype</param>
        private void RetrieveAdditionalProperties(ref Models.Archetype archetype, ArchetypePreValue preValue)
        {
            foreach (var fieldset in preValue.Fieldsets)
            {
                var fieldsetAlias = fieldset.Alias;
                foreach (var fieldsetInst in archetype.Fieldsets.Where(x => x.Alias == fieldsetAlias))
                {
                    foreach (var property in fieldset.Properties)
                    {
                        var propertyAlias = property.Alias;
                        foreach ( var propertyInst in fieldsetInst.Properties.Where(x => x.Alias == propertyAlias))
                        {
                            propertyInst.DataTypeGuid = property.DataTypeGuid.ToString();
                            propertyInst.DataTypeId = GetDataTypeByGuid(property.DataTypeGuid).Id;
                            propertyInst.PropertyEditorAlias = property.PropertyEditorAlias;
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
        private void RetrieveAdditionalProperties(ref Models.ArchetypePreValue preValue)
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
