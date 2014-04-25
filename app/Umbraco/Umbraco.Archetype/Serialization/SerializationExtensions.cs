using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Archetype.Umbraco.Serialization
{
    public static class SerializationExtensions
    {
        //temporary
        public static string GetPropertyDataValue(this IPublishedContent contentPage, string propertyAlias)
        {
            if (contentPage == null || !contentPage.HasProperty(propertyAlias))
                return null;

            return (string)contentPage.Properties.Single(p => p.PropertyTypeAlias == propertyAlias).DataValue;
        }

        public static T GetModelFromArchetype<T>(this IPublishedContent content,
            string cmsFieldAlias, bool returnInstanceIfNull = false)
            where T : class, new()
        {
            var archetypeJson = content.GetPropertyDataValue(cmsFieldAlias);
            return JsonConvert.DeserializeObject<T>(archetypeJson) ??
                   (returnInstanceIfNull ? Activator.CreateInstance<T>() : default(T));
        }
    }
}
