using System;
using System.Collections.Generic;
using Archetype.Serialization;
using Newtonsoft.Json;

namespace Archetype.Tests.Serialization.Regression
{
    [AsArchetype("textstringArray")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class TextString
    {
        [JsonProperty("textstring")]
        public String Text { get; set; }
    }

    [AsArchetype("textstringArray")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class TextStringArray : List<TextString>
    {

    }

    [AsArchetype("captions")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class Captions
    {
        [JsonProperty("captions")]
        public TextStringArray TextStringArray { get; set; }
    }

    [AsArchetype("slides")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class SlideShow
    {
        [JsonProperty("slides")]
        public string Slides { get; set; }
        [AsFieldset]
        [JsonProperty("captions")]
        public Captions Captions { get; set; }
    }
}