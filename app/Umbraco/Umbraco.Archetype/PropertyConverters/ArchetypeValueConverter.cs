using System;
using System.Collections.Generic;
using System.Linq;
using Archetype.Umbraco.Extensions;
using Archetype.Umbraco.Models;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using umbraco.interfaces;

namespace Archetype.Umbraco.PropertyConverters
{
    /* based on the Tim Geyssens sample at:  https://github.com/TimGeyssens/MatrixPropEditor/blob/master/SamplePropertyValueConverter/SamplePropertyValueConverter/MatrixValueConverter.cs */
    [PropertyValueType(typeof(Archetype.Umbraco.Models.Archetype))]
    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
    public class ArchetypeValueConverter : PropertyValueConverterBase
    {
        protected JsonSerializerSettings _jsonSettings;

        public ArchetypeValueConverter()
        {
            var dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;

            _jsonSettings = new JsonSerializerSettings { ContractResolver = dcr };
        }

        public ServiceContext Services
        {
            get { return ApplicationContext.Current.Services; }
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditorAlias);
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            var defaultValue = new Models.Archetype();

            if (source == null)
                return defaultValue;

            var sourceString = source.ToString();

            if (sourceString.DetectIsJson())
            {
                try
                {
                    // Deserialize value to archetype model
                    var archetype = JsonConvert.DeserializeObject<Models.Archetype>(sourceString, _jsonSettings);

                    try
                    {
                        // Get list of configured properties and their types 
                        // and map them to the deserialized archetype model
                        var dataTypeCache = new Dictionary<Guid, IDataTypeDefinition>();
                        var preValue = GetArchetypePreValueFromDataTypeId(propertyType.DataTypeId, dataTypeCache);
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
                                        propertyInst.DataTypeId = GetDataTypeByGuid(property.DataTypeGuid).Id;
                                        propertyInst.PropertyEditorAlias = property.PropertyEditorAlias;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    return archetype;
                }
                catch (Exception ex)
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        private ArchetypePreValue GetArchetypePreValueFromDataTypeId(int dataTypeId, IDictionary<Guid, IDataTypeDefinition> dataTypeCache)
        {
            return ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                Constants.CacheKey_PreValueFromDataTypeId + dataTypeId,
                () =>
                {
                    var preValues = Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId);

                    var configJson = preValues.IsDictionaryBased
                        ? preValues.PreValuesAsDictionary[Constants.PreValueAlias].Value
                        : preValues.PreValuesAsArray.First().Value;

                    var config = JsonConvert.DeserializeObject<Models.ArchetypePreValue>(configJson, _jsonSettings);

                    foreach (var fieldset in config.Fieldsets)
                    {
                        foreach (var property in fieldset.Properties)
                        {
                            property.PropertyEditorAlias = GetDataTypeByGuid(property.DataTypeGuid).PropertyEditorAlias;
                        }
                    }

                    return config;

                }) as ArchetypePreValue;
        }    
        
        private IDataTypeDefinition GetDataTypeByGuid(Guid guid)
        {
            return (IDataTypeDefinition) ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                Constants.CacheKey_DataTypeByGuid + guid,
                () => Services.DataTypeService.GetDataTypeDefinitionById(guid));
        }
    }
}
