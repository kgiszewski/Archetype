using System;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Web;
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
            return prop.PropertyEditorAlias.InvariantEquals(Constants.PropertyEditorAlias);
        }

        public static string UnescapeJson(this string input)
        {
            if (!input.DetectIsJson())
                return String.Empty;

            var tokens = new Dictionary<string, Regex>
            {
                {"fieldsets", new Regex(@"""\{\\""(fieldsets)\\""\s*:\s*(.+?)\}""")},
                {"properties", new Regex(@"\{\\""(properties)\\""\s*:\s*(.+?)\}")},
                {"alias", new Regex(@"\\""(alias)\\""\s*:\s*\\""(.+?)\\""")},
                {"value", new Regex(@"\\""(value)\\""\s*:\s*\\""(.+?)\\""")},
            };

            input = tokens["fieldsets"].Replace(input, delegate(Match match)
            {
                return String.Format(@"{{""{0}"":{1}}}", match.Groups[1], match.Groups[2]);
            });

            input = tokens["properties"].Replace(input, delegate(Match match)
            {
                return String.Format(@"{{""{0}"":{1}}}", match.Groups[1], match.Groups[2]);
            });

            foreach (var token in tokens.Keys.Skip(2))
            {
                input = tokens[token].Replace(input, delegate(Match match)
                {
                    return String.Format(@"""{0}"":""{1}""", match.Groups[1], match.Groups[2]);
                });
            }

            return input;
        }
    }
}
