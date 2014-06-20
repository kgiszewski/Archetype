﻿using System;
using System.Collections.Generic;
using System.Linq;
using Archetype.PropertyEditors;
using Archetype.Serialization;
using Archetype.Tests.Serialization.Base;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Archetype.Tests.Serialization.Delinter
{
    [TestFixture]
    public class ArchetypeJsonDelinterTest : RegressionTestBase
    {
        private const string _ESCAPED_JSON = @"{""fieldsets"":[{""properties"":[{""alias"":""slides"",""value"":""2680""}],""alias"":""slides""},{""properties"":[{""alias"":""captions"",""value"":""{\""fieldsets\"":[{\""properties\"":[{\""alias\"":\""textstring\"",\""value\"":\""test1 test1\""}],\""alias\"":\""textstringArray\""},{\""properties\"":[{\""alias\"":\""textstring\"",\""value\"":\""test2  test2\""}],\""alias\"":\""textstringArray\""},{\""properties\"":[{\""alias\"":\""textstring\"",\""value\"":\""test3b   test3b\""}],\""alias\"":\""textstringArray\""}]}""}],""alias"":""captions""}]}";
        private const string _JSON = @"{""fieldsets"":[{""properties"":[{""alias"":""slides"",""value"":""2680""}],""alias"":""slides""},{""properties"":[{""alias"":""captions"",""value"":{""fieldsets"":[{""properties"":[{""alias"":""textstring"",""value"":""test1 test1""}],""alias"":""textstringArray""},{""properties"":[{""alias"":""textstring"",""value"":""test2  test2""}],""alias"":""textstringArray""},{""properties"":[{""alias"":""textstring"",""value"":""test3b   test3b""}],""alias"":""textstringArray""}]}}],""alias"":""captions""}]}";
        private const string _ESCAPED_JSON_DEEP_NESTED = @"{""fieldsets"":[{""properties"":[{""alias"":""pages"",""value"":""""},{""alias"":""captions"",""value"":""{\""fieldsets\"":[{\""properties\"":[{\""alias\"":\""captions\"",\""value\"":\""{\\\""fieldsets\\\"":[{\\\""properties\\\"":[{\\\""alias\\\"":\\\""textString\\\"",\\\""value\\\"":\\\""{\\\\\\\""fieldsets\\\\\\\"":[{\\\\\\\""properties\\\\\\\"":[{\\\\\\\""alias\\\\\\\"":\\\\\\\""textString\\\\\\\"",\\\\\\\""value\\\\\\\"":\\\\\\\""\\\\\\\""}],\\\\\\\""alias\\\\\\\"":\\\\\\\""textItem\\\\\\\""}]}\\\""}],\\\""alias\\\"":\\\""textList\\\""}]}\""}],\""alias\"":\""captions\""}]}""}],""alias"":""pages""}]}";
        private const string _ESCAPED_JSON_FROM_APP = @"{
  ""alias"": ""captions"",
  ""value"": ""{\""fieldsets\"":[{\""properties\"":[{\""alias\"":\""textstring\"",\""value\"":\""test1 test1\""}],\""alias\"":\""textstringArray\""},{\""properties\"":[{\""alias\"":\""textstring\"",\""value\"":\""test2  test2\""}],\""alias\"":\""textstringArray\""},{\""properties\"":[{\""alias\"":\""textstring\"",\""value\"":\""test3b   test3b\""}],\""alias\"":\""textstringArray\""}]}""
}";

        private SlideShow _referenceModel;
        private NestedClass _nestedClass;

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

            _nestedClass = GetNestedClass();
            _nestedClass.SlideShow = _referenceModel;
            _nestedClass.Nested = GetNestedClass();
            _nestedClass.Nested.SlideShow = _referenceModel;
            _nestedClass.Nested.Nested = GetNestedClass();

        }

        [Test]
        public void Delinter_RemoveNewLine_ByPasses_Values()
        {
            const string _JSON_WITH_RESERVED_CHARS = @"{
  ""alias"": ""captions"",
  ""value"": ""{   \""fieldsets\"":    [
{
              \""properties\"":
    [
{          \""alias\"":    \""textstring\""   ,    \""value\"":\""test1 \r\n  test1\r\n\""}],        \""alias\"":\""textstringArray\""    },{\""properties\"":  [   {   \""alias\"":\""textstring\"",\""value\"":\""test2\r\ntest2\""}],\""alias\"":\""textstringArray\""},{\""properties\"":[{\""alias\"":\""textstring\"",\""value\"":\""test3b \""quote\""   \\r\\r\\rtest3\""}],\""alias\"":\""textstringArray\""   }    ]    }""
}";

            var referenceModel = new Captions()
            {
                new TextString() {Text = @"test1 
  test1
"},
                new TextString() {Text = @"test2
test2"},
                new TextString() {Text = @"test3b ""quote""   \r\r\rtest3"}
            };

            var delintedJson = _JSON_WITH_RESERVED_CHARS.DelintArchetypeJson();
            var actualModel = ConvertArchetypeJsonToModel<Captions>(delintedJson);

            var t = JToken.Parse(delintedJson).ToString(Formatting.None);

            Assert.IsInstanceOf<Captions>(actualModel);
            Assert.IsNotNull(actualModel);

            Assert.AreEqual(3, actualModel.Count);

            foreach (var refItem in referenceModel)
            {
                var index = referenceModel.IndexOf(refItem);
                AssertAreEqual(refItem, actualModel.ElementAt(index));
            }
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
        public void EscapedJson_FromApp_SerializesToModel()
        {
            var delintedJson = _ESCAPED_JSON_FROM_APP.DelintArchetypeJson();

            var referenceModel = new Captions()
            {
                new TextString() {Text = "test1 test1"},
                new TextString() {Text = "test2  test2"},
                new TextString() {Text = "test3b   test3b"}
            };

            var actualModel = ConvertArchetypeJsonToModel<Captions>(delintedJson);

            Assert.IsInstanceOf<Captions>(actualModel);
            Assert.AreEqual(3, actualModel.Count);

            foreach (var refItem in referenceModel)
            {
                var index = referenceModel.IndexOf(refItem);
                AssertAreEqual(refItem, actualModel.ElementAt(index));
            }
        }

        [Test]
        public void DeepNestedJson_Escapes_Correctly()
        {
            var propGuid = new Guid();
            var dtd = new DataTypeDefinition(-1, String.Empty) { Id = 0 };

            var dataTypeService = new Mock<IDataTypeService>();
            dataTypeService.Setup(dts => dts.GetDataTypeDefinitionById(Guid.Parse(propGuid.ToString()))).Returns(dtd);
            dataTypeService.Setup(dts => dts.GetPreValuesCollectionByDataTypeId(dtd.Id)).Returns(new PreValueCollection(new Dictionary<string, PreValue>()));

            var propValueEditor = new Mock<ArchetypePropertyEditor.ArchetypePropertyValueEditor>(new PropertyValueEditor());
            propValueEditor.Setup(pe => pe.GetPropertyEditor(dtd)).Returns(new ArchetypePropertyEditor());

            var archetype = ConvertModelToArchetype(_nestedClass);

            foreach (var prop in archetype.Fieldsets.SelectMany(fs => fs.Properties))
            {
                prop.DataTypeGuid = propGuid.ToString();
                if (prop.Value.ToString().Contains("fieldsets"))
                    prop.PropertyEditorAlias = Constants.PropertyEditorAlias;
            }

            var archetypeJson = JsonConvert.SerializeObject(archetype);
            Assert.IsNotNullOrEmpty(archetypeJson);

            EnsureUmbracoContext(dataTypeService);

            var propEditor = new ArchetypePropertyEditor.ArchetypePropertyValueEditor(new PropertyValueEditor());
            var convertedJson = propEditor.ConvertEditorToDb(new ContentPropertyData(JObject.FromObject(archetype),
                new PreValueCollection(new Dictionary<string, PreValue>()), new Dictionary<string, object>()),
                archetypeJson);

            Assert.IsNotNull(convertedJson);

            var delintedJson = convertedJson.ToString().DelintArchetypeJson();
            Assert.IsNotNullOrEmpty(delintedJson);

            var modelfromArchetypeJson = ConvertArchetypeJsonToModel<NestedClass>(archetypeJson);
            var modelfromConvertedJson = ConvertArchetypeJsonToModel<NestedClass>(delintedJson);

            Assert.IsInstanceOf<NestedClass>(modelfromArchetypeJson);
            Assert.IsInstanceOf<NestedClass>(modelfromConvertedJson);

            Assert.AreEqual(3, modelfromArchetypeJson.SlideShow.Captions.Count);
            Assert.AreEqual(3, modelfromConvertedJson.SlideShow.Captions.Count);

            Assert.AreEqual(3, modelfromArchetypeJson.Nested.SlideShow.Captions.Count);
            Assert.AreEqual(3, modelfromConvertedJson.Nested.SlideShow.Captions.Count);

            AssertAreEqual(modelfromArchetypeJson, modelfromConvertedJson);

        }

        #region private methods

        private NestedClass GetNestedClass()
        {
            return new NestedClass();
        }

        #endregion

        #region classes

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

        [AsArchetype("textstringArray")] /* due to hard coded string calues above, must have same archetype alias as list class */
        [JsonConverter(typeof(ArchetypeJsonConverter))]
        public class TextString
        {
            [JsonProperty("textstring")]
            public String Text { get; set; }
        }

        [AsArchetype("nestedClass")]
        [JsonConverter(typeof(ArchetypeJsonConverter))]
        public class NestedClass
        {
            [JsonProperty("nestedClass")]
            public NestedClass Nested { get; set; }
            [JsonProperty("slides")]
            public SlideShow SlideShow { get; set; }
        }

        #endregion
    }
}
