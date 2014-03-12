using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Archetype.Umbraco.Models
{
    [JsonObject]
    public class Archetype : IEnumerable<Fieldset>
    {
        [JsonProperty("fieldsets")]
        internal IEnumerable<Fieldset> Fieldsets { get; set; }

        public Archetype()
        {
            Fieldsets = new List<Fieldset>();
        }

        public IEnumerator<Fieldset> GetEnumerator()
        {
            return this.Fieldsets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
