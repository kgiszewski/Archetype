using System.Linq;
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

        public static IPublishedContent ToPublishedContent(this ArchetypeFieldsetModel fieldset, ArchetypeModel archetype)
        {
            var contentSet = archetype.ToPublishedContentSet();

            var first = contentSet
                .Cast<ArchetypePublishedContent>()
                .FirstOrDefault(x => x.ArchetypeFieldset == fieldset);

            return first ?? new ArchetypePublishedContent(fieldset);
        }
    }
}