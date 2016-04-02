using System;
using Newtonsoft.Json;

namespace Archetype.Models
{
    /// <summary>
    /// Model that represents a configured property on an Archetype fieldset.
    /// </summary>
    public class ArchetypePreValueProperty
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("remove")]
        public bool Remove { get; set; }

        [JsonProperty("collapse")]
        public bool Collapse { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("helpText")]
        public string HelpText { get; set; }
        
        [JsonProperty("dataTypeGuid")]
        public Guid DataTypeGuid { get; set; }

        [JsonProperty("propertyEditorAlias")]
        public string PropertyEditorAlias { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("regEx")]
        public string RegEx { get; set; }
    }
}