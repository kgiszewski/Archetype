using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Archetype.Events
{
    public class ExpireCache : ApplicationEventHandler
    {
        /// <summary>
        /// Registers our ExpirePreValueCache handler on app starting.
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);
            
            DataTypeService.Saved += ExpirePreValueCache;
        }

        /// <summary>
        /// Expires the pre value cache when a datatype is saved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Umbraco.Core.Events.SaveEventArgs{IDataTypeDefinition}"/> instance containing the event data.</param>
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
