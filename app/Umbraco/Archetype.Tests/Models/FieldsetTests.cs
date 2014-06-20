using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archetype.PropertyConverters;
using NUnit.Framework;
using Archetype.Tests.Serialization;

namespace Archetype.Tests.Models
{

    [TestFixture]
    public class FieldsetTests
    {

        private string _sampleJson;

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
        public void Can_Convert_Property_Default_Value_Types() 
		{
            var converter = new ArchetypeValueConverter();
            var result = (Archetype.Models.ArchetypeModel)converter.ConvertDataToSource(null, _sampleJson, false);

            var fieldset = result.Fieldsets.First();

            Assert.That(fieldset.GetValue<int>("noSuchInteger", 1234) == 1234);
            Assert.That(fieldset.GetValue<bool>("noSuchBoolean", true) == true);
            Assert.That(fieldset.GetValue<bool>("noSuchBoolean", false) == false);
            Assert.That(fieldset.GetValue<string>("noSuchString", "noSuchString default value") == "noSuchString default value");

            var contactDetails = fieldset.GetValue<ContactDetails>("noSuchContact", new ContactDetails { Address = "some street, some city", Name = "someone" });
            Assert.That(contactDetails.Address == "some street, some city");
            Assert.That(contactDetails.Name == "someone");
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
