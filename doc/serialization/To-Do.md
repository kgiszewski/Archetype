####21.5.2014
1. ~~To get property values to use: ```PropertyValueConverters```~~ (But uses default built-in conversion)
1. ~~Add support for direct use ```IEnumerable<T>``` based types~~ (See below note for support.)
1. ~~Improve dealing with multiple fieldset types (including the ```AsFieldset``` usage issue)~~

Notes
1. Support for ```List<T>``` based types is now available. However, unless you write your own interface based serializer, type declaration is done with ```List<T>```. That is, you cannot use ```IEnumerable<T>``` or ```IList<T>```. Example below:
```csharp
    [AsArchetype("captions")]
    [JsonConverter(typeof(ArachetypeJsonConverter))]
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
```