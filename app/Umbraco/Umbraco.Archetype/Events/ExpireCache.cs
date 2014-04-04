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
            foreach (var dataType in e.SavedEntities)
            {
                ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(Constants.CacheKey_PreValueFromDataTypeId + dataType.Id);
                ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(Constants.CacheKey_DataTypeByGuid + dataType.Key);
            }
        }
    }
}
