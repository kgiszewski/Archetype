using System;
using System.Collections.Generic;
using Archetype.Umbraco.Serialization;
using Newtonsoft.Json;

namespace Archetype.Tests.Serialization.Enumerable
{
    #region Feedback - a simple list based model

    [AsArchetype("feedback")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class Feedbacks : List<Feedback>
    {
    }

    [AsArchetype("feedback")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class Feedback
    {
        [JsonProperty("testimonial")]
        public String Testimonial { get; set; }
    }

    #endregion    
    
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
}
