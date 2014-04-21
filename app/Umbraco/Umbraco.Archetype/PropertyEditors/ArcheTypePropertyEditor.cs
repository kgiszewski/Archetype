﻿using System;
using System.Collections.Generic;
using ClientDependency.Core;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PropertyEditors;

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
				: base(wrapped)
			{
				var dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
				dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;

				_jsonSettings = new JsonSerializerSettings { ContractResolver = dcr };
			}

			public override string ConvertDbToString(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
			{
				if (property.Value == null)
					return string.Empty;

				var archetype = JsonConvert.DeserializeObject<Models.Archetype>(property.Value.ToString(), _jsonSettings);

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

				property.Value = JsonConvert.SerializeObject(archetype);

				return base.ConvertDbToString(property, propertyType, dataTypeService);
			}

			public override object ConvertDbToEditor(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
			{
				if (property.Value == null)
					return string.Empty;

				var archetype = JsonConvert.DeserializeObject<Models.Archetype>(property.Value.ToString(), _jsonSettings);

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
				if (editorValue.Value == null)
					return string.Empty;

				var archetype = JsonConvert.DeserializeObject<Models.Archetype>(editorValue.Value.ToString(), _jsonSettings);

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

				return JsonConvert.SerializeObject(archetype);
			}
		}

		#endregion	
	}
}
