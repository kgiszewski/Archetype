using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Archetype.Umbraco.Events
{
    public class ExpireCache : ApplicationEventHandler
    {

        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);
            
            DataTypeService.Saved += ExpirePreValueCache;
        }

        void ExpirePreValueCache(IDataTypeService sender, global::Umbraco.Core.Events.SaveEventArgs<IDataTypeDefinition> e)
        {
            ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem("Archetype_GetArchetypePreValueFromDataTypeId_" + e.SavedEntities.First().Id);
            ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem("Archetype_GetArchetypePreValueFromDataTypeId_GetPropertyEditorAlias_"+ e.SavedEntities.First().Id);
        }
    }
}
