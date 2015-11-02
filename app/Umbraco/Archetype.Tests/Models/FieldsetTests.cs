using System.IO;
using System.Linq;
using Archetype.PropertyConverters;
using NUnit.Framework;

namespace Archetype.Tests.Models
{
    /// <summary>
    /// Tests designed for the fieldsets.
    /// </summary>
    [TestFixture]
    public class FieldsetTests
    {
        private string _sampleJson;

        /// <summary>
        /// Sets up the test to use sample JSON data.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _sampleJson = File.ReadAllText("..\\..\\Data\\sample-1.json");
        }

        [Test]
        public void Can_Get_Fieldset_Property_By_Alias()
        {
            var converter = new ArchetypeValueConverter();
            var result = (Archetype.Models.ArchetypeModel)converter.ConvertDataToSource(null, _sampleJson, false);

            var fieldset = result.Fieldsets.First();
            var propertyValue = fieldset.GetValue("boxHeadline");

            Assert.That(propertyValue == "Box 1 Title");
        }

        [Test]
        public void Can_Get_Fieldset_Property_Default_Value_By_Alias() 
		{
            var converter = new ArchetypeValueConverter();
            var result = (Archetype.Models.ArchetypeModel)converter.ConvertDataToSource(null, _sampleJson, false);

            var fieldset = result.Fieldsets.First();
            var propertyValue = fieldset.GetValue<string>("noSuchProperty", "noSuchProperty default value");

            Assert.That(propertyValue == "noSuchProperty default value");
        }

        [Test]
        public void Can_Convert_Property_Value_Types()
        {
            var converter = new ArchetypeValueConverter();
            var result = (Archetype.Models.ArchetypeModel)converter.ConvertDataToSource(null, _sampleJson, false);

            var fieldset = result.Fieldsets.First();

            Assert.That(fieldset.GetValue<int>("link") == 3175);
            Assert.That(fieldset.GetValue<bool>("show") == true);
            Assert.That(fieldset.GetValue<string>("blurb") == "A blurb here");
        }  

        [Test]
        public void Returns_String_When_No_Type_Specified()
        {
            var converter = new ArchetypeValueConverter();
            var result = (Archetype.Models.ArchetypeModel)converter.ConvertDataToSource(null, _sampleJson, false);

            var fieldset = result.Fieldsets.First();

            var property = fieldset.GetValue("link");
            Assert.That(property is string);
            Assert.That(property == "3175");
        }
    }
}
