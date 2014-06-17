using System.Linq;
using Archetype.PropertyConverters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Archetype.Tests.Serialization
{
    [TestFixture]
    public class ArchetypeJsonConverterComplexModelTest
    {
        private TextList _textList;
        private Captions _captions;
        private PageDetails _pageDetails;

        private SlideShow _slides;
        private Seo _seo;
        private TextPageList _textPages;
        private PageList _pages;
        
        [SetUp]
        public void SetUp()
        {
            InitComplexModels();
            InitComplexTreeModel();
        }

        #region complex nested model tests

        [Test]
        public void PageDetailsModel_Serializes_To_Archetype_Property()
        {
            var result = JsonConvert.SerializeObject(_pageDetails, Formatting.Indented);
            Assert.AreEqual(JsonTestStrings._PAGE_DETAILS_JSON, result);
        }

        [Test]
        public void ConvertComplexModelToArchetype()
        {
            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(_pageDetails, Formatting.Indented);
            var archetype = (Archetype.Models.ArchetypeModel)converter.ConvertDataToSource(null, json, false);

            Assert.NotNull(archetype);
        }

        [Test]
        public void DeserializeComplexModelFromArchetype()
        {
            var result = JsonConvert.DeserializeObject<PageDetails>(JsonTestStrings._PAGE_DETAILS_JSON);

            Assert.NotNull(result);
            Assert.IsInstanceOf<PageDetails>(result);

            Assert.AreEqual("2439,2440,2441,2442,2443,2444,2445,2446,2447,2448,2449,2450,2451,2452,2453", result.Pages);
            Assert.AreEqual("First Page", result.Captions.TextStringArray.ElementAt(0).TextString);
            Assert.AreEqual("Second Page", result.Captions.TextStringArray.ElementAt(1).TextString);
            Assert.AreEqual("Third Page", result.Captions.TextStringArray.ElementAt(2).TextString);
            Assert.AreEqual("Fourth Page", result.Captions.TextStringArray.ElementAt(3).TextString);
        }

        [Test]
        public void SerializeThenDeserializeComplexModelFromArchetype()
        {
            var json = JsonConvert.SerializeObject(_pageDetails);
            var result = JsonConvert.DeserializeObject<PageDetails>(json);

            Assert.NotNull(result);
            Assert.IsInstanceOf<PageDetails>(result);

            Assert.AreEqual(_pageDetails.Pages, result.Pages);
            Assert.AreEqual(_pageDetails.Captions.TextStringArray.ElementAt(0).TextString, result.Captions.TextStringArray.ElementAt(0).TextString);
            Assert.AreEqual(_pageDetails.Captions.TextStringArray.ElementAt(1).TextString, result.Captions.TextStringArray.ElementAt(1).TextString);
            Assert.AreEqual(_pageDetails.Captions.TextStringArray.ElementAt(2).TextString, result.Captions.TextStringArray.ElementAt(2).TextString);
            Assert.AreEqual(_pageDetails.Captions.TextStringArray.ElementAt(3).TextString, result.Captions.TextStringArray.ElementAt(3).TextString);
        }
        #endregion

        #region complex nested tree model tests

        [Test]
        public void ConvertComplexNestedModelToArchetype()
        {
            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(_pages, Formatting.Indented);
            var archetype = (Archetype.Models.ArchetypeModel)converter.ConvertDataToSource(null, json, false);

            Assert.NotNull(archetype);
        }

        [Test]
        public void PagesModel_Serializes_To_Archetype_Property()
        {
            var result = JsonConvert.SerializeObject(_pages, Formatting.Indented);
            Assert.AreEqual(JsonTestStrings._PAGES_JSON, result);
        }

        [Test]
        public void DeserializeComplexTreeModelFromArchetype()
        {
            var result = JsonConvert.DeserializeObject<PageList>(JsonTestStrings._PAGES_JSON);

            Assert.NotNull(result);
            Assert.IsInstanceOf<PageList>(result);

            Assert.AreEqual("1,2,3,4,5,6,7,8", result.Pages.ElementAt(0).Media.Slides);
            Assert.AreEqual("1,2,3,4,5,6,7,8", result.Pages.ElementAt(1).Media.Slides);
            Assert.AreEqual("1,2,3,4,5,6,7,8", result.Pages.ElementAt(2).Media.Slides);

            Assert.AreEqual("Test Meta Title", result.Pages.ElementAt(0).Seo.MetaTitle);
            Assert.AreEqual("Test Meta Description", result.Pages.ElementAt(0).Seo.MetaDescription);
            Assert.AreEqual("Test Meta Title", result.Pages.ElementAt(1).Seo.MetaTitle);
            Assert.AreEqual("Test Meta Description", result.Pages.ElementAt(1).Seo.MetaDescription);
            Assert.AreEqual("Test Meta Title", result.Pages.ElementAt(2).Seo.MetaTitle);
            Assert.AreEqual("Test Meta Description", result.Pages.ElementAt(2).Seo.MetaDescription);

            Assert.AreEqual("Home Page Body", result.Pages.ElementAt(0).BodyText);
            Assert.AreEqual("Home Page", result.Pages.ElementAt(0).PageTitle);
            Assert.AreEqual("About us Page Body", result.Pages.ElementAt(1).BodyText);
            Assert.AreEqual("About us Page", result.Pages.ElementAt(1).PageTitle);
            Assert.AreEqual("Contact us Page Body", result.Pages.ElementAt(2).BodyText);
            Assert.AreEqual("Contact us Page", result.Pages.ElementAt(2).PageTitle);


        }

        [Test]
        public void SerializeThenDeserializeComplexTreeModelFromArchetype()
        {
            var json = JsonConvert.SerializeObject(_pages);
            var result = JsonConvert.DeserializeObject<PageList>(json);

            Assert.NotNull(result);
            Assert.IsInstanceOf<PageList>(result);

            Assert.AreEqual(_pages.Pages.ElementAt(0).Media.Slides, result.Pages.ElementAt(0).Media.Slides);
            Assert.AreEqual(_pages.Pages.ElementAt(1).Media.Slides, result.Pages.ElementAt(1).Media.Slides);
            Assert.AreEqual(_pages.Pages.ElementAt(2).Media.Slides, result.Pages.ElementAt(2).Media.Slides);

            Assert.AreEqual(_pages.Pages.ElementAt(0).Seo.MetaTitle, result.Pages.ElementAt(0).Seo.MetaTitle);
            Assert.AreEqual(_pages.Pages.ElementAt(0).Seo.MetaDescription, result.Pages.ElementAt(0).Seo.MetaDescription);
            Assert.AreEqual(_pages.Pages.ElementAt(1).Seo.MetaTitle, result.Pages.ElementAt(1).Seo.MetaTitle);
            Assert.AreEqual(_pages.Pages.ElementAt(1).Seo.MetaDescription, result.Pages.ElementAt(1).Seo.MetaDescription);
            Assert.AreEqual(_pages.Pages.ElementAt(2).Seo.MetaTitle, result.Pages.ElementAt(2).Seo.MetaTitle);
            Assert.AreEqual(_pages.Pages.ElementAt(2).Seo.MetaDescription, result.Pages.ElementAt(2).Seo.MetaDescription);

            Assert.AreEqual(_pages.Pages.ElementAt(0).BodyText, result.Pages.ElementAt(0).BodyText);
            Assert.AreEqual(_pages.Pages.ElementAt(0).PageTitle, result.Pages.ElementAt(0).PageTitle);
            Assert.AreEqual(_pages.Pages.ElementAt(1).BodyText, result.Pages.ElementAt(1).BodyText);
            Assert.AreEqual(_pages.Pages.ElementAt(1).PageTitle, result.Pages.ElementAt(1).PageTitle);
            Assert.AreEqual(_pages.Pages.ElementAt(2).BodyText, result.Pages.ElementAt(2).BodyText);
            Assert.AreEqual(_pages.Pages.ElementAt(2).PageTitle, result.Pages.ElementAt(2).PageTitle);


        }

        #endregion

        #region utility methods

        private void InitComplexModels()
        {
            _textList = new TextList
            {
                new TextItem{TextString = "First Page"},
                new TextItem{TextString = "Second Page"},
                new TextItem{TextString = "Third Page"},
                new TextItem{TextString = "Fourth Page"}
            };

            _captions = new Captions {TextStringArray = _textList};

            _pageDetails = new PageDetails
            {
                Captions = _captions,
                Pages = "2439,2440,2441,2442,2443,2444,2445,2446,2447,2448,2449,2450,2451,2452,2453"
            };            
        }

        private void InitComplexTreeModel()
        {
            _slides = new SlideShow{Slides = "1,2,3,4,5,6,7,8"};
            _seo = new Seo{MetaDescription = "Test Meta Description", MetaTitle = "Test Meta Title"};
            _textPages = new TextPageList
            {
                new TextPage()
                {
                    PageTitle = "Home Page",
                    BodyText = "Home Page Body",
                    Media = _slides,
                    Seo = _seo
                },
                new TextPage()
                {
                    PageTitle = "About us Page",
                    BodyText = "About us Page Body",
                    Media = _slides,
                    Seo = _seo
                },
                new TextPage()
                {
                    PageTitle = "Contact us Page",
                    BodyText = "Contact us Page Body",
                    Media = _slides,
                    Seo = _seo
                }
            };

            _pages = new PageList
            {
                Pages = _textPages
            };
        }

        #endregion
    }
}
