namespace Archetype.Umbraco.Models
{
	internal class Property
    {
		internal string Alias { get; set; }
		internal object Value { get; set; }

		// If you decide to make Properties public
		// ensure these properties remain internal
		// as they aren't meant for external use
		internal int DataTypeId { get; set; }
		internal string PropertyEditorAlias { get; set; }
    }
}
