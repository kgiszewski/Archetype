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
    public class ArchetypeJsonConverterTest : RegressionTestBase
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
            var simpleModel = _testHelper.GetModel<SimpleModel>();
            SimpleModel_Regression_Battery(simpleModel);

            var json = ConvertModelToArchetypeJson(simpleModel, Formatting.Indented);
            Assert.AreEqual(JsonTestStrings._SIMPLE_JSON, json);
        }

        [Test]
        public void SimpleModelWithFieldsets_Regression_Battery()
        {
            var simpleModel = _testHelper.GetModel<SimpleModelWithFieldsets>();
            SimpleModel_Regression_Battery(simpleModel);
        }

        [Test]
        public void SimpleModelWithMixedFieldsets_Regression_Battery()
        {
            var simpleModel = _testHelper.GetModel<SimpleModelWithMixedFieldsets>();
            SimpleModel_Regression_Battery(simpleModel);
        }

        [Test]
        public void SimpleModels_Regression_Battery()
        {
            var simpleModels = _testHelper.GetModel<SimpleModels>();
            SimpleModels_Regression_Battery(simpleModels);
        }



    }
}
