﻿using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using Umbraco.Core.Models;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Mvc;
using Umbraco.Web.Editors;

namespace Archetype.Umbraco.Api
{
    [PluginController("ArchetypeApi")]
    public class ArchetypeDataTypeController : UmbracoAuthorizedJsonController
    {
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
    }
}
