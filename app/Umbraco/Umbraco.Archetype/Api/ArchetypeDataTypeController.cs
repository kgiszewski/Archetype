using System.Collections.Generic;
﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Http;
using AutoMapper;
using Umbraco.Core.Models;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Mvc;
using Umbraco.Web.Editors;
using Archetype.Extensions;

namespace Archetype.Api
{
    [PluginController("ArchetypeApi")]
    public class ArchetypeDataTypeController : UmbracoAuthorizedJsonController
    {

        public IEnumerable<object> GetAllPropertyEditors()
        {
            return
                global::Umbraco.Core.PropertyEditors.PropertyEditorResolver.Current.PropertyEditors
                    .Select(x => new {defaultPreValues = x.DefaultPreValuesForArchetype(), alias = x.Alias, view = x.ValueEditor.View});
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
            return new { selectedEditor = dataTypeDisplay.SelectedEditor, preValues = dataTypeDisplay.PreValues };
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

        public object GetDllVersion()
        {
            return new {dllVersion = _version()};
        }

        private string _version()
        {
            var asm = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(asm.Location);

            return fvi.FileVersion;
        }
    }
}
