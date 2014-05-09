using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            foreach (var propInfo in model.GetSerialiazableProperties())
            {
                Assert.AreEqual(propInfo.GetValue(model, null), 
                    result.GetSerialiazableProperties().Single(pinfo => pinfo.Name.Equals(propInfo.Name))
                        .GetValue(result, null));
            }
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

                foreach (var propInfo in model[index].GetSerialiazableProperties())
                {
                    Assert.AreEqual(propInfo.GetValue(model[index], null),
                        resultItem.GetSerialiazableProperties().Single(pinfo => pinfo.Name.Equals(propInfo.Name))
                            .GetValue(resultItem, null));
                }
            }
        }
    }
}
