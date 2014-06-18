using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Archetype.Tests.Serialization.Regression
{
    [TestFixture]
    public class ArchetypeJsonConverterLegacySetUpTest
    {
        [Test]
        public void Deserialize_SlideShow_FromArchetype()
        {
            var result = JsonConvert.DeserializeObject<SlideShow>(JsonTestStrings._SLIDES_JSON);

            Assert.NotNull(result);
            Assert.IsInstanceOf<SlideShow>(result);

            Assert.AreEqual("1,2,3,4,5", result.Slides);
            Assert.AreEqual("Test 1", result.Captions.TextStringArray.ElementAt(0).Text);
            Assert.AreEqual("Test 2", result.Captions.TextStringArray.ElementAt(1).Text);
            Assert.AreEqual("Test 3", result.Captions.TextStringArray.ElementAt(2).Text);
            Assert.AreEqual("Test 4", result.Captions.TextStringArray.ElementAt(3).Text);
        }
    }
}
