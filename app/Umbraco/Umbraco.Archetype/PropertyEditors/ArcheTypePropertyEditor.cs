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
using Umbraco.Core.Logging;

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
				if(persistedPreVals != null & persistedPreVals.PreValuesAsDictionary != null && persistedPreVals.PreValuesAsDictionary.Any(IsPreValueChunk()))
				{
					// join all chunks of the JSON configuration
					var configuration = UnChunkify(persistedPreVals);

					// remove all prevalue chunks and add the joined configuration instead
					var preValues = persistedPreVals.PreValuesAsDictionary.ToDictionary(i => i.Key, i => i.Value); 
					preValues.RemoveAll(IsPreValueChunk());
					preValues[Constants.PreValueAlias] = new PreValue(configuration);

					// update the prevalues before letting the base class convert them to editor
					persistedPreVals.PreValuesAsDictionary = preValues;
				}

				return base.ConvertDbToEditor(defaultPreVals, persistedPreVals);
			}

			public override IDictionary<string, PreValue> ConvertEditorToDb(IDictionary<string, object> editorValue, PreValueCollection currentValue)
			{
				// first let the base class convert the prevalues to DB
				var baseConversion = base.ConvertEditorToDb(editorValue, currentValue);

				// next get the JSON configuration and transform it into chunks of an appropriate size for storing as prevalues
				var configuration = baseConversion[Constants.PreValueAlias];
				var preValueChunks = Chunkify(configuration.Value);

				// finally remove the original (and possibly offending in size) prevalue and add the chunks instead
				baseConversion.Remove(Constants.PreValueAlias);
				foreach(var chunk in preValueChunks.OrderBy(kvp => kvp.Key))
				{
					baseConversion[chunk.Key] = new PreValue(chunk.Value);
				}

				return baseConversion;
			}

			public static string UnChunkify(PreValueCollection preValues)
			{
				return string.Join("", preValues.PreValuesAsDictionary.Where(IsPreValueChunk()).OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value.Value));
			}

			private static Dictionary<string, string> Chunkify(string configuration)
			{
				// this is the max size of a prevalue chunk (2500 characters)
				// pro tip: the quickest way to test the chunkification is to lower this number to get more prevalue chunks stored in DB
				const int chunkSize = 2500;
				var chunks = new Dictionary<string, string>();
				// chop the JSON configuration into chunks and return them in dictionary with sortable keys ("archetypeConfig000", "archetypeConfig001" etc),
				// so they can be put back together in the correct order
				for(var i = 0; i < configuration.Length; i += chunkSize)
				{
					chunks[string.Format("{0}{1}", Constants.PreValueAlias, (i / chunkSize).ToString().PadLeft(3, '0'))] = configuration.Substring(i, Math.Min(chunkSize, configuration.Length - i));
				}
				return chunks;
			}

			private static Func<KeyValuePair<string, PreValue>, bool> IsPreValueChunk()
			{
				return kvp => kvp.Key.StartsWith(Constants.PreValueAlias);
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

				var archetype = new ArchetypeHelper().DeserializeJsonToArchetype(editorValue.Value.ToString(), editorValue.PreValues);

				foreach (var fieldset in archetype.Fieldsets)
				{
					foreach (var propDef in fieldset.Properties)
					{
                        try
                        {
						    var dtd = ApplicationContext.Current.Services.DataTypeService.GetDataTypeDefinitionById(Guid.Parse(propDef.DataTypeGuid));
						    var preValues = ApplicationContext.Current.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dtd.Id);
						    var propData = new ContentPropertyData(propDef.Value, preValues, new Dictionary<string, object>());
						    var propEditor = PropertyEditorResolver.Current.GetByAlias(dtd.PropertyEditorAlias);
						    propDef.Value = propEditor.ValueEditor.ConvertEditorToDb(propData, propDef.Value);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<ArchetypeHelper>(ex.Message, ex);
                        }
					}
				}

                return archetype.SerializeForPersistence();
			}
		}

		#endregion	
	}
}
