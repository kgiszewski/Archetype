using System.Collections.Generic;
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
        public IEnumerable<object> GetAllPropertyEditors()
        {
            return
                global::Umbraco.Core.PropertyEditors.PropertyEditorResolver.Current.PropertyEditors
                    .Select(x => new {defaultPreValues = x.DefaultPreValues, alias = x.Alias, view = x.ValueEditor.View});
        }

        public object GetById(int id)
        {
            var dataType = Services.DataTypeService.GetDataTypeDefinitionById(id);
            if (dataType == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var dataTypeDisplay = Mapper.Map<IDataTypeDefinition, DataTypeDisplay>(dataType);
            return new { selectedEditor = dataTypeDisplay.SelectedEditor, preValues = dataTypeDisplay.PreValues };
        }
    }
}
