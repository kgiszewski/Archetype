using System.Collections.Generic;
using Archetype.Models;
using Umbraco.Core.Models;

namespace Archetype.Extensions
{
    public static class ArchetypeModelExtensions
    {
        public static IEnumerable<IPublishedContent> ToPublishedContentSet(this ArchetypeModel archetype)
        {
            return new ArchetypePublishedContentSet(archetype);
        }
    }
}