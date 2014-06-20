using System.Collections;
using System.Linq;
using System.Reflection;
using Archetype.Serialization;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;

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

        protected void ComplexModel_Regression_Battery<T>(T model)
        {
            CompoundModel_Regression_Battery(model);
        }

        protected void AssertAreEqual<T>(T model, T result)
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

        protected void EnsureUmbracoContext(Mock<IDataTypeService> dataTypeService)
        {
            var cacheHelper = CacheHelper.CreateDisabledCacheHelper();
            var serviceContext = new ServiceContext(
                Mock.Of<IContentService>(),
                Mock.Of<IMediaService>(),
                Mock.Of<IContentTypeService>(),
                dataTypeService.Object,
                Mock.Of<IFileService>(),
                Mock.Of<ILocalizationService>(),
                Mock.Of<IPackagingService>(),
                Mock.Of<IEntityService>(),
                Mock.Of<IRelationService>(),
                Mock.Of<IMemberGroupService>(),
                Mock.Of<IMemberTypeService>(),
                Mock.Of<IMemberService>(),
                Mock.Of<IUserService>(),
                Mock.Of<ISectionService>(),
                Mock.Of<IApplicationTreeService>(),
                Mock.Of<ITagService>(),
                Mock.Of<INotificationService>()
                );

            ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>()), serviceContext, cacheHelper, true);
        }

        private object GetExpectedValue<T>(T expected, PropertyInfo propInfo)
        {
            return propInfo.GetValue(expected, null);
        }

        private object GetActualValue<T>(T actual, PropertyInfo propInfo)
        {
            var actualProp = actual.GetSerialiazableProperties().SingleOrDefault(pinfo => pinfo.Name.Equals(propInfo.Name));
            return actualProp != null
                ? actualProp.GetValue(actual, null)
                : null;
        }
    }
}
