using System;
using System.ComponentModel;
using System.Globalization;
using Archetype.Extensions;
using Archetype.Models;
using Umbraco.Core.Models;

namespace Archetype.TypeConverters
{
    public class ArchetypeFieldsetModelConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(IPublishedContent) ||
                destinationType == typeof(ArchetypePublishedContent))
            {
                return true;
            }

            if (destinationType == typeof(string))
            {
                // NOTE: The converter needs to return false here, otherwise `ArchetypeHelper.DeserializeJsonToArchetype` fails.
                return false;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is ArchetypeFieldsetModel && (destinationType == typeof(IPublishedContent) || destinationType == typeof(ArchetypePublishedContent)))
            {
                return ((ArchetypeFieldsetModel)value).ToPublishedContent();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}