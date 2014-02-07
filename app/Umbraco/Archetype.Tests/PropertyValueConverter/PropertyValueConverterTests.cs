using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archetype.Umbraco.PropertyConverters;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Models.PublishedContent;

namespace Archetype.Tests.PropertyValueConverter
{
    [TestFixture]
    public class PropertyValueConverterTests
    {

        private string _sampleJson;

        [SetUp]
        public void SetUp()
        {
            _sampleJson = File.ReadAllText("..\\..\\Data\\sample-1.json");
        }

        [Test]
        public void Returns_Null_When_Data_Is_Null()
        {
            var converter = new ArchetypeValueConverter();
            var result = converter.ConvertDataToSource(null, null, false);

            Assert.IsNull(result);
        }

        [Test]
        public void Returns_Empty_String_When_Data_Is_Empty_String()
        {
            var data = "";
            var converter = new ArchetypeValueConverter();
            var result = converter.ConvertDataToSource(null, data, false);

            Assert.AreEqual("", result);
        }

        [Test]
        public void Returns_Original_String_When_Data_Is_Invalid_Json()
        {
            var data = "{ invalid";
            var converter = new ArchetypeValueConverter();
            var result = converter.ConvertDataToSource(null, data, false);
            
            Assert.AreEqual(data, result);
        }

        [Test]
        public void Can_Serialize_From_Json_Model()
        {
            var converter = new ArchetypeValueConverter();
            var result = (Archetype.Umbraco.Models.Archetype)converter.ConvertDataToSource(null, _sampleJson, false);

            Assert.That(result != null);
            Assert.That(result.Fieldsets.Count() == 2);
        }
    }
}
