using System.Collections.Generic;
using Archetype.Umbraco.Serialization;
using Newtonsoft.Json;

namespace Archetype.Tests.Serialization.UseCases.Complex
{
    #region complex nested model

    [AsArchetype("pages")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class PageDetails
    {
        [JsonProperty("pages")]
        public string Pages { get; set; }
        [AsFieldset]
        [JsonProperty("captions")]
        public Captions Captions { get; set; }
    }

    [AsArchetype("captions")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class Captions
    {
        [JsonProperty("captions")]
        public TextList TextStringArray { get; set; }
    }

    [AsArchetype("textstringArray")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class TextList : List<TextItem>
    {
    }

    [AsArchetype("textstringArray")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class TextItem
    {
        [JsonProperty("textstring")]
        public string TextString { get; set; }
    }

    #endregion

    #region complex nested tree model

    public abstract class PageBase
    {
        public string PageTitle { get; set; }
        public string BodyText { get; set; }
    }

    [AsArchetype("slideShow")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class SlideShow
    {
        public string Slides { get; set; }
    }

    [AsArchetype("seo")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class Seo
    {
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
    }

    [AsArchetype("TextPage")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class TextPage : PageBase
    {
        [AsFieldset]
        public SlideShow Media { get; set; }

        [AsFieldset]
        public Seo Seo { get; set; }
    }

    [AsArchetype("TextPageList")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class TextPageList : List<TextPage>
    {
    }

    [AsArchetype("Pages")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class PageList
    {
        public TextPageList Pages { get; set; }
    }

    #endregion
}
