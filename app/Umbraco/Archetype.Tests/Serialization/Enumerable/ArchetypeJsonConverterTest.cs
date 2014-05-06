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
        private Feedback _feedback;
        
        [SetUp]
        public void SetUp()
        {
            _feedback = new Feedback
            {
                Testimonials = new List<String>
                {
                    "Testimonial 1",
                    "Testimonial 2",
                    "Testimonial 3"
                }
            };   
        }
        
        [Test]
        public void Convert_FeedbackModel_To_Archetype()
        {
            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(_feedback);
            var archetype = (Umbraco.Models.Archetype)converter.ConvertDataToSource(null, json, false);

            Assert.NotNull(archetype);
        }

        [Test]
        public void Convert_FeedbackModel_To_ArchetypeJson()
        {
            var json = ConvertModelToArchetype(_feedback);
            Assert.AreEqual(JsonTestStrings._FEEDBACK_JSON, json);
        }

        [Test]
        public void Convert_ArchetypeJson_To_FeedbackModel()
        {
            var result = ConvertArchetypeJsonToModel<Feedback>(JsonTestStrings._FEEDBACK_JSON);
            Assert.NotNull(result);
        }

        [Test]
        public void Convert_FeedbackModel_To_Archetype_AndBack()
        {
            var result = ConvertModelToArchetypeAndBack(_feedback);
            Assert.NotNull(result);            
        }
    }
}
