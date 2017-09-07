using System;
using System.IO;
using Newtonsoft.Json;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;

namespace Archetype.Models
{
    public class ArchetypeGlobalSettings
    {
        public bool CheckForUpdates { get; set; }
        public Guid Id { get; set; }

        private static ArchetypeGlobalSettings _instance;

        private static string _pathToConfig = @"~/Config/Archetype.config.js";

        private static readonly string _mappedPathToConfig = IOHelper.MapPath(_pathToConfig);

        private static object _padLock = new object();

        private ArchetypeGlobalSettings()
        {
            
        }

        public static ArchetypeGlobalSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_padLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ArchetypeGlobalSettings();

                            _loadSettingsFromConfigFile();
                        }
                    }
                }

                return _instance;
            }
        }

        public void Save()
        {
            //write to JSON
            var configFileModel = new ArchetypeConfigFileModel
            {
                Id = _instance.Id,
                CheckForUpdates = _instance.CheckForUpdates
            };

            var serializedJson = JsonConvert.SerializeObject(configFileModel, Formatting.Indented);

            File.WriteAllText(_mappedPathToConfig, serializedJson);
        }

        private static void _loadSettingsFromConfigFile()
        {
            try
            {
                if (File.Exists(_mappedPathToConfig))
                {
                    //load
                    var deserializedConfigFile = JsonConvert.DeserializeObject<ArchetypeConfigFileModel>(File.ReadAllText(_mappedPathToConfig));

                    if (deserializedConfigFile != null)
                    {
                        _instance.Id = deserializedConfigFile.Id;
                        _instance.CheckForUpdates = deserializedConfigFile.CheckForUpdates;
                    }
                    else
                    {
                        _createNewConfigFile("Config file model was null!");
                    }
                }
                else
                {
                    _createNewConfigFile("Config file was missing!");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<ArchetypeGlobalSettings>(ex.Message, ex);

                _createNewConfigFile("Exception!");
            }
        }

        private static void _createNewConfigFile(string reason)
        {
            //write a new file with defaults
            LogHelper.Info<ArchetypeGlobalSettings>(string.Format("Generating a new config file reason: {0}", reason));

            _instance.Id = Guid.NewGuid();
            _instance.CheckForUpdates = true;
            _instance.Save();
        }
    }
}
