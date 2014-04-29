using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archetype.Umbraco.PropertyConverters;
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
        
        [SetUp]
        public void SetUp()
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

        #region serialization tests

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
            var archetype = (Archetype.Umbraco.Models.Archetype)converter.ConvertDataToSource(null, json, false);

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

    }
}
