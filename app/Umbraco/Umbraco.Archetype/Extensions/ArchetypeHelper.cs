using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using Archetype.Models;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using System.Reflection;
using System.Diagnostics;
using System.Net;

/// <summary>
/// The Extensions namespace.
/// </summary>
namespace Archetype.Extensions
{
    /// <summary>
    /// Helper class that handles several Archetype related interactions.
    /// </summary>
    public class ArchetypeHelper
    {
        /// <summary>
        /// The json settings
        /// </summary>
        protected JsonSerializerSettings _jsonSettings;
        /// <summary>
        /// The application
        /// </summary>
        protected ApplicationContext _app;

        /// <summary>
        /// The global settings
        /// </summary>
        private readonly ArchetypeGlobalSettings _globalSettings;

        /// <summary>
        /// The pad lock
        /// </summary>
        private static readonly object _padLock = new object();

        /// <summary>
        /// The instance
        /// </summary>
        private static ArchetypeHelper _instance = new ArchetypeHelper();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        internal static ArchetypeHelper Instance {
            get
            {
                if (_instance == null)
                {
                    lock (_padLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ArchetypeHelper();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchetypeHelper" /> class.
        /// </summary>
        private ArchetypeHelper()
        {
            var dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= BindingFlags.NonPublic;

            _jsonSettings = new JsonSerializerSettings { ContractResolver = dcr };
            _app = ApplicationContext.Current;
            _globalSettings = new ArchetypeGlobalSettings();
        }

        /// <summary>
        /// Gets the json serializer settings.
        /// </summary>
        /// <value>The json serializer settings.</value>
        internal JsonSerializerSettings JsonSerializerSettings { get { return _jsonSettings; } }

        /// <summary>
        /// Deserializes the JSON to archetype.
        /// </summary>
        /// <param name="sourceJson">The source JSON.</param>
        /// <param name="dataTypePreValues">The data type pre values.</param>
        /// <returns>ArchetypeModel.</returns>
        internal ArchetypeModel DeserializeJsonToArchetype(string sourceJson, PreValueCollection dataTypePreValues)
        {
            try
            {
                var archetype = JsonConvert.DeserializeObject<ArchetypeModel>(sourceJson, _jsonSettings);

                try
                {
                    // Get list of configured properties and their types and map them to the deserialized archetype model
                    var preValue = GetArchetypePreValueFromPreValuesCollection(dataTypePreValues);
                    RetrieveAdditionalProperties(ref archetype, preValue);
                }
                catch (Exception ex)
                {
                    LogHelper.Error<ArchetypeHelper>("DeserializeJsonToArchetype", ex);
                }

                return archetype;
            }
            catch
            {
                return new ArchetypeModel();
            }
        }

        /// <summary>
        /// Deserializes the JSON to archetype.
        /// </summary>
        /// <param name="sourceJson">The source JSON.</param>
        /// <param name="dataTypeId">The data type identifier.</param>
        /// <param name="hostContentType">Type of the host content.</param>
        /// <returns>ArchetypeModel.</returns>
        internal ArchetypeModel DeserializeJsonToArchetype(string sourceJson, int dataTypeId, PublishedContentType hostContentType = null)
        {
            try
            {
                var archetype = JsonConvert.DeserializeObject<ArchetypeModel>(sourceJson, _jsonSettings);

                try
                {
                    // Get list of configured properties and their types and map them to the deserialized archetype model
                    var preValue = GetArchetypePreValueFromDataTypeId(dataTypeId);
                    RetrieveAdditionalProperties(ref archetype, preValue, hostContentType);
                }
                catch (Exception ex)
                {
                    LogHelper.Error<ArchetypeHelper>(string.Format("DeserializeJsonToArchetype Error DatatypeId=>{0} SourceJson=>{1}", dataTypeId, sourceJson), ex);
                }

                return archetype;
            }
            catch (Exception ex)
            {
                return new ArchetypeModel();
            }
        }

        /// <summary>
        /// Determines whether datatypeId has had it's PVC overridden.
        /// </summary>
        /// <param name="dataTypeId">The data type identifier.</param>
        /// <returns><c>true</c> if [is property value converter overridden] [the specified data type identifier]; otherwise, <c>false</c>.</returns>
        internal bool IsPropertyValueConverterOverridden(int dataTypeId)
        {
            var prevalues = GetArchetypePreValueFromDataTypeId(dataTypeId);

            if (prevalues == null)
                return false;

            return prevalues.OverrideDefaultPropertyValueConverter;
        }

        /// <summary>
        /// Gets the global settings.
        /// </summary>
        /// <returns>ArchetypeGlobalSettings.</returns>
        internal ArchetypeGlobalSettings GetGlobalSettings()
        {
            return _globalSettings;
        }

        /// <summary>
        /// Sets the check for updates.
        /// </summary>
        /// <param name="isChecking">if set to <c>true</c> [is checking].</param>
        internal void SetCheckForUpdates(bool isChecking)
        {
            _globalSettings.IsCheckingForUpdates = isChecking;
            _globalSettings.Save();
        }

        /// <summary>
        /// Checks for updates.
        /// </summary>
        /// <returns>ArchetypeUpdateNotification.</returns>
        internal ArchetypeUpdateNotification CheckForUpdates()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var id = ConfigurationManager.AppSettings[Constants.IdAlias];

                    if (id == null)
                    {
                        id = Guid.NewGuid().ToString();
                    }

                    var content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        umbracoVersion = ConfigurationManager.AppSettings[Constants.UmbracoVersionAlias],
                        archetypeVersion = DllVersion(),
                        id = id
                    }), Encoding.UTF8, "application/json");
                    
                    var response = client.PostAsync(new Uri(Constants.NotificationUrl), content).Result;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseString = response.Content.ReadAsStringAsync().Result;

                        return JsonConvert.DeserializeObject<ArchetypeUpdateNotification>(responseString);
                    }

