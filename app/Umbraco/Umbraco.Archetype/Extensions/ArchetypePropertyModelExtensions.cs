using Archetype.Models;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Archetype.Extensions
{
    /// <summary>
    /// ArchetypePropertyModel extensions.
    /// </summary>
    public static class ArchetypePropertyModelExtensions
    {
        /// <summary>
        /// Determines whether this instance is an archetype.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <returns></returns>
        public static bool IsArchetype(this ArchetypePropertyModel prop)
        {
            return prop.PropertyEditorAlias.InvariantEquals(Constants.PropertyEditorAlias);
        }

        /// <summary>
        /// Creates dummy property type.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <returns></returns>
        internal static PublishedPropertyType CreateDummyPropertyType(this ArchetypePropertyModel prop)
        {
            return new PublishedPropertyType(prop.HostContentType, new PropertyType(new DataTypeDefinition(-1, prop.PropertyEditorAlias) { Id = prop.DataTypeId }));
        }
    }
}