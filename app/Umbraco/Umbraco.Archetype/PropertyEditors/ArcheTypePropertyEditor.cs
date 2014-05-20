using System;
using System.Collections.Generic;
using System.Linq;
using Archetype.Extensions;
using ClientDependency.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PropertyEditors;
using Umbraco.Core.Logging;
using Umbraco.Web.Models.ContentEditing;

namespace Archetype.PropertyEditors
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
                        try
                        {
						    var dtd = dataTypeService.GetDataTypeDefinitionById(Guid.Parse(propDef.DataTypeGuid));
						    var propType = new PropertyType(dtd) { Alias = propDef.Alias };
						    var prop = new Property(propType, propDef.Value);
						    var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
						    propDef.Value = propEditor.ValueEditor.ConvertDbToString(prop, propType, dataTypeService);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<ArchetypeHelper>(ex.Message, ex);
                        }
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
                        try
                        {
                            var dtd = dataTypeService.GetDataTypeDefinitionById(Guid.Parse(propDef.DataTypeGuid));
                            var propType = new PropertyType(dtd) { Alias = propDef.Alias };
                            var prop = new Property(propType, propDef.Value);
                            var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
                            propDef.Value = propEditor.ValueEditor.ConvertDbToEditor(prop, propType, dataTypeService);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<ArchetypeHelper>(ex.Message, ex);
                        }
					}
				}

				return archetype;
			}
            public override object ConvertEditorToDb(ContentPropertyData editorValue, object currentValue)
			{
				if (editorValue.Value == null || editorValue.Value.ToString() == "")
					return string.Empty;

				var helper = new ArchetypeHelper();
				// attempt to deserialize the current property value as an Archetype
				var currentArchetype = currentValue != null ? helper.DeserializeJsonToArchetype(currentValue.ToString(), editorValue.PreValues) : null;
				var archetype = helper.DeserializeJsonToArchetype(editorValue.Value.ToString(), editorValue.PreValues);

				// get all files uploaded via the file manager (if any)
				var uploadedFiles = editorValue.AdditionalData.ContainsKey("files") ? editorValue.AdditionalData["files"] as IEnumerable<ContentItemFile> : null;

				foreach (var fieldset in archetype.Fieldsets)
				{
					// assign an id to the fieldset if it has none (e.g. newly created fieldset)
					fieldset.Id = fieldset.Id == Guid.Empty ? Guid.NewGuid() : fieldset.Id;
					// find the corresponding fieldset in the current Archetype value (if any)
					var currentFieldset = currentArchetype != null ? currentArchetype.Fieldsets.FirstOrDefault(f => f.Id == fieldset.Id) : null;
					foreach (var propDef in fieldset.Properties)
					{
                        try
                        {
							// find the corresponding property in the current Archetype value (if any)
							var currentProperty = currentFieldset != null ? currentFieldset.Properties.FirstOrDefault(p => p.Alias == propDef.Alias) : null;
						    var dtd = ApplicationContext.Current.Services.DataTypeService.GetDataTypeDefinitionById(Guid.Parse(propDef.DataTypeGuid));
						    var preValues = ApplicationContext.Current.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dtd.Id);

							var additionalData = new Dictionary<string, object>();

							// figure out if we need to pass a files collection in the additional data to the property value editor
							if(uploadedFiles != null && propDef.FileNames != null && propDef.FileNames.Any())
							{
								// get the uploaded files that belongs to this property (if any)
								var propertyFiles = propDef.FileNames.Select(f => uploadedFiles.FirstOrDefault(u => u.FileName == f)).Where(f => f != null).ToList();
								if(propertyFiles.Any())
								{
									additionalData["files"] = propertyFiles;
								}
							}

							var propData = new ContentPropertyData(propDef.Value, preValues, additionalData);
						    var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
							// make sure to send the current property value (if any) to the PE ValueEditor
							propDef.Value = propEditor.ValueEditor.ConvertEditorToDb(propData, currentProperty != null ? currentProperty.Value : null);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<ArchetypeHelper>(ex.Message, ex);
                        }
					}
				}

				// clear the contents of the property files collections before saving to DB
				foreach(var property in archetype.Fieldsets.SelectMany(f => f.Properties.Where(p => p.FileNames != null)).ToList())
				{
					property.FileNames = null;
				}

                return archetype.SerializeForPersistence();
			}
		}

		#endregion	
	}
}
