using System.Web.Configuration;

namespace Archetype.Models
{
    public class ArchetypeGlobalSettings
    {
        private bool? _isCheckingForUpdates;

        public bool IsCheckingForUpdates
        {
            get
            {
                if (_isCheckingForUpdates == null)
                {
                    var setting = WebConfigurationManager.AppSettings[Constants.CheckForUpdatesAlias];

                    if (setting == null)
                    {
                        _isCheckingForUpdates = true;
                    }

                    var settingValue = true;

                    if (bool.TryParse(setting, out settingValue))
                    {
                        _isCheckingForUpdates = settingValue;
                    }
                    else
                    {
                        _isCheckingForUpdates = true;
                    }
                }

                return _isCheckingForUpdates.Value;
            }

            set
            {
                _isCheckingForUpdates = value;
            }
        }

        public void Save()
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~");

            if (config.AppSettings.Settings[Constants.CheckForUpdatesAlias] != null)
            {
                config.AppSettings.Settings.Remove(Constants.CheckForUpdatesAlias);
            }

            config.AppSettings.Settings.Add(Constants.CheckForUpdatesAlias, IsCheckingForUpdates.ToString());
            config.Save();
        }
    }
}
