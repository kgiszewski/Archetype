using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using Archetype.Umbraco.Models;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Archetype.Umbraco.Extensions
{
    public class ArchetypeHelper
    {

        protected JsonSerializerSettings _jsonSettings;
        protected ApplicationContext _app; 

        public ArchetypeHelper()
        {
            var dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;

            _jsonSettings = new JsonSerializerSettings { ContractResolver = dcr };
            _app = ApplicationContext.Current;
        }

        public Models.Archetype DeserializeJsonToArchetype(string sourceJson, int dataTypeId)
        {
            try
            {
                var archetype = JsonConvert.DeserializeObject<Models.Archetype>(sourceJson, _jsonSettings);

                try
                {
                    // Get list of configured properties and their types and map them to the deserialized archetype model
                    var preValue = GetArchetypePreValueFromDataTypeId(dataTypeId);
                    foreach (var fieldset in preValue.Fieldsets)
                    {
                        var fieldsetAlias = fieldset.Alias;
                        foreach (var fieldsetInst in archetype.Fieldsets.Where(x => x.Alias == fieldsetAlias))
                        {
                            foreach (var property in fieldset.Properties)
                            {
                                var propertyAlias = property.Alias;
                                foreach (
                                    var propertyInst in fieldsetInst.Properties.Where(x => x.Alias == propertyAlias))
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

                    var config = JsonConvert.DeserializeObject<ArchetypePreValue>(configJson, _jsonSettings);

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
                () => _app.Services.DataTypeService.GetDataTypeDefinitionById(guid));
        }

    }
}
