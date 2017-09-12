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
using Archetype.Extensions;
using Archetype.Models;

namespace Archetype.Api
{
    /// <summary>
    /// Controller that handles datatype related interactions.
    /// </summary>
    /// <seealso cref="Umbraco.Web.Editors.UmbracoAuthorizedJsonController" />
    [PluginController("ArchetypeApi")]
    public class ArchetypeDataTypeController : UmbracoAuthorizedJsonController
    {
        public IEnumerable<object> GetAllPropertyEditors()
        {
            return
                global::Umbraco.Core.PropertyEditors.PropertyEditorResolver.Current.PropertyEditors
                    .Select(x => new { defaultPreValues = x.DefaultPreValuesForArchetype(), alias = x.Alias, view = x.ValueEditor.View });
        }

        /// <summary>
        /// Gets all datatypes.
        /// </summary>
        /// <returns></returns>
        public object GetAll()
        {
            var dataTypes = Services.DataTypeService.GetAllDataTypeDefinitions();
            return dataTypes.Select(t => new { guid = t.Key, name = t.Name });
        }

        /// <summary>
        /// Gets the datatype by GUID.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
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

        /// <summary>
        /// Gets the datatype by GUID.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="contentTypeAlias">The content type alias.</param>
        /// <param name="propertyTypeAlias">The property type alias.</param>
        /// <param name="archetypeAlias">The archetype alias.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
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

        /// <summary>
        /// Returns the DLL version from the file.
        /// </summary>
        /// <returns></returns>
        public object GetDllVersion()
        {
            return new { dllVersion = ArchetypeHelper.Instance.DllVersion() };
        }

        /// <summary>
        /// Globals the settings.
        /// </summary>
        /// <returns>System.Object.</returns>
        [HttpGet]
        public object GlobalSettings()
        {
            return new
            {
                isCheckingForUpdates = ArchetypeGlobalSettings.Instance.CheckForUpdates
            };
        }

        /// <summary>
        /// Sets the check for updates.
        /// </summary>
        /// <param name="isChecking">if set to <c>true</c> [is checking].</param>
        [HttpPost]
        public void SetCheckForUpdates([FromBody] bool isChecking)
        {
            ArchetypeGlobalSettings.Instance.CheckForUpdates = isChecking;
            ArchetypeGlobalSettings.Instance.Save();
        }

        /// <summary>
        /// Checks for updates.
        /// </summary>
        /// <returns>System.Object.</returns>
        [HttpPost]
        public object CheckForUpdates()
        {
            if (!ArchetypeGlobalSettings.Instance.CheckForUpdates)
            {
                return new
                {
                    isUpdateAvailable = false
                };
            }

            var updateNotificationModel = ArchetypeHelper.Instance.CheckForUpdates();

            return new
            {
                isUpdateAvailable = updateNotificationModel.IsUpdateAvailable,
                headline = updateNotificationModel.Headline,
                type = updateNotificationModel.Type,
                message = updateNotificationModel.Message,
                url = updateNotificationModel.Url
            };
        }
    }
}
