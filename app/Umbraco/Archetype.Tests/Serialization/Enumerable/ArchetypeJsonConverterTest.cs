using System;
using System.Collections.Generic;
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
        
        [SetUp]
        public void SetUp()
        {
            _feedbacks = new Feedbacks
            {
                new Feedback{Testimonial = "Testimonial 1"},
                new Feedback{Testimonial = "Testimonial 2"},
                new Feedback{Testimonial = "Testimonial 3"}
            };   
        }
        
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
        }

        [Test]
        public void Convert_FeedbackModel_To_Archetype_AndBack()
        {
            var result = ConvertModelToArchetypeAndBack(_feedbacks);
            Assert.NotNull(result);            
        }
    }
}
