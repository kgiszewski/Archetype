namespace Archetype.Umbraco.Models
{
	public class Property
    {
		public string Alias { get; internal set; }
		public object Value { get; internal set; }
        public string PropertyEditorAlias { get; internal set; }
		internal int DataTypeId { get; set; }
    }
}
