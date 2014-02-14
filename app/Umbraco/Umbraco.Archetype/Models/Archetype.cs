using System.Collections.Generic;
using Newtonsoft.Json;

namespace Archetype.Umbraco.Models
{
    public class Archetype
    {
        [JsonProperty("fieldsets")]
        public IEnumerable<Fieldset> Fieldsets { get; set; }

        public Archetype()
        {
            Fieldsets = new List<Fieldset>();
        }
    }
}
