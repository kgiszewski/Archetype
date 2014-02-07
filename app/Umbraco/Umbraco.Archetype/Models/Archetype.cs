using System.Collections.Generic;

namespace Archetype.Umbraco.Models
{
    public class Archetype
    {
	    public IEnumerable<Fieldset> Fieldsets { get; set; }

        public Archetype()
        {
            Fieldsets = new List<Fieldset>();
        }
    }
}
