using Archetype.Models;
using Umbraco.Core.Models;

namespace Archetype.Extensions
{
    public static class ArchetypeFieldsetModelExtensions
    {
        public static IPublishedContent ToPublishedContent(this ArchetypeFieldsetModel fieldset)
        {
            return new ArchetypePublishedContent(fieldset);
        }
    }
}