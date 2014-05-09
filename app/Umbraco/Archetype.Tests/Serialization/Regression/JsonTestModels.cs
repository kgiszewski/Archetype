using System;
using System.Collections.Generic;
using Archetype.Umbraco.Serialization;
using Newtonsoft.Json;

namespace Archetype.Tests.Serialization.Regression
{
    [AsArchetype("simpleModel")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class SimpleModel
    {
        [JsonProperty("text")]        
        public String Text { get; set; }
        [JsonProperty("integer")]
        public int Id { get; set; }
        [JsonProperty("nullableInteger")]
        public int? NullableId { get; set; }
        [JsonProperty("double")]
        public double Amount { get; set; }
        [JsonProperty("nullableDouble")]
        public double? NullableAmount { get; set; }
        [JsonProperty("dateTime")]
        public DateTime DateOne { get; set; }
        [JsonProperty("nullableDateTime")]
        public DateTime? DateTwo { get; set; }
    }

    [AsArchetype("simpleModel")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class SimpleModelWithFieldsets
    {
        [AsFieldset]
        [JsonProperty("text")]
        public string Text { get; set; }
        [AsFieldset]
        [JsonProperty("integer")]
        public int Id { get; set; }
        [AsFieldset]
        [JsonProperty("nullableInteger")]
        public int? NullableId { get; set; }
        [AsFieldset]
        [JsonProperty("double")]
        public double Amount { get; set; }
        [AsFieldset]
        [JsonProperty("nullableDouble")]
        public double? NullableAmount { get; set; }
        [AsFieldset]
        [JsonProperty("dateTime")]
        public DateTime DateOne { get; set; }
        [AsFieldset]
        [JsonProperty("nullableDateTime")]
        public DateTime? DateTwo { get; set; }
    }

    [AsArchetype("simpleModel")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class SimpleModelWithMixedFieldsets
    {
        [AsFieldset]
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("integer")]
        public int Id { get; set; }
        [AsFieldset]
        [JsonProperty("nullableInteger")]
        public int? NullableId { get; set; }
        [JsonProperty("double")]
        public double Amount { get; set; }
        [AsFieldset]
        [JsonProperty("nullableDouble")]
        public double? NullableAmount { get; set; }
        [AsFieldset]
        [JsonProperty("dateTime")]
        public DateTime DateOne { get; set; }
        public DateTime? DateTwo { get; set; }
    }

    [AsArchetype("simpleModels")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class SimpleModels : List<SimpleModel>
    {
    }

}
