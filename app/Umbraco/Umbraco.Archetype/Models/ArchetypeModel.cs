using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Archetype.Umbraco.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Archetype.Models
{
    [JsonObject]
    public class ArchetypeModel : IEnumerable<ArchetypeFieldsetModel>
    {
        [JsonProperty("fieldsets")]
        internal IEnumerable<ArchetypeFieldsetModel> Fieldsets { get; set; }

        public ArchetypeModel()
        {
            Fieldsets = new List<ArchetypeFieldsetModel>();
        }

        public IEnumerator<ArchetypeFieldsetModel> GetEnumerator()
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

            return json.ToString(Formatting.None).DelintArchetypeJson();
        }
    }
}
