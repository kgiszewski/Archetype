using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archetype.Umbraco.Serialization;
using Newtonsoft.Json;

namespace Archetype.Tests.Serialization.Enumerable
{        
    [AsArchetype("aboutUs")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class AboutUs
    {
        [AsFieldset]
        [JsonProperty("contacts")]
        public IEnumerable<ContactDetails> Contacts { get; set; } 
        [AsFieldset]
        [JsonProperty("testimonials")]
        public IEnumerable<String> Testimonials { get; set; }
    }

    [AsArchetype("feedback")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class Feedback
    {
        [AsFieldset]
        [JsonProperty("testimonials")]
        public IEnumerable<String> Testimonials { get; set; }
    }
}
