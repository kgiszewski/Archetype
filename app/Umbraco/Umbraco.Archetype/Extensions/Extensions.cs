using Umbraco.Core;
using Archetype.Umbraco.Models;

namespace Archetype.Umbraco.Extensions
{
    public static class Extensions
    {
        //lifted from the core as it is marked 'internal'
        public static bool DetectIsJson(this string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }

        public static bool IsArchetype(this Property prop)
        {
            return prop.PropertyEditorAlias.InvariantEquals("Imulus.Archetype");
        }
    }
}
