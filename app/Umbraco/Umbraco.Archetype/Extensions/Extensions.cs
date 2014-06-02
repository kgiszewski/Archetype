using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Umbraco.Core;
using Archetype.Models;

namespace Archetype.Extensions
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

        public static bool IsArchetype(this ArchetypePropertyModel prop)
        {
            return prop.PropertyEditorAlias.InvariantEquals(Constants.PropertyEditorAlias);
        }
    }
}
