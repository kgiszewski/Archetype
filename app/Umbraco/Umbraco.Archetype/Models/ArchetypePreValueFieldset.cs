using System.Collections.Generic;

namespace Archetype.Umbraco.Models
{
	internal class ArchetypePreValueFieldset
	{
		internal string Alias { get; set; }
		internal IEnumerable<ArchetypePreValueProperty> Properties { get; set; }
	}
}