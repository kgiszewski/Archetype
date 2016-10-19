using System;
using System.Collections.Generic;
using System.Linq;
using Archetype.Models;
using Archetype.PropertyConverters;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Archetype.Tests.PublishedContent
{
    [TestFixture]
    public class TypeConverterTests
    {
        private ArchetypeModel _archetype;

        [TestFixtureSetUp]
        public void Init()
        {
            var archetypeJson = System.IO.File.ReadAllText("..\\..\\Data\\sample-1.json");
            var converter = new ArchetypeValueConverter();

            _archetype = (ArchetypeModel)converter.ConvertDataToSource(null, archetypeJson, false);
        }

        [TestCase(typeof(ArchetypeModel), true)]
        [TestCase(typeof(ArchetypePublishedContentSet), true)]
        [TestCase(typeof(IEnumerable<IPublishedContent>), true)]
        [TestCase(typeof(string), true)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(ArchetypeFieldsetModel), false)]
        public void ArchetypeModel_ConvertsTo_EnumerablePublishedContent(Type destinationType, bool expected)
        {
            Assert.IsNotNull(_archetype);

            var attempt = _archetype.TryConvertTo(destinationType);

            Assert.That(attempt.Success, Is.EqualTo(expected));

            if (attempt.Success)
            {
                Assert.IsNotNull(attempt.Result);
                Assert.That(attempt.Result, Is.InstanceOf(destinationType));
            }
        }

        [TestCase(typeof(ArchetypeFieldsetModel), true)]
        [TestCase(typeof(ArchetypePublishedContent), true)]
        [TestCase(typeof(IPublishedContent), true)]
        [TestCase(typeof(string), true)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(ArchetypeModel), false)]
        public void ArchetypeFieldsetModel_ConvertsTo_PublishedContent(Type destinationType, bool expected)
        {
            CollectionAssert.IsNotEmpty(_archetype.Fieldsets);

            var attempt = _archetype.First().TryConvertTo(destinationType);

            Assert.That(attempt.Success, Is.EqualTo(expected));

            if (attempt.Success)
            {
                Assert.IsNotNull(attempt.Result);
                Assert.That(attempt.Result, Is.InstanceOf(destinationType));
            }
        }
    }
}