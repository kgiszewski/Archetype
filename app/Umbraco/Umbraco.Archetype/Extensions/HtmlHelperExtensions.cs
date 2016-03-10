using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Archetype.Models;
using Umbraco.Core.Logging;

namespace Archetype.Extensions
{
    /// <summary>
    /// HtmlHelper extenions used for rendering an Archetype.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Renders the partials based on the model, partial path and given viewdata dictionary.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="archetypeModel">The archetype model.</param>
        /// <param name="partialPath">The partial path.</param>
        /// <param name="viewDataDictionary">The view data dictionary.</param>
        /// <returns></returns>
        private static IHtmlString _renderPartials(this HtmlHelper htmlHelper, ArchetypeModel archetypeModel, string partialPath, ViewDataDictionary viewDataDictionary)
        {
            var sb = new StringBuilder();

            foreach (var fieldsetModel in archetypeModel)
            {
                sb.AppendLine(_renderPartial(htmlHelper, fieldsetModel, partialPath, viewDataDictionary).ToString());
            }

            return new HtmlString(sb.ToString());
        }

        /// <summary>
        /// Renders the fieldset partial
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="fieldsetModel"></param>
        /// <param name="partialPath">The partial path.</param>
        /// <param name="viewDataDictionary">The view data dictionary.</param>
        /// <returns></returns>
        private static IHtmlString _renderPartial(HtmlHelper htmlHelper, ArchetypeFieldsetModel fieldsetModel, string partialPath, ViewDataDictionary viewDataDictionary)
        {
            var context = HttpContext.Current;

            if (fieldsetModel == null || context == null)
            {
                return new HtmlString("");
            }

            var sb = new StringBuilder();

            //default
            var pathToPartials = "~/Views/Partials/Archetype/";

            if (!string.IsNullOrEmpty(partialPath))
            {
                pathToPartials = partialPath;
            }

            var partial = string.Format("{0}{1}.cshtml", pathToPartials, fieldsetModel.Alias);

            if (System.IO.File.Exists(context.Server.MapPath(partial)))
            {
                sb.AppendLine(htmlHelper.Partial(partial, fieldsetModel, viewDataDictionary).ToString());
            }
            else
            {
                LogHelper.Info<ArchetypeModel>(string.Format("The partial for {0} could not be found.  Please create a partial with that name or rename your alias.", context.Server.MapPath(partial)));
            }

            return new HtmlString(sb.ToString());
        }

        /// <summary>
        /// Renders a single archtype partial from a fieldset
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="fieldsetModel">The fieldset model.</param>
        /// <returns></returns>
        public static IHtmlString RenderArchetypePartial(this HtmlHelper htmlHelper, ArchetypeFieldsetModel fieldsetModel)
        {
            return _renderPartial(htmlHelper, fieldsetModel, null, null);
        }

        /// <summary>
        /// Renders the archetype partial.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="fieldsetModel">The fieldset model.</param>
        /// <param name="partialView">The partial view.</param>
        /// <returns></returns>
        public static IHtmlString RenderArchetypePartial(this HtmlHelper htmlHelper, ArchetypeFieldsetModel fieldsetModel, string partialView)
        {
            return _renderPartial(htmlHelper, fieldsetModel, partialView, null);
        }

        /// <summary>
        /// Renders the archetype partial.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="fieldsetModel">The fieldset model.</param>
        /// <param name="partialView">The partial view.</param>
        /// <param name="viewDataDictionary">The view data dictionary.</param>
        /// <returns></returns>
        public static IHtmlString RenderArchetypePartial(this HtmlHelper htmlHelper, ArchetypeFieldsetModel fieldsetModel, string partialView, ViewDataDictionary viewDataDictionary)
        {
            return _renderPartial(htmlHelper, fieldsetModel, partialView, viewDataDictionary);
        }

        /// <summary>
        /// Renders the archetype partials.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="archetypeModel">The archetype model.</param>
        /// <returns></returns>
        public static IHtmlString RenderArchetypePartials(this HtmlHelper htmlHelper, ArchetypeModel archetypeModel)
        {
            return _renderPartials(htmlHelper, archetypeModel, null, null);
        }

        /// <summary>
        /// Renders the archetype partials.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="archetypeModel">The archetype model.</param>
        /// <param name="partialPath">The partial path.</param>
        /// <returns></returns>
        public static IHtmlString RenderArchetypePartials(this HtmlHelper htmlHelper, ArchetypeModel archetypeModel, string partialPath)
        {
            return _renderPartials(htmlHelper, archetypeModel, partialPath, null);
        }

        /// <summary>
        /// Renders the archetype partials.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="archetypeModel">The archetype model.</param>
        /// <param name="viewDataDictionary">The view data dictionary.</param>
        /// <returns></returns>
        public static IHtmlString RenderArchetypePartials(this HtmlHelper htmlHelper, ArchetypeModel archetypeModel, ViewDataDictionary viewDataDictionary)
        {
            return _renderPartials(htmlHelper, archetypeModel, null, viewDataDictionary);
        }

        /// <summary>
        /// Renders the archetype partials.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="archetypeModel">The archetype model.</param>
        /// <param name="partialPath">The partial path.</param>
        /// <param name="viewDataDictionary">The view data dictionary.</param>
        /// <returns></returns>
        public static IHtmlString RenderArchetypePartials(this HtmlHelper htmlHelper, ArchetypeModel archetypeModel, string partialPath, ViewDataDictionary viewDataDictionary)
        {
            return htmlHelper._renderPartials(archetypeModel, partialPath, viewDataDictionary);
        }
    }
}