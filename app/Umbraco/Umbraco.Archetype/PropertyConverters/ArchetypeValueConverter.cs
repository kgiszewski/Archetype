using System;
using Archetype.Extensions;
using Archetype.Models;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Archetype.PropertyConverters
{
    /// <summary>
    /// Default property value converter that models the JSON to a C# object.
    /// </summary>
    [PropertyValueType(typeof(Models.ArchetypeModel))]
    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
    public class ArchetypeValueConverter : PropertyValueConverterBase
    {
        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        public ServiceContext Services
        {
            get { return ApplicationContext.Current.Services; }
        }

        /// <summary>
        /// Determines whether the specified property type is converter for Archetype.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns></returns>
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            var isArcheTypePropertyEditor = !String.IsNullOrEmpty(propertyType.PropertyEditorAlias) 
                && propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditorAlias);
            if (!isArcheTypePropertyEditor)
                return false;

            return !ArchetypeHelper.Instance.IsPropertyValueConverterOverridden(propertyType.DataTypeId);
        }

        /// <summary>
        /// Converts the data to source.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="source">The source.</param>
        /// <param name="preview">if set to <c>true</c> [preview].</param>
        /// <returns></returns>
        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            var defaultValue = new ArchetypeModel();

            if (source == null)
                return defaultValue;

            var sourceString = source.ToString();

            if (!sourceString.DetectIsJson())
                return defaultValue;

			using (var timer = DisposableTimer.DebugDuration<ArchetypeValueConverter>(string.Format("ConvertDataToSource ({0})", propertyType != null ? propertyType.PropertyTypeAlias : "null")))
            {
                var archetype = ArchetypeHelper.Instance.DeserializeJsonToArchetype(sourceString,
                    (propertyType != null ? propertyType.DataTypeId : -1),
                    (propertyType != null ? propertyType.ContentType : null));

                return archetype;
            }
        }
    }
}