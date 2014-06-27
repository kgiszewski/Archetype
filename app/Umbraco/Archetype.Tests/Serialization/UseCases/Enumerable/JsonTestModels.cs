using System;
using System.Collections.Generic;
using Archetype.Serialization;
using Newtonsoft.Json;

namespace Archetype.Tests.Serialization.UseCases.Enumerable
{
    #region Feedback - single fieldset with multiple items

    [AsArchetype("feedbacks")] /* Note: when inheriting from a list, archetype alias is not used */
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class Feedbacks : List<Feedback>
    {
    }

    [AsArchetype("feedback")] /* Must have same archetype alias as list class */
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class Feedback
    {
        [JsonProperty("testimonial")]
        public String Testimonial { get; set; }
    }

    #endregion   
 
    #region Captions - a root fieldset which contains another fieldset list

    [AsArchetype("captions")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class Captions
    {
        [JsonProperty("captions")]
        public List<Text> TextArray { get; set; }
    }

    [AsArchetype("textArray")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class Text
    {
        [JsonProperty("textstring")]
        public String TextString { get; set; }
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
