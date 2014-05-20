using System.Collections.Generic;
using Newtonsoft.Json;

namespace Archetype.Models
{
    public class ArchetypePreValueFieldset
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("remove")]
        public bool Remove { get; set; }

        [JsonProperty("collapse")]
        public bool Collapse { get; set; }

        [JsonProperty("labelTemplate")]
        public string LabelTemplate { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("properties")]
        public IEnumerable<ArchetypePreValueProperty> Properties { get; set; }
    }
}
