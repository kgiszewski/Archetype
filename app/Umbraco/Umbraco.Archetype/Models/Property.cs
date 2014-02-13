using System.Linq;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Archetype.Umbraco.Models
{
	public class Property
    {
        [JsonProperty("alias")]
        public string Alias { get; internal set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("propertyEditorAlias")]
        public string PropertyEditorAlias { get; internal set; }

        [JsonProperty("dataTypeId")]
        public int DataTypeId { get; internal set; }

		public T GetValue<T>()
		{
			// If the value is of type T, just return it
			if (Value is T)
				return (T)Value;

			// Umbraco has the concept of a IPropertyEditorValueConverter which it 
			// also queries for property resolvers. However I'm not sure what these
			// are for, nor can I find any implementations in core, so am currently
			// just ignoring these when looking up converters.
			// NB: IPropertyEditorValueConverter not to be confused with
			// IPropertyValueConverter which are the ones most people are creating
			var properyType = CreateDummyPropertyType(DataTypeId, PropertyEditorAlias);
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
				var value2 = converter.ConvertDataToSource(properyType, Value, false);

				// If the value is of type T, just return it
				if (value2 is T)
					return (T)value2;

				// Value is not final value type, so try a regular type conversion aswell
				var convertAttempt = value2.TryConvertTo<T>();
				if (convertAttempt.Success)
					return convertAttempt.Result;
			}

			// Value is not final value type, so try a regular type conversion
			var convertAttempt2 = Value.TryConvertTo<T>();
			if (convertAttempt2.Success)
				return convertAttempt2.Result;

			return default(T);
		}

		private PublishedPropertyType CreateDummyPropertyType(int dataTypeId, string propertyEditorAlias)
		{
			return new PublishedPropertyType(null,
				new PropertyType(new DataTypeDefinition(-1, propertyEditorAlias)
				{
					Id = dataTypeId
				}));
		}
    }
}
