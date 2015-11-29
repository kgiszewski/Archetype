using System.IO;
using System.Linq;
using Archetype.PropertyConverters;
using NUnit.Framework;

namespace Archetype.Tests.PropertyValueConverter
{
    /// <summary>
    /// Tests designed to test the PVC.
    /// </summary>
    [TestFixture]
    public class PropertyValueConverterTests
    {
        private string _sampleJson;

        /// <summary>
        /// Sets up the test to use JSON data.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _sampleJson = File.ReadAllText("..\\..\\Data\\sample-1.json");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Returns_Empty_Archetype_When_Data_Is_Null_Or_Empty(object data)
        {
            var converter = new ArchetypeValueConverter();
            var result = converter.ConvertDataToSource(null, data, false);

            Assert.AreEqual(result.GetType(), typeof (Archetype.Models.ArchetypeModel));

            var fieldsets = (Archetype.Models.ArchetypeModel) result;
            Assert.IsTrue(fieldsets.Count() == 0);
        }

        [Test]
        public void Returns_Empty_Archetype_When_Data_Is_Invalid_Json()
        {
            var data = "{ invalid";
            var converter = new ArchetypeValueConverter();
            var result = converter.ConvertDataToSource(null, data, false);
            
            Assert.AreEqual(result.GetType(), typeof (Archetype.Models.ArchetypeModel));

            var fieldsets = (Archetype.Models.ArchetypeModel) result;
            Assert.IsTrue(fieldsets.Count() == 0);
        }

        [Test]
        public void Can_Serialize_From_Json_Model()
        {
            var converter = new ArchetypeValueConverter();
            var result = (Archetype.Models.ArchetypeModel)converter.ConvertDataToSource(null, _sampleJson, false);

            Assert.That(result != null);
            Assert.That(result.Fieldsets.Count() == 2);
        }
    }
}
