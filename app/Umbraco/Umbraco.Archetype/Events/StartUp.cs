using System;
using System.Web.Configuration;
using Umbraco.Core;

namespace Archetype.Events
{
    public class StartUp : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);

            var config = WebConfigurationManager.OpenWebConfiguration("~");

            //do we have an Archetype Id?
            if (config.AppSettings.Settings[Constants.IdAlias] == null)
            {
                //guess we need one
                config.AppSettings.Settings.Add(Constants.IdAlias, Guid.NewGuid().ToString());
                config.Save();
            }
            else
            {
                //we have the setting, but is it a GUID?
                Guid id;

                if (!Guid.TryParse(config.AppSettings.Settings[Constants.IdAlias].Value, out id))
                {
                    config.AppSettings.Settings.Remove(Constants.IdAlias);
                    config.AppSettings.Settings.Add(Constants.IdAlias, Guid.NewGuid().ToString());
                    config.Save();
                }

                //guess we're g2g
            }
        }
    }
}
