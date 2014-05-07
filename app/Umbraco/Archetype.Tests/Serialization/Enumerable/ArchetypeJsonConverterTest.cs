using System;
using System.Collections.Generic;
using System.Linq;
using Archetype.Tests.Serialization.Base;
using Archetype.Umbraco.PropertyConverters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Archetype.Tests.Serialization.Enumerable
{
    [TestFixture]
    public class ArchetypeJsonConverterTest : ArchetypeJsonConverterTestBase
    {
        private Feedbacks _feedbacks;
        private Captions _captions;

        
        [SetUp]
        public void SetUp()
        {
            _feedbacks = new Feedbacks
            {
                new Feedback{Testimonial = "Testimonial 1"},
                new Feedback{Testimonial = "Testimonial 2"},
                new Feedback{Testimonial = "Testimonial 3"}
            };

            _captions = new Captions()
            {
                TextArray = new List<Text>
                {
                    new Text{TextString = "Caption 1"},
                    new Text{TextString = "Caption 2"},
                    new Text{TextString = "Caption 3"}
                }
            };
        }

        #region Feedback - single fieldset with multiple items

        [Test]
        public void Convert_FeedbackModel_To_Archetype()
        {
            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(_feedbacks);
            var archetype = (Umbraco.Models.Archetype)converter.ConvertDataToSource(null, json, false);

            Assert.NotNull(archetype);
        }

        [Test]
        public void Convert_FeedbackModel_To_ArchetypeJson()
        {
            var json = ConvertModelToArchetypeJson(_feedbacks, Formatting.Indented);
            Assert.AreEqual(JsonTestStrings._FEEDBACK_JSON, json);
        }

        [Test]
        public void Convert_ArchetypeJson_To_FeedbackModel()
        {
            var result = ConvertArchetypeJsonToModel<Feedbacks>(JsonTestStrings._FEEDBACK_JSON);
            Assert.NotNull(result);
            Assert.AreEqual(3, result.Count);

            foreach (var feedback in result)
            {
                Assert.AreEqual(String.Format("Testimonial {0}", result.IndexOf(feedback) + 1),
                    feedback.Testimonial);
            }
        }

        [Test]
        public void Convert_FeedbackModel_To_Archetype_AndBack()
        {
            var result = ConvertModelToArchetypeAndBack(_feedbacks);
            Assert.NotNull(result);

            Assert.AreEqual(3, result.Count);

            foreach (var feedback in result)
            {
                var index = result.IndexOf(feedback);
                Assert.AreEqual(_feedbacks.ElementAt(index).Testimonial, feedback.Testimonial);
            }
        }

        #endregion

        #region Captions - a root fieldset which contains another fieldset list

        [Test]
        public void Convert_CaptionsModel_To_Archetype()
        {
            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(_captions);
            var archetype = (Umbraco.Models.Archetype)converter.ConvertDataToSource(null, json, false);

            Assert.NotNull(archetype);
        }

        [Test]
        public void Convert_CaptionsModel_To_ArchetypeJson()
        {
            var json = ConvertModelToArchetypeJson(_captions, Formatting.Indented);
            Assert.AreEqual(JsonTestStrings._CAPTIONS_JSON, json);
        }

        [Test]
        public void Convert_ArchetypeJson_To_CaptionsModel()
        {
            var result = ConvertArchetypeJsonToModel<Captions>(JsonTestStrings._CAPTIONS_JSON);
            Assert.NotNull(result);
            Assert.NotNull(result.TextArray);

            Assert.AreEqual(3, result.TextArray.Count);

            foreach (var caption in result.TextArray)
            {
                Assert.AreEqual(String.Format("Caption {0}", result.TextArray.IndexOf(caption) + 1),
                    caption.TextString);
            }
        }

        [Test]
        public void Convert_CaptionsModel_To_Archetype_AndBack()
        {
            var result = ConvertModelToArchetypeAndBack(_captions);
            Assert.NotNull(result);
            Assert.NotNull(result.TextArray);

            Assert.AreEqual(3, result.TextArray.Count);

            foreach (var caption in result.TextArray)
            {
                var index = result.TextArray.IndexOf(caption);
                Assert.AreEqual(_captions.TextArray.ElementAt(index).TextString, caption.TextString);
            }
        }

        #endregion
    }
}
