using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Archetype.Models
{
    [JsonObject]
    public class ArchetypeModel : IEnumerable<ArchetypeFieldsetModel>
    {
        [JsonProperty("fieldsets")]
        public IEnumerable<ArchetypeFieldsetModel> Fieldsets { get; set; }

        public ArchetypeModel()
        {
            Fieldsets = new List<ArchetypeFieldsetModel>();
        }

        public IEnumerator<ArchetypeFieldsetModel> GetEnumerator()
        {
            return this.Fieldsets.Where(f => f.Disabled == false).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public string SerializeForPersistence()
        {
            // clear the contents of the property files collections before serializing (it's temporary state data)
            foreach(var property in Fieldsets.SelectMany(f => f.Properties.Where(p => p.FileNames != null)).ToList())
            {
                property.FileNames = null;
            }

            var json = JObject.Parse(JsonConvert.SerializeObject(this, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

            var propertiesToRemove = new String[] { "propertyEditorAlias", "dataTypeId", "dataTypeGuid", "hostContentType" };

            json.Descendants().OfType<JProperty>()
              .Where(p => propertiesToRemove.Contains(p.Name))
              .ToList()
              .ForEach(x => x.Remove());

            return json.ToString(Formatting.None);
        }
    }
}
