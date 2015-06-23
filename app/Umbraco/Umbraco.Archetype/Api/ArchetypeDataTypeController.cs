using System.Collections.Generic;
﻿using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using Umbraco.Core.Models;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Mvc;
using Umbraco.Web.Editors;
using Umbraco.Core.PropertyEditors;
using Archetype.Extensions;
using Newtonsoft.Json;

namespace Archetype.Api
{
    [PluginController("ArchetypeApi")]
    public class ArchetypeDataTypeController : UmbracoAuthorizedJsonController
    {

        public IEnumerable<object> GetAllPropertyEditors()
        {
            return
                global::Umbraco.Core.PropertyEditors.PropertyEditorResolver.Current.PropertyEditors
                    .Select(x => new {defaultPreValues = x.DefaultPreValues, alias = x.Alias, view = x.ValueEditor.View});
        }

        public object GetAll() 
        {
            var dataTypes = Services.DataTypeService.GetAllDataTypeDefinitions();
            return dataTypes.Select(t => new { guid = t.Key, name = t.Name });
        }

        public object GetByGuid(Guid guid)
        {
            var dataType = Services.DataTypeService.GetDataTypeDefinitionById(guid);
            if (dataType == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var dataTypeDisplay = Mapper.Map<IDataTypeDefinition, DataTypeDisplay>(dataType);

			// enrich the prevalues with any missing default prevalues for the datatype (e.g. image cropper "focalPoint# prevalue)
			var preValues = dataTypeDisplay.PreValues.ToList();
			if(string.IsNullOrEmpty(dataType.PropertyEditorAlias) == false)
			{
				// fetch the default prevalues for the datatype
				var propEditor = PropertyEditorResolver.Current.GetByAlias(dataType.PropertyEditorAlias);
				if(propEditor != null && propEditor.DefaultPreValues != null && propEditor.DefaultPreValues.Any())
				{
					// add any missing prevalues
					foreach(var missingPreValue in propEditor.DefaultPreValues.Where(p => preValues.Any(pre => pre.Key == p.Key) == false))
					{
						var value = missingPreValue.Value;
						// if the prevalue is a JSON object, deserialize it
						if(value != null && value.ToString().DetectIsJson())
						{
							value = JsonConvert.DeserializeObject(value.ToString());
						}
						preValues.Add(new PreValueFieldDisplay { Key = missingPreValue.Key, Value = value });
					}
				}
			}

			return new { selectedEditor = dataTypeDisplay.SelectedEditor, preValues = preValues };
        }

        public object GetByGuid(Guid guid, string contentTypeAlias, string propertyTypeAlias, string archetypeAlias, int nodeId)
        {
            var dataType = Services.DataTypeService.GetDataTypeDefinitionById(guid);
            if (dataType == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var dataTypeDisplay = Mapper.Map<IDataTypeDefinition, DataTypeDisplay>(dataType);
            return new { selectedEditor = dataTypeDisplay.SelectedEditor, preValues = dataTypeDisplay.PreValues, contentTypeAlias = contentTypeAlias, propertyTypeAlias = propertyTypeAlias, archetypeAlias = archetypeAlias, nodeId = nodeId };
        }
    }
}
