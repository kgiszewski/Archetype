using Newtonsoft.Json;

namespace Archetype.Models
{
    /// <summary>
    /// Model that represents configured groupings of Archetype fieldsets.
    /// </summary>
    public class ArchetypePreValueFieldsetGroup
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}