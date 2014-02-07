using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Archetype.Umbraco.Models
{
    public class Fieldset
    {
        public string Alias { get; set; }

		internal IEnumerable<Property> Properties;

        public Fieldset()
        {
            Properties = new List<Property>();
        }

		#region Helper Methods

		public string GetValue(string propertyAlias)
        {
            return GetValue<string>(propertyAlias);
        }

        public T GetValue<T>(string propertyAlias)
        {
            var property = GetProperty(propertyAlias);

            if (property == null || string.IsNullOrEmpty(property.Value.ToString()))
                return default(T);

			var value = property.Value;

			// If the value is of type T, just return it
			if (value is T)
				return (T)value;

			// Umbraco has the concept of a IPropertyEditorValueConverter which it 
			// also queries for property resolvers. However I'm not sure what these
			// are for, nor can I find any implementations in core, so am currently
			// just ignoring these when looking up converters.
			// NB: IPropertyEditorValueConverter not to be confused with
			// IPropertyValueConverter which are the ones most people are creating
	        var properyType = CreateDummyPropertyType(property.DataTypeId, property.PropertyEditorAlias);
			var converters = PropertyValueConvertersResolver.Current.Converters.ToArray();

			// In umbraco, there are default value converters that try to convert the 
			// value if all else fails. The problem is, they are also in the list of
			// converters, and the means for filtering these out is internal, so
			// we currently have to try ALL converters to see if they can convert
			// rather than just finding the most appropreate. If the ability to filter
			// out default value converters becomes public, the following logic could
			// and probably should be changed.
			foreach (var converter in converters.Where(x => x.IsConverter(properyType)))
	        {
				// Convert the type using a found value converter
				var value2 = converter.ConvertDataToSource(properyType, value, false);

				// If the value is of type T, just return it
				if (value2 is T)
					return (T)value2;

				// Value is not final value type, so try a regular type conversion aswell
				var convertAttempt = value2.TryConvertTo<T>();
				if (convertAttempt.Success)
					return convertAttempt.Result;
	        }

			// Value is not final value type, so try a regular type conversion
			var convertAttempt2 = value.TryConvertTo<T>();
            if (convertAttempt2.Success)
                return convertAttempt2.Result;

            return default(T);
        }

        private Property GetProperty(string propertyAlias)
        {
            return Properties.FirstOrDefault(p => p.Alias.InvariantEquals(propertyAlias));
		}

		private PublishedPropertyType CreateDummyPropertyType(int dataTypeId, string propertyEditorAlias)
	    {
		    return new PublishedPropertyType(null,
			    new PropertyType(new DataTypeDefinition(-1, propertyEditorAlias)
			    {
				    Id = dataTypeId
			    }));
	    }

		#endregion

    }
}
