using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archetype.Umbraco.Models
{
    public class Fieldset
    {
        public string Alias { get; set; }
        public IEnumerable<Property> Properties;

        public Fieldset()
        {
            Properties = new List<Property>();
        }

    }
}
