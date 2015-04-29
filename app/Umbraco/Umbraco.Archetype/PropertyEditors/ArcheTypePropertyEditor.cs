using System;
using System.Collections.Generic;
using System.Linq;
using Archetype.Extensions;
using ClientDependency.Core;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PropertyEditors;

namespace Archetype.PropertyEditors
{
	[PropertyEditorAsset(ClientDependencyType.Javascript, "/App_Plugins/Archetype/js/archetype.js")]
	[PropertyEditor(Constants.PropertyEditorAlias, "Archetype", "/App_Plugins/Archetype/views/archetype.html", ValueType = "JSON")]
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

                //LogHelper.Info<ArchetypeHelper>(property.Value.ToString());

				var archetype = ArchetypeHelper.Instance.DeserializeJsonToArchetype(property.Value.ToString(), propertyType.DataTypeDefinitionId);

				foreach (var fieldset in archetype.Fieldsets)
				{
          fieldset.Properties = fieldset.Properties.Where( p => p.DataTypeGuid != null );
					foreach (var propDef in fieldset.Properties)
					{
                        try
                        {
                            if(propDef == null || propDef.DataTypeGuid == null) continue;
                            var dtd = ArchetypeHelper.Instance.GetDataTypeByGuid(Guid.Parse(propDef.DataTypeGuid));
						    var propType = new PropertyType(dtd) { Alias = propDef.Alias };
						    var prop = new Property(propType, propDef.Value);
						    var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
						    propDef.Value = propEditor.ValueEditor.ConvertDbToString(prop, propType, dataTypeService);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<ArchetypePropertyValueEditor>(ex.Message, ex);
                        }
					}
				}

                return archetype.SerializeForPersistence();
			}

			public override object ConvertDbToEditor(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
			{
				if (property.Value == null || property.Value.ToString() == "")
					return string.Empty;

                //LogHelper.Info<ArchetypeHelper>(property.Value.ToString());

				var archetype = ArchetypeHelper.Instance.DeserializeJsonToArchetype(property.Value.ToString(), propertyType.DataTypeDefinitionId);

				foreach (var fieldset in archetype.Fieldsets)
				{
          fieldset.Properties = fieldset.Properties.Where( p => p.DataTypeGuid != null );
					foreach (var propDef in fieldset.Properties)
					{
                        try
                        {
                            var dtd = ArchetypeHelper.Instance.GetDataTypeByGuid(Guid.Parse(propDef.DataTypeGuid));
                            var propType = new PropertyType(dtd) { Alias = propDef.Alias };
                            var prop = new Property(propType, propDef.Value);
                            var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
                            propDef.Value = propEditor.ValueEditor.ConvertDbToEditor(prop, propType, dataTypeService);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<ArchetypePropertyValueEditor>(ex.Message, ex);
                        }
					}
				}

				return archetype;
			}
            public override object ConvertEditorToDb(ContentPropertyData editorValue, object currentValue)
			{
				if (editorValue.Value == null || editorValue.Value.ToString() == "")
					return string.Empty;

                //LogHelper.Info<ArchetypeHelper>(editorValue.Value.ToString());

				var archetype = ArchetypeHelper.Instance.DeserializeJsonToArchetype(editorValue.Value.ToString(), editorValue.PreValues);

				foreach (var fieldset in archetype.Fieldsets)
				{
          fieldset.Properties = fieldset.Properties.Where( p => p.DataTypeGuid != null );
					foreach (var propDef in fieldset.Properties)
					{
                        try
                        {
                            var dtd = ArchetypeHelper.Instance.GetDataTypeByGuid(Guid.Parse(propDef.DataTypeGuid));
						    var preValues = ApplicationContext.Current.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dtd.Id);
						    var propData = new ContentPropertyData(propDef.Value, preValues, new Dictionary<string, object>());
                            var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
						    propDef.Value = propEditor.ValueEditor.ConvertEditorToDb(propData, propDef.Value);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<ArchetypePropertyValueEditor>(ex.Message, ex);
                        }
					}
				}

                return archetype.SerializeForPersistence();
			}

		    internal virtual PropertyEditor GetPropertyEditor(IDataTypeDefinition dtd)
		    {
		        if (dtd.Id != 0) 
                    return PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);

		        return dtd.PropertyEditorAlias.Equals(Constants.PropertyEditorAlias)
		            ? new ArchetypePropertyEditor()
                    : (PropertyEditor)new TextboxPropertyEditor();
		    }
		}

		#endregion	
	}
}
