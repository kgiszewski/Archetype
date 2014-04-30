using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        internal string SerializeForPersistence()
        {
            var json = JObject.Parse(JsonConvert.SerializeObject(this));
            var propertiesToRemove = new String[] { "propertyEditorAlias", "dataTypeId", "dataTypeGuid" };

            json.Descendants().OfType<JProperty>()
              .Where(p => propertiesToRemove.Contains(p.Name))
              .ToList()
              .ForEach(x => x.Remove());

            return json.ToString();
        }
    }
}
