using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Archetype.Extensions;
using Archetype.Models;
using Umbraco.Core.Models;

namespace Archetype.TypeConverters
{
    public class ArchetypeModelConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(ArchetypePublishedContentSet) ||
                destinationType == typeof(IEnumerable<IPublishedContent>))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is ArchetypeModel && (destinationType == typeof(ArchetypePublishedContentSet) || destinationType == typeof(IEnumerable<IPublishedContent>)))
            {
                return ((ArchetypeModel)value).ToPublishedContentSet();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}