using Archetype.Models;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Archetype.Extensions
{
    public static class ArchetypePropertyModelExtensions
    {
        public static bool IsArchetype(this ArchetypePropertyModel prop)
        {
            return prop.PropertyEditorAlias.InvariantEquals(Constants.PropertyEditorAlias);
        }

        internal static PublishedPropertyType CreateDummyPropertyType(this ArchetypePropertyModel prop)
        {
            return new PublishedPropertyType(prop.HostContentType, new PropertyType(new DataTypeDefinition(-1, prop.PropertyEditorAlias) { Id = prop.DataTypeId }));
        }
    }
}