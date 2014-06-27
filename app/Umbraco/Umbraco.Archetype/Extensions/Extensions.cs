using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Logging;
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

        public static IHtmlString RenderArchetypePartials(this HtmlHelper htmlHelper, ArchetypeModel archetypeModel)
        {
            return RenderPartials(htmlHelper, archetypeModel, null, null);
        }

        public static IHtmlString RenderArchetypePartials(this HtmlHelper htmlHelper, ArchetypeModel archetypeModel, string partialPath)
        {
            return RenderPartials(htmlHelper, archetypeModel, partialPath, null);
        }

        public static IHtmlString RenderArchetypePartials(this HtmlHelper htmlHelper, ArchetypeModel archetypeModel, ViewDataDictionary viewDataDictionary)
        {
            return RenderPartials(htmlHelper, archetypeModel, null, viewDataDictionary);
        }

        public static IHtmlString RenderArchetypePartials(this HtmlHelper htmlHelper, ArchetypeModel archetypeModel, string partialPath, ViewDataDictionary viewDataDictionary)
        {
            return RenderPartials(htmlHelper, archetypeModel, partialPath, viewDataDictionary);
        }

        private static IHtmlString RenderPartials(HtmlHelper htmlHelper, ArchetypeModel archetypeModel, string partialPath, ViewDataDictionary viewDataDictionary)
        {
            var context = HttpContext.Current;

            if (archetypeModel == null || context == null)
            {
                return new HtmlString("");
            }

            var sb = new StringBuilder();
            
            var pathToPartials = "~/Views/Partials/Archetype/";
            if (!string.IsNullOrEmpty(partialPath))
            {
                pathToPartials = partialPath;
            }

            foreach (var fieldsetModel in archetypeModel)
            {
                var partial = pathToPartials + fieldsetModel.Alias + ".cshtml";

                if (System.IO.File.Exists(context.Server.MapPath(partial)))
                {
                    sb.AppendLine(htmlHelper.Partial(partial, fieldsetModel, viewDataDictionary).ToString());
                }
                else
                {
                    LogHelper.Info<ArchetypeModel>(string.Format("The partial for {0} could not be found.  Please create a partial with that name or rename your alias.", context.Server.MapPath(partial)));
                }
            }

            return new HtmlString(sb.ToString());
        }
    }
}
