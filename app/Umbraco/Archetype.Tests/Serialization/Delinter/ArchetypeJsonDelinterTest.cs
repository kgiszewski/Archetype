using System;
using System.Collections.Generic;
using Archetype.Tests.Serialization.Base;
using Archetype.Umbraco.Extensions;
using Archetype.Umbraco.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Archetype.Tests.Serialization.Delinter
{
    [TestFixture]
    public class ArchetypeJsonDelinterTest : RegressionTestBase
    {
        private const string _ESCAPED_JSON = @"{""fieldsets"":[{""properties"":[{""alias"":""slides"",""value"":""2680""}],""alias"":""slides""},{""properties"":[{""alias"":""captions"",""value"":""{\""fieldsets\"":[{\""properties\"":[{\""alias\"":\""textstring\"",\""value\"":\""test1 test1\""}],\""alias\"":\""textstringArray\""},{\""properties\"":[{\""alias\"":\""textstring\"",\""value\"":\""test2  test2\""}],\""alias\"":\""textstringArray\""},{\""properties\"":[{\""alias\"":\""textstring\"",\""value\"":\""test3b   test3b\""}],\""alias\"":\""textstringArray\""}]}""}],""alias"":""captions""}]}";
        private const string _JSON = @"{""fieldsets"":[{""properties"":[{""alias"":""slides"",""value"":""2680""}],""alias"":""slides""},{""properties"":[{""alias"":""captions"",""value"":{""fieldsets"":[{""properties"":[{""alias"":""textstring"",""value"":""test1 test1""}],""alias"":""textstringArray""},{""properties"":[{""alias"":""textstring"",""value"":""test2  test2""}],""alias"":""textstringArray""},{""properties"":[{""alias"":""textstring"",""value"":""test3b   test3b""}],""alias"":""textstringArray""}]}}],""alias"":""captions""}]}";
        private const string _ESCAPED_JSON_DEEP_NESTED = @"{""fieldsets"":[{""properties"":[{""alias"":""pages"",""value"":""""},{""alias"":""captions"",""value"":""{\""fieldsets\"":[{\""properties\"":[{\""alias\"":\""captions\"",\""value\"":\""{\\\""fieldsets\\\"":[{\\\""properties\\\"":[{\\\""alias\\\"":\\\""textString\\\"",\\\""value\\\"":\\\""{\\\\\\\""fieldsets\\\\\\\"":[{\\\\\\\""properties\\\\\\\"":[{\\\\\\\""alias\\\\\\\"":\\\\\\\""textString\\\\\\\"",\\\\\\\""value\\\\\\\"":\\\\\\\""\\\\\\\""}],\\\\\\\""alias\\\\\\\"":\\\\\\\""textItem\\\\\\\""}]}\\\""}],\\\""alias\\\"":\\\""textList\\\""}]}\""}],\""alias\"":\""captions\""}]}""}],""alias"":""pages""}]}";

        private SlideShow _referenceModel;
        [SetUp]
        public void SetUp()
        {
            _referenceModel = new SlideShow()
            {
                Slides = "2680",
                Captions = new Captions()
                {
                    new TextString(){Text = "test1 test1"},
                    new TextString(){Text = "test2  test2"},
                    new TextString(){Text = "test3b   test3b"}
                }
            };
        }        

        [Test]
        public void ReferenceJsonAndModel_Equal_ExpectedJsonAndModel()
        {
            const string expectedJson = _JSON;

            var expectedModel = ConvertArchetypeJsonToModel<SlideShow>(expectedJson);
            var referenceModel = ConvertModelToArchetypeAndBack(_referenceModel);

            Assert.IsInstanceOf<SlideShow>(expectedModel);
            Assert.IsInstanceOf<SlideShow>(referenceModel);

            Assert.AreEqual(3, expectedModel.Captions.Count);
            Assert.AreEqual(3, referenceModel.Captions.Count);

            AssertAreEqual(expectedModel, referenceModel);
        }

        [Test]
        public void UnescapedJsonAndModel_Equal_ExpectedJsonAndModel()
        {
            const string expectedJson = _JSON;
            var unescapedJson = _ESCAPED_JSON.DelintArchetypeJson();

            var expectedModel = ConvertArchetypeJsonToModel<SlideShow>(expectedJson);
            var actualModel = ConvertArchetypeJsonToModel<SlideShow>(unescapedJson);

            Assert.IsInstanceOf<SlideShow>(expectedModel);
            Assert.IsInstanceOf<SlideShow>(actualModel);

            Assert.AreEqual(3, expectedModel.Captions.Count);
            Assert.AreEqual(3, actualModel.Captions.Count);

            AssertAreEqual(expectedModel, actualModel);
        }

        [Test]
        public void DeepNestedJson_Escapes_Correctly()
        {
            var actual = _ESCAPED_JSON_DEEP_NESTED.DelintArchetypeJson();

            Assert.IsNotNullOrEmpty(actual);
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

        [AsArchetype("textstringArray")] /* Note: when inheriting from a list, archetype alias is not used */
        [JsonConverter(typeof(ArchetypeJsonConverter))]
        public class Captions : List<TextString>
        {
        }

        [AsArchetype("textstringArray")] /* Must have same archetype alias as list class */
        [JsonConverter(typeof(ArchetypeJsonConverter))]
        public class TextString
        {
            [JsonProperty("textstring")]
            public String Text { get; set; }
        }
    }
}
