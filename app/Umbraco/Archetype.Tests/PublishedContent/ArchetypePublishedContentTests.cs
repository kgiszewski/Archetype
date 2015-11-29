using System;
using System.Linq;
using Archetype.Extensions;
using Archetype.Models;
using Archetype.PropertyConverters;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Archetype.Tests.PublishedContent
{
    [TestFixture]
    public class ArchetypePublishedContentTests
    {
        private ArchetypeModel _archetype;

        [TestFixtureSetUp]
        public void Init()
        {
            var archetypeJson = System.IO.File.ReadAllText("..\\..\\Data\\sample-1.json");
            var converter = new ArchetypeValueConverter();

            _archetype = (ArchetypeModel)converter.ConvertDataToSource(null, archetypeJson, false);
        }

        [Test]
        public void ArchetypeModel_Initialized()
        {
            Assert.IsNotNull(_archetype);
            Assert.IsNotEmpty(_archetype.Fieldsets);
        }

        [Test]
        public void ArchetypeModel_To_PublishedContentSet()
        {
            var contentSet = _archetype.ToPublishedContentSet();

            Assert.That(contentSet, Is.Not.Null);
            Assert.That(contentSet, Is.Not.Empty);
            Assert.That(contentSet, Is.All.InstanceOf<IPublishedContent>());
        }

        [Test]
        public void ArchetypeFieldsetModel_To_PublishedContent()
        {
            var fieldset = _archetype.Fieldsets.FirstOrDefault();
            var content = fieldset.ToPublishedContent();

            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.InstanceOf<IPublishedContent>());
            Assert.That(content.Properties, Is.Not.Empty);
            Assert.That(content.Properties, Is.All.InstanceOf<IPublishedProperty>());
        }

        [TestCase("boxHeadline", "Box 1 Title")]
        [TestCase("link", "3175")]
        [TestCase("link", 3175)]
        [TestCase("show", "1")]
        [TestCase("show", true)]
        [TestCase("show", 1)]
        public void ArchetypePublishedContent_Typed_Properties<T>(string propertyAlias, T expected)
        {
            var fieldset = _archetype.Fieldsets.FirstOrDefault();
            var content = fieldset.ToPublishedContent();

            var actual = content.GetPropertyValue<T>(propertyAlias);

            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void Null_ArchetypeModel_Throws_Exception()
        {
            TestDelegate code = () =>
            {
                ArchetypeModel archetype = null;
                archetype.ToPublishedContentSet();
            };

            Assert.Throws<ArgumentNullException>(code);
        }

        [Test]
        public void Null_ArchetypeFieldsetModel_Throws_Exception()
        {
            TestDelegate code = () =>
            {
                ArchetypeFieldsetModel fieldset = null;
                fieldset.ToPublishedContent();
            };

            Assert.Throws<ArgumentNullException>(code);
        }
    }
}