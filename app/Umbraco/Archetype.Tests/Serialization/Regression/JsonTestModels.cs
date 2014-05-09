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

    [AsArchetype("simpleModels")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class SimpleModelsWithFieldsets : List<SimpleModelWithFieldsets>
    {
    }

    [AsArchetype("simpleModels")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class SimpleModelsWithMixedFieldsets : List<SimpleModelWithMixedFieldsets>
    {
    }

    [AsArchetype("compoundModel")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class CompoundModel
    {
        [JsonProperty("simpleModel")]
        public SimpleModel SimpleModel { get; set; }
        [JsonProperty("text")]
        public String Text { get; set; }
        [JsonProperty("integer")]
        public int Id { get; set; }
    }

    [AsArchetype("compoundModelWithMixedFieldset")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class CompoundModelWithMixedFieldsetVariant1
    {
        [AsFieldset]
        [JsonProperty("simpleModel")]
        public SimpleModel SimpleModel { get; set; }
        [JsonProperty("text")]
        public String Text { get; set; }
        [JsonProperty("integer")]
        public int Id { get; set; }
    }

    [AsArchetype("compoundModelWithMixedFieldset")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class CompoundModelWithMixedFieldsetVariant2
    {        
        [JsonProperty("simpleModel")]
        public SimpleModel SimpleModel { get; set; }
        [AsFieldset]
        [JsonProperty("text")]
        public String Text { get; set; }
        [AsFieldset]
        [JsonProperty("integer")]
        public int Id { get; set; }
    }

    [AsArchetype("compoundModel")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class CompoundModelWithFieldset
    {
        [AsFieldset]
        [JsonProperty("simpleModel")]
        public SimpleModel SimpleModel { get; set; }
        [AsFieldset]
        [JsonProperty("text")]
        public String Text { get; set; }
        [AsFieldset]
        [JsonProperty("integer")]
        public int Id { get; set; }
    }

    [AsArchetype("compoundModelWithList")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class CompoundModelWithList
    {
        [JsonProperty("simpleModelList")]
        public List<SimpleModel> SimpleModelList { get; set; }
        [JsonProperty("text")]
        public String Text { get; set; }
        [JsonProperty("integer")]
        public int Id { get; set; }
    }

    [AsArchetype("compoundModelWithList")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class CompoundModelWithMixedFieldsetWithList
    {
        [JsonProperty("simpleModelList")]
        public List<SimpleModel> SimpleModelList { get; set; }
        [AsFieldset]
        [JsonProperty("text")]
        public String Text { get; set; }
        [JsonProperty("integer")]
        public int Id { get; set; }
    }

    [AsArchetype("compoundModelWithList")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class CompoundModelWithFieldsetWithList
    {
        [AsFieldset]
        [JsonProperty("simpleModelList")]
        public List<SimpleModel> SimpleModelList { get; set; }
        [AsFieldset]
        [JsonProperty("text")]
        public String Text { get; set; }
        [AsFieldset]
        [JsonProperty("integer")]
        public int Id { get; set; }
    }

    [AsArchetype("nestedModel")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class NestedModel
    {
        [JsonProperty("compoundModel")]
        public CompoundModel CompoundModel { get; set; }
        [JsonProperty("simpleModel")]
        public SimpleModel SimpleModel { get; set; }
        [JsonProperty("text")]
        public String Text { get; set; }
        [JsonProperty("integer")]
        public int Id { get; set; }
    }

    [AsArchetype("nestedModel")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class NestedModelWithMixedFieldset
    {
        [JsonProperty("compoundModel")]
        public CompoundModel CompoundModel { get; set; }
        [AsFieldset]
        [JsonProperty("simpleModel")]
        public SimpleModel SimpleModel { get; set; }
        [JsonProperty("text")]
        public String Text { get; set; }
        [AsFieldset]
        [JsonProperty("integer")]
        public int Id { get; set; }
    }

    [AsArchetype("nestedModel")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class NestedModelWithFieldset
    {
        [AsFieldset]
        [JsonProperty("compoundModel")]
        public CompoundModel CompoundModel { get; set; }
        [AsFieldset]
        [JsonProperty("simpleModel")]
        public SimpleModel SimpleModel { get; set; }
        [AsFieldset]
        [JsonProperty("text")]
        public String Text { get; set; }
        [AsFieldset]
        [JsonProperty("integer")]
        public int Id { get; set; }
    }
}
