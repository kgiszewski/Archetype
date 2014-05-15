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

        [Test]
        public void SimpleModelsWithFieldsets_Regression_Battery()
        {
            var simpleModels = _testHelper.GetModel<SimpleModelsWithFieldsets>();
            SimpleModels_Regression_Battery(simpleModels);
        }

        [Test]
        public void SimpleModelsWithMixedFieldsets_Regression_Battery()
        {
            var simpleModels = _testHelper.GetModel<SimpleModelsWithMixedFieldsets>();
            SimpleModels_Regression_Battery(simpleModels);
        }

        [Test]
        public void CompoundModel_Regression_Battery()
        {
            var compoundModel = _testHelper.GetModel<CompoundModel>();
            CompoundModel_Regression_Battery(compoundModel);
        }

        [Test]
        public void CompoundModel_WithEscapedJson_Regression_Battery()
        {
            var compoundModel = _testHelper.GetModel<CompoundModel>();
            CompoundModel_WithEscapedJson_Regression_Battery(compoundModel);
        }

        [Test]
        public void CompoundModelWithMixedFieldsetVariant1_Regression_Battery()
        {
            var compoundModel = _testHelper.GetModel<CompoundModelWithMixedFieldsetVariant1>();
            CompoundModel_Regression_Battery(compoundModel);
        }

        [Test]
        public void CompoundModelWithMixedFieldsetVariant1_WithEscapedJson_Regression_Battery()
        {
            var compoundModel = _testHelper.GetModel<CompoundModelWithMixedFieldsetVariant1>();
            CompoundModel_WithEscapedJson_Regression_Battery(compoundModel);
        }

        [Test]
        public void CompoundModelWithMixedFieldsetVariant2_Regression_Battery()
        {
            var compoundModel = _testHelper.GetModel<CompoundModelWithMixedFieldsetVariant2>();
            CompoundModel_Regression_Battery(compoundModel);
        }

        [Test]
        public void CompoundModelWithMixedFieldsetVariant2_WithEscapedJson_Regression_Battery()
        {
            var compoundModel = _testHelper.GetModel<CompoundModelWithMixedFieldsetVariant2>();
            CompoundModel_WithEscapedJson_Regression_Battery(compoundModel);
        }

        [Test]
        public void CompoundModelWithFieldset_Regression_Battery()
        {
            var compoundModel = _testHelper.GetModel<CompoundModelWithFieldset>();
            CompoundModel_Regression_Battery(compoundModel);
        }

        [Test]
        public void CompoundModelWithFieldset_WithEscapedJson_Regression_Battery()
        {
            var compoundModel = _testHelper.GetModel<CompoundModelWithFieldset>();
            CompoundModel_WithEscapedJson_Regression_Battery(compoundModel);
        }

        [Test]
        public void CompoundModelWithList_Regression_Battery()
        {
            var compoundModelWithList = _testHelper.GetModel<CompoundModelWithList>();
            CompoundModel_Regression_Battery(compoundModelWithList);
        }

        [Test]
        public void CompoundModelWithList_WithEscapedJson_Regression_Battery()
        {
            var compoundModel = _testHelper.GetModel<CompoundModelWithList>();
            CompoundModel_WithEscapedJson_Regression_Battery(compoundModel);
        }

        [Test]
        public void CompoundModelWithMixedFieldsetWithList_Regression_Battery()
        {
            var compoundModelWithList = _testHelper.GetModel<CompoundModelWithMixedFieldsetWithList>();
            CompoundModel_Regression_Battery(compoundModelWithList);
        }

        [Test]
        public void CompoundModelWithMixedFieldsetWithList_WithEscapedJson_Regression_Battery()
        {
            var compoundModel = _testHelper.GetModel<CompoundModelWithMixedFieldsetWithList>();
            CompoundModel_WithEscapedJson_Regression_Battery(compoundModel);
        }

        [Test]
        public void CompoundModelWithFieldsetWithList_Regression_Battery()
        {
            var compoundModelWithList = _testHelper.GetModel<CompoundModelWithFieldsetWithList>();
            CompoundModel_Regression_Battery(compoundModelWithList);
        }

        [Test]
        public void CompoundModelWithFieldsetWithList_WithEscapedJson_Regression_Battery()
        {
            var compoundModel = _testHelper.GetModel<CompoundModelWithFieldsetWithList>();
            CompoundModel_WithEscapedJson_Regression_Battery(compoundModel);
        }

        [Test]
        public void NestedModel_Regression_Battery()
        {
            var nestedModel = _testHelper.GetModel<NestedModel>();
            NestedModel_Regression_Battery(nestedModel);
        }

        [Test]
        public void NestedModel_WithEscapedJson_Regression_Battery()
        {
            var nestedModel = _testHelper.GetModel<NestedModel>();
            NestedModel_WithEscapedJson_Regression_Battery(nestedModel);
        }

        [Test]
        public void NestedModelWithMixedFieldset_Regression_Battery()
        {
            var nestedModel = _testHelper.GetModel<NestedModelWithMixedFieldset>();
            NestedModel_Regression_Battery(nestedModel);
        }

        [Test]
        public void NestedModelWithMixedFieldset_WithEscapedJson_Regression_Battery()
        {
            var nestedModel = _testHelper.GetModel<NestedModelWithMixedFieldset>();
            NestedModel_WithEscapedJson_Regression_Battery(nestedModel);
        }

        [Test]
        public void NestedModelWithFieldset_Regression_Battery()
        {
            var nestedModel = _testHelper.GetModel<NestedModelWithFieldset>();
            NestedModel_Regression_Battery(nestedModel);
        }

        [Test]
        public void NestedModelWithFieldset_WithEscapedJson_Regression_Battery()
        {
            var nestedModel = _testHelper.GetModel<NestedModelWithFieldset>();
            NestedModel_WithEscapedJson_Regression_Battery(nestedModel);            
        }

        [Test]
        public void ComplexModel_Regression_Battery()
        {
            var complexModel = _testHelper.GetModel<ComplexModel>();
            NestedModel_Regression_Battery(complexModel);
        }

    }
}
