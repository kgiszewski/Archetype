using System;
using System.Collections.Generic;
using System.Linq;
using Archetype.Umbraco.Extensions;
using ClientDependency.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PropertyEditors;
using Archetype.Umbraco.Database;

namespace Archetype.Umbraco.PropertyEditors
{
	[PropertyEditorAsset(ClientDependencyType.Javascript, "/App_Plugins/Archetype/js/archetype.js")]
	[PropertyEditor("Imulus.Archetype", "Archetype", "/App_Plugins/Archetype/views/archetype.html",
		ValueType = "JSON")]
	public class ArchetypePropertyEditor : PropertyEditor
	{
		#region Pre Value Editor

		protected override PreValueEditor CreatePreValueEditor()
		{
			return new ArchetypePreValueEditor();
		}

		internal class ArchetypePreValueEditor : PreValueEditor
		{
			[PreValueField("archetypeConfig", "Config", "/App_Plugins/Archetype/views/archetype.config.html",
				Description = "(Required) Describe your Archetype.")]
			public string Config { get; set; }

			[PreValueField("hideLabel", "Hide Label", "boolean",
				Description = "Hide the Umbraco property title and description, making the Archetype span the entire page width")]
			public bool HideLabel { get; set; }

            public override IDictionary<string, object> ConvertDbToEditor(IDictionary<string, object> defaultPreVals, PreValueCollection persistedPreVals)
            {
                if(persistedPreVals != null & persistedPreVals.PreValuesAsDictionary != null && persistedPreVals.PreValuesAsDictionary.ContainsKey(Constants.PreValueAlias))
                {
                    // convert the persisted prevalues to a dictionary and get the config prevalue
                    var preValues = persistedPreVals.PreValuesAsDictionary.ToDictionary(i => i.Key, i => i.Value);
                    var config = preValues[Constants.PreValueAlias];

                    Guid configurationId;
                    if(Guid.TryParse(config.Value, out configurationId))
                    {
                        // if the config prevalue is a GUID, try to get the Archetype configuration from DB and update the config prevalue to the stored Archetype configuration
                        var configuration = DatabaseHelper.Get(configurationId);
                        if(configuration != null)
                        {                            
                            preValues[Constants.PreValueAlias] = new PreValue(config.Id, configuration.Configuration, config.SortOrder);
                        }
                        else
                        {
                            // this shouldn't happen, but just in case... return an empty string as Archetype configuration - this will cause the client to use the default configuration
                            preValues[Constants.PreValueAlias] = new PreValue(config.Id, string.Empty, config.SortOrder);
                        }

                        // update the prevalues before letting the base class convert them to editor
                        persistedPreVals.PreValuesAsDictionary = preValues;
                    }
                }

                var baseConversion = base.ConvertDbToEditor(defaultPreVals, persistedPreVals);
                return baseConversion;
            }

            public override IDictionary<string, PreValue> ConvertEditorToDb(IDictionary<string, object> editorValue, PreValueCollection currentValue)
            {
                // first let the base class convert the prevalues to DB
                var baseConversion = base.ConvertEditorToDb(editorValue, currentValue).ToDictionary(i => i.Key, i => i.Value);

                if(baseConversion.ContainsKey(Constants.PreValueAlias))
                {
                    // get the config prevalue
                    var config = baseConversion[Constants.PreValueAlias];

                    // generate a new configuration ID for this configuration
                    var configurationId = Guid.NewGuid();
                    ArchetypeConfiguration configuration = null;

                    // is this an update of an existing configuration?
                    if(currentValue != null && currentValue.PreValuesAsDictionary != null && currentValue.PreValuesAsDictionary.ContainsKey(Constants.PreValueAlias))
                    {
                        // yes, do we already have a configuration stored in DB (is the current prevalue a GUID)?
                        if(Guid.TryParse(currentValue.PreValuesAsDictionary[Constants.PreValueAlias].Value, out configurationId))
                        {
                            // yes, get the currently stored configuration
                            configuration = DatabaseHelper.Get(configurationId);
                        }
                    }

                    // do we have an existing configuration in DB?
                    if(configuration == null)
                    {
                        // no, create a new
                        configuration = new ArchetypeConfiguration
                        {
                            Id = configurationId,
                            Configuration = config.Value
                        };
                        DatabaseHelper.Add(configuration);
                    }
                    else
                    {
                        // yes, update the existing configuration
                        configuration.Configuration = config.Value;
                        DatabaseHelper.Update(configuration);
                    }

                    // store the configuration ID as prevalue 
                    baseConversion[Constants.PreValueAlias] = new PreValue(config.Id, configurationId.ToString(), config.SortOrder);
                }

                return baseConversion;
            }
		}

