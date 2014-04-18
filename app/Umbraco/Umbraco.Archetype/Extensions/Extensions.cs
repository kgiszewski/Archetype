using System;
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

        public static HtmlString ParseMacros(this string input, UmbracoHelper umbHelper)
        {
            var regex = new Regex("(<div class=\".*umb-macro-holder.*\".*macroAlias=\"(\\w*)\"(.*)\\/>.*\\/div>)");
            var match = regex.Match(input);

            while (match.Success)
            {
                var parms = match.Groups[3].ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var dictionary = new Dictionary<string, object>();
                foreach (var parm in parms)
                {
                    var thisParm = parm.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    dictionary.Add(thisParm[0], thisParm[1].Substring(1, thisParm[1].Length - 2));
                }

                input = input.Replace(match.Groups[0].ToString(), umbHelper.RenderMacro(match.Groups[2].ToString(), dictionary).ToHtmlString());
                match = regex.Match(input);
            }

            return new HtmlString(input);
        }
    }
}