                    return new ArchetypeUpdateNotification
                    {
                        IsUpdateAvailable = false
                    };
                }
            }
            catch (Exception ex)
            {
                //if anything goes wrong let's make sure we don't break their site
                return new ArchetypeUpdateNotification
                {
                    IsUpdateAvailable = false
                };
            }
        }

        /// <summary>
        /// Gets the archetype pre value from data type identifier.
        /// </summary>
        /// <param name="dataTypeId">The data type identifier.</param>
        /// <returns>ArchetypePreValue.</returns>
        private ArchetypePreValue GetArchetypePreValueFromDataTypeId(int dataTypeId)
        {
            return _app.ApplicationCache.RuntimeCache.GetCacheItem(
                Constants.CacheKey_PreValueFromDataTypeId + dataTypeId,
                () =>
                {
                    var preValues = _app.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId);

                    var configJson = preValues.IsDictionaryBased
                        ? preValues.PreValuesAsDictionary[Constants.PreValueAlias].Value
                        : preValues.PreValuesAsArray.First().Value;

                    var config = JsonConvert.DeserializeObject<ArchetypePreValue>(configJson, _jsonSettings);
                    RetrieveAdditionalProperties(ref config);

                    return config;

                }) as ArchetypePreValue;
        }

        /// <summary>
        /// Gets the archetype pre value from pre values collection.
        /// </summary>
        /// <param name="dataTypePreValues">The data type pre values.</param>
        /// <returns>ArchetypePreValue.</returns>
        private ArchetypePreValue GetArchetypePreValueFromPreValuesCollection(PreValueCollection dataTypePreValues)
        {
            var preValueAsString = dataTypePreValues.PreValuesAsDictionary.First().Value.Value;
            var preValue = JsonConvert.DeserializeObject<ArchetypePreValue>(preValueAsString, _jsonSettings);
            return preValue;
        }

        /// <summary>
        /// Gets the data type by unique identifier.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>IDataTypeDefinition.</returns>
        internal IDataTypeDefinition GetDataTypeByGuid(Guid guid)
        {
            return (IDataTypeDefinition)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                Constants.CacheKey_DataTypeByGuid + guid,
                () => _app.Services.DataTypeService.GetDataTypeDefinitionById(guid));
        }

        /// <summary>
        /// Retrieves additional metadata that isn't available on the stored model of an Archetype
        /// </summary>
        /// <param name="archetype">The Archetype to add the additional metadata to</param>
        /// <param name="preValue">The configuration of the Archetype</param>
        /// <param name="hostContentType">Type of the host content.</param>
        private void RetrieveAdditionalProperties(ref ArchetypeModel archetype, ArchetypePreValue preValue, PublishedContentType hostContentType = null)
        {
            foreach (var fieldset in preValue.Fieldsets)
            {
                var fieldsetAlias = fieldset.Alias;
                foreach (var fieldsetInst in archetype.Fieldsets.Where(x => x.Alias == fieldsetAlias))
                {
                    foreach (var property in fieldset.Properties)
                    {
                        var propertyAlias = property.Alias;
                        foreach (var propertyInst in fieldsetInst.Properties.Where(x => x.Alias == propertyAlias))
                        {
                            propertyInst.DataTypeGuid = property.DataTypeGuid.ToString();
                            propertyInst.DataTypeId = GetDataTypeByGuid(property.DataTypeGuid).Id;
                            propertyInst.PropertyEditorAlias = property.PropertyEditorAlias;
                            propertyInst.HostContentType = hostContentType;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves additional metadata that isn't available on the stored model of an ArchetypePreValue
        /// </summary>
        /// <param name="preValue">The configuration of the Archetype</param>
        private void RetrieveAdditionalProperties(ref ArchetypePreValue preValue)
        {
            foreach (var fieldset in preValue.Fieldsets)
            {
                foreach (var property in fieldset.Properties)
                {
                    property.PropertyEditorAlias = GetDataTypeByGuid(property.DataTypeGuid).PropertyEditorAlias;
                }
            }
        }


        /// <summary>
        /// Gets the DLL version from the file.
        /// </summary>
        /// <returns>System.String.</returns>
        internal string DllVersion()
        {
            var asm = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(asm.Location);

            return fvi.FileVersion;
        }
    }
}