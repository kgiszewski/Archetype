using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archetype.Umbraco.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Archetype.Tests.Serialization.Base
{
    public abstract class RegressionTestBase : ArchetypeJsonConverterTestBase
    {
        protected void SimpleModel_Regression_Battery<T>(T model)
        {
            var archetype = ConvertModelToArchetype(model);
            Assert.IsNotNull(archetype);

            var archetypeJson = archetype.SerializeForPersistence();
            var modelJson = ConvertModelToArchetypeJson(model, Formatting.Indented);

            Assert.IsNotNullOrEmpty(modelJson);

            var result = ConvertModelToArchetypeAndBack(model);
            var resultfromArchetypeJson = ConvertArchetypeJsonToModel<T>(archetypeJson);

            Assert.IsInstanceOf<T>(result);
            Assert.IsInstanceOf<T>(resultfromArchetypeJson);
            AssertAreEqual(model, result);
            AssertAreEqual(model, resultfromArchetypeJson);
        }

        protected void SimpleModels_Regression_Battery<T>(T model)
            where T : IList
        {
            var archetype = ConvertModelToArchetype(model);
            Assert.IsNotNull(archetype);

            var archetypeJson = archetype.SerializeForPersistence();
            var modelJson = ConvertModelToArchetypeJson(model, Formatting.Indented);

            Assert.IsNotNullOrEmpty(modelJson);

            var result = ConvertModelToArchetypeAndBack(model);
            var resultfromArchetypeJson = ConvertArchetypeJsonToModel<T>(archetypeJson);

            Assert.IsInstanceOf<T>(result);
            Assert.IsInstanceOf<T>(resultfromArchetypeJson);

            Assert.AreEqual(model.Count, result.Count);
            Assert.AreEqual(model.Count, resultfromArchetypeJson.Count);

            foreach (var resultItem in result)
            {
                var index = result.IndexOf(resultItem);
                AssertAreEqual(model[index], resultItem);
            }

            foreach (var resultItem in resultfromArchetypeJson)
            {
                var index = resultfromArchetypeJson.IndexOf(resultItem);
                AssertAreEqual(model[index], resultItem);
            }
        }

        protected void CompoundModel_Regression_Battery<T>(T model)
        {
            var archetype = ConvertModelToArchetype(model);
            Assert.IsNotNull(archetype);

            var archetypeJson = archetype.SerializeForPersistence();
            var modelJson = ConvertModelToArchetypeJson(model, Formatting.Indented);

            Assert.IsNotNullOrEmpty(modelJson);

            var result = ConvertModelToArchetypeAndBack(model);
            var resultfromArchetypeJson = ConvertArchetypeJsonToModel<T>(archetypeJson);

            Assert.IsInstanceOf<T>(result);
            AssertAreEqual(model, result);

            Assert.IsInstanceOf<T>(resultfromArchetypeJson);
            AssertAreEqual(model, resultfromArchetypeJson);
        }

        protected void NestedModel_Regression_Battery<T>(T model)
        {
            CompoundModel_Regression_Battery(model);
        }


        protected void NestedModel_WithEscapedJson_Regression_Battery<T>(T model)
        {
            var archetype = ConvertModelToArchetype(model);
            Assert.IsNotNull(ConvertModelToArchetype(model));

            var nestedJson = archetype.SerializeForPersistence();
        }

        protected void ComplexModel_Regression_Battery<T>(T model)
        {
            CompoundModel_Regression_Battery(model);
        }

        private static void AssertAreEqual<T>(T model, T result)
        {
            foreach (var propInfo in model.GetSerialiazableProperties())
            {
                var expected = GetExpectedValue(model, propInfo);
                var actual = GetActualValue(result, propInfo);
                
                if (propInfo.PropertyType.Namespace.Equals("System"))
                {
                    Assert.AreEqual(expected, actual);                 
                    continue;
                }

                var list = expected as IList;
                if (list != null)
                {
                    foreach (var expectedItem in list)
                    {
                        var index = list.IndexOf(expectedItem);
                        AssertAreEqual(expectedItem, ((IList)actual)[index]);
                    }
                    continue;
                }

                AssertAreEqual(expected, actual);
            }
        }

        private static object GetExpectedValue<T>(T expected, PropertyInfo propInfo)
        {
            return propInfo.GetValue(expected, null);
        }

        private static object GetActualValue<T>(T actual, PropertyInfo propInfo)
        {
            return actual.GetSerialiazableProperties().Single(pinfo => pinfo.Name.Equals(propInfo.Name))
                .GetValue(actual, null);
        }
    }
}
