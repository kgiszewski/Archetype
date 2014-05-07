using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archetype.Tests.Serialization.Base;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Archetype.Tests.Serialization.Regression
{
    [TestFixture]
    public class ArchetypeJsonConverterTest : ArchetypeJsonConverterTestBase
    {
        private SerializationTestHelper _testHelper;

        [SetUp]
        public void SetUp()
        {
            _testHelper = new SerializationTestHelper();
        }
        
        [Test]
        public void SimpleModel_Regression_Battery()
        {
            var simpleModel = _testHelper.GetSimpleModel();
            Assert.IsNotNull(ConvertModelToArchetype(simpleModel));

            var json = ConvertModelToArchetypeJson(simpleModel, Formatting.Indented);
            Assert.IsNotNullOrEmpty(json);
            Assert.AreEqual(JsonTestStrings._SIMPLE_JSON, json);

            var result = ConvertModelToArchetypeAndBack(simpleModel);

            Assert.IsInstanceOf<SimpleModel>(result);
            Assert.AreEqual(simpleModel.DateOne, result.DateOne);
            Assert.AreEqual(simpleModel.DateTwo, result.DateTwo);
            Assert.AreEqual(simpleModel.Id, result.Id);
            Assert.AreEqual(simpleModel.NullableId, result.NullableId);
            Assert.AreEqual(simpleModel.Text, result.Text);
            Assert.AreEqual(simpleModel.Amount, result.Amount);
            Assert.AreEqual(simpleModel.NullableAmount, result.NullableAmount);
        }

        [Test]
        public void SimpleModelWithFieldsets_Regression_Battery()
        {
            var simpleModel = _testHelper.GetSimpleModellWithFieldsets();
            Assert.IsNotNull(ConvertModelToArchetype(simpleModel));

            var json = ConvertModelToArchetypeJson(simpleModel, Formatting.Indented);
            Assert.IsNotNullOrEmpty(json);
            //Assert.AreEqual(JsonTestStrings._SIMPLE_JSON, json);

            var result = ConvertModelToArchetypeAndBack(simpleModel);

            Assert.IsInstanceOf<SimpleModelWithFieldsets>(result);
            Assert.AreEqual(simpleModel.DateOne, result.DateOne);
            Assert.AreEqual(simpleModel.DateTwo, result.DateTwo);
            Assert.AreEqual(simpleModel.Id, result.Id);
            Assert.AreEqual(simpleModel.NullableId, result.NullableId);
            Assert.AreEqual(simpleModel.Text, result.Text);
            Assert.AreEqual(simpleModel.Amount, result.Amount);
            Assert.AreEqual(simpleModel.NullableAmount, result.NullableAmount);
        }

        [Test]
        public void SimpleModelWithMixedFieldsets_Regression_Battery()
        {
            var simpleModel = _testHelper.GetSimpleModellWithMixedFieldsets();
            Assert.IsNotNull(ConvertModelToArchetype(simpleModel));

            var json = ConvertModelToArchetypeJson(simpleModel, Formatting.Indented);
            Assert.IsNotNullOrEmpty(json);
            //Assert.AreEqual(JsonTestStrings._SIMPLE_JSON, json);

            var result = ConvertModelToArchetypeAndBack(simpleModel);

            Assert.IsInstanceOf<SimpleModelWithMixedFieldsets>(result);
            Assert.AreEqual(simpleModel.DateOne, result.DateOne);
            Assert.AreEqual(simpleModel.DateTwo, result.DateTwo);
            Assert.AreEqual(simpleModel.Id, result.Id);
            Assert.AreEqual(simpleModel.NullableId, result.NullableId);
            Assert.AreEqual(simpleModel.Text, result.Text);
            Assert.AreEqual(simpleModel.Amount, result.Amount);
            Assert.AreEqual(simpleModel.NullableAmount, result.NullableAmount);
        }

    }
}