		#endregion

		#region Value Editor

		protected override PropertyValueEditor CreateValueEditor()
		{
			return new ArchetypePropertyValueEditor(base.CreateValueEditor());
		}

		internal class ArchetypePropertyValueEditor : PropertyValueEditorWrapper
		{
			protected JsonSerializerSettings _jsonSettings;

			public ArchetypePropertyValueEditor(PropertyValueEditor wrapped)
				: base(wrapped) { }

			public override string ConvertDbToString(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
			{
				if (property.Value == null || property.Value.ToString() == "")
					return string.Empty;

			    var archetype = new ArchetypeHelper().DeserializeJsonToArchetype(property.Value.ToString(), propertyType.DataTypeDefinitionId);

				foreach (var fieldset in archetype.Fieldsets)
				{
					foreach (var propDef in fieldset.Properties)
					{
						var dtd = dataTypeService.GetDataTypeDefinitionById(Guid.Parse(propDef.DataTypeGuid));
						var propType = new PropertyType(dtd) { Alias = propDef.Alias };
						var prop = new Property(propType, propDef.Value);
						var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
						propDef.Value = propEditor.ValueEditor.ConvertDbToString(prop, propType, dataTypeService);
					}
				}

                return archetype.SerializeForPersistence();
			}

			public override object ConvertDbToEditor(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
			{
				if (property.Value == null || property.Value.ToString() == "")
					return string.Empty;

			    var archetype = new ArchetypeHelper().DeserializeJsonToArchetype(property.Value.ToString(), propertyType.DataTypeDefinitionId);

				foreach (var fieldset in archetype.Fieldsets)
				{
					foreach (var propDef in fieldset.Properties)
					{
						var dtd = dataTypeService.GetDataTypeDefinitionById(Guid.Parse(propDef.DataTypeGuid));
						var propType = new PropertyType(dtd) { Alias = propDef.Alias };
						var prop = new Property(propType, propDef.Value);
						var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
						propDef.Value = propEditor.ValueEditor.ConvertDbToEditor(prop, propType, dataTypeService);
					}
				}

				return archetype;
			}
            public override object ConvertEditorToDb(ContentPropertyData editorValue, object currentValue)
			{
				if (editorValue.Value == null || editorValue.Value.ToString() == "")
					return string.Empty;

				var archetype = new ArchetypeHelper().DeserializeJsonToArchetype(editorValue.Value.ToString(), editorValue.PreValues);

				foreach (var fieldset in archetype.Fieldsets)
				{
					foreach (var propDef in fieldset.Properties)
					{
						var dtd = ApplicationContext.Current.Services.DataTypeService.GetDataTypeDefinitionById(Guid.Parse(propDef.DataTypeGuid));
						var preValues = ApplicationContext.Current.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dtd.Id);
						var propData = new ContentPropertyData(propDef.Value, preValues, new Dictionary<string, object>());
						var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
						propDef.Value = propEditor.ValueEditor.ConvertEditorToDb(propData, propDef.Value);
					}
				}

                return archetype.SerializeForPersistence();
			}
		}

		#endregion	
    }
}
