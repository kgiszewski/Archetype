using System;
using System.Collections.Generic;
using Archetype.Umbraco.Serialization;
using Newtonsoft.Json;

namespace Archetype.Tests.Serialization
{
    #region json test Models

    [AsArchetype("urlPicker")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class UrlPicker
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("content")]
        public int? Content { get; set; }
        [JsonProperty("media")]
        public int? Media { get; set; }
        [JsonProperty("openInNewWindow")]
        public bool OpenInNewWindow { get; set; }
    }

    [AsArchetype("contactDetails")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class ContactDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("telephone")]
        public string Telephone { get; set; }
        [JsonProperty("mobile")]
        public string Mobile { get; set; }
        [JsonProperty("fax")]
        public string Fax { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("webSite")]
        public UrlPicker WebSite { get; set; }
    }

    [AsArchetype("companyDetails")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class CompanyDetails
    {
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("contactDetails")]
        public ContactDetails ContactDetails { get; set; }
    }

    [AsArchetype("allContactDetails")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class AllContactDetails
    {
        public ContactDetails UserDetails { get; set; }
        public ContactDetails AdminDetails { get; set; }

    }

    [AsArchetype("contactDetailsList")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class ContactDetailsList : List<ContactDetails>
    {
    }

    [AsArchetype("annualStatement")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class AnnualStatement
    {
        public DateTime FiscalYearStart { get; set; }
        public DateTime FiscalYearEnd { get; set; }
        public DateTime? DividendPaymentDate { get; set; }
        public Int32 TotalShares { get; set; }
        public Double Sales { get; set; }
        public Decimal Profit { get; set; }
    }

    [AsArchetype("mergerDetails")]
    [JsonConverter(typeof(ArchetypeJsonConverter))]
    public class MergerDetails
    {
        public DateTime MergerDate { get; set; }
        [JsonIgnore]
        public decimal MergerValue { get; set; }
    }

    #endregion

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
