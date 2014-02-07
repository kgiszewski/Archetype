using System;
using System.Linq;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Models.PublishedContent;
using Archetype.Umbraco.Models;
using Archetype.Umbraco.Extensions;
using Umbraco.Core.Services;

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
            return propertyType.PropertyEditorAlias.Equals("Imulus.Archetype");
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null) return null;
            
            var sourceString = source.ToString();         

            if (sourceString.DetectIsJson())
            {
                try
                {
					// Deserialize value to archetype model
					var archetype = JsonConvert.DeserializeObject<Models.Archetype>(sourceString, _jsonSettings);

					// Get list of configured properties and their types 
					// and map them to the deserialized archetype model
					var preValue = GetArchetypePreValueFromDataTypeId(propertyType.DataTypeId);
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
									propertyInst.DataTypeId = property.DataTypeId;
									propertyInst.PropertyEditorAlias = property.PropertyEditorAlias;
								}
							}
		                }
	                }

                    return archetype;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return sourceString;
        }

	    internal ArchetypePreValue GetArchetypePreValueFromDataTypeId(int dataTypeId)
	    {
			var preValues = Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId);

			var configJson = preValues.IsDictionaryBased
				? preValues.PreValuesAsDictionary["archetypeConfig"].Value
				: preValues.PreValuesAsArray.First().Value;

			var config = JsonConvert.DeserializeObject<Models.ArchetypePreValue>(configJson, _jsonSettings);

		    foreach (var fieldset in config.Fieldsets)
		    {
			    foreach (var property in fieldset.Properties)
			    {
				    // Lookup the properties property editor alias
					// (See if we've already looked it up first though to save a database hit)
				    var propertyWithSameDataType = config.Fieldsets.SelectMany(x => x.Properties)
					    .FirstOrDefault(x => x.DataTypeId == property.DataTypeId && !string.IsNullOrWhiteSpace(x.PropertyEditorAlias));

				    property.PropertyEditorAlias = propertyWithSameDataType != null 
						? propertyWithSameDataType.PropertyEditorAlias
						: Services.DataTypeService.GetDataTypeDefinitionById(property.DataTypeId).PropertyEditorAlias;
			    }
		    }

		    return config;
	    }
    }
}
