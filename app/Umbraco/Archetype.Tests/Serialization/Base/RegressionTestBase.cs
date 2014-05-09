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
            Assert.IsNotNull(ConvertModelToArchetype(model));

            var json = ConvertModelToArchetypeJson(model, Formatting.Indented);
            Assert.IsNotNullOrEmpty(json);

            var result = ConvertModelToArchetypeAndBack(model);

            Assert.IsInstanceOf<T>(result);
            AssertAreEqual(model, result);
        }

        protected void SimpleModels_Regression_Battery<T>(T model)
            where T : IList
        {
            Assert.IsNotNull(ConvertModelToArchetype(model));

            var json = ConvertModelToArchetypeJson(model, Formatting.Indented);
            Assert.IsNotNullOrEmpty(json);

            var result = ConvertModelToArchetypeAndBack(model);

            Assert.IsInstanceOf<T>(result);
            Assert.AreEqual(model.Count, result.Count);

            foreach (var resultItem in result)
            {
                var index = result.IndexOf(resultItem);
                AssertAreEqual(model[index], resultItem);
            }
        }

        protected void CompoundModel_Regression_Battery<T>(T model)
        {
            Assert.IsNotNull(ConvertModelToArchetype(model));

            var json = ConvertModelToArchetypeJson(model, Formatting.Indented);
            Assert.IsNotNullOrEmpty(json);

            var result = ConvertModelToArchetypeAndBack(model);

            Assert.IsInstanceOf<T>(result);
            AssertAreEqual(model, result);
        }

        private static void AssertAreEqual<T>(T model, T result)
        {
            foreach (var propInfo in model.GetSerialiazableProperties())
            {
                if (!propInfo.PropertyType.Namespace.Equals("System"))
                {
                    AssertAreEqual(GetExpectedValue(model, propInfo),
                        GetActualValue(result, propInfo));
                    
                    continue;
                }

                Assert.AreEqual(GetExpectedValue(model, propInfo),
                    GetActualValue(result, propInfo));
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
