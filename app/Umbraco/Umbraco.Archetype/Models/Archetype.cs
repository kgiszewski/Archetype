using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
