using System.Collections.Generic;
using Newtonsoft.Json;

namespace Archetype.Models
{
    /// <summary>
    /// Model that represents configured Archetype fieldsets.
    /// </summary>
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

        [JsonProperty("previewImage")]
        public string PreviewImage { get; set; }

        [JsonProperty("properties")]
        public IEnumerable<ArchetypePreValueProperty> Properties { get; set; }

        [JsonProperty("group")]
        public ArchetypePreValueFieldsetGroup Group { get; set; }
    }
}
