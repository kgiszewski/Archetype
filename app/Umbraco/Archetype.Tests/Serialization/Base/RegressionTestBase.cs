using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archetype.Umbraco.PropertyEditors;
using Archetype.Umbraco.Serialization;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.Persistence;
using Umbraco.Core.PropertyEditors;
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


        protected void NestedModel_WithEscapedJson_Regression_Battery<T>(T model)
        {
            var propGuid = new Guid();
            var cacheHelper = CacheHelper.CreateDisabledCacheHelper();
            var dtd = new DataTypeDefinition(-1, String.Empty) {Id = 0};
            
            var dataTypeService = new Mock<IDataTypeService>();
            dataTypeService.Setup(dts => dts.GetDataTypeDefinitionById(Guid.Parse(propGuid.ToString()))).Returns(dtd);
            dataTypeService.Setup(dts => dts.GetPreValuesCollectionByDataTypeId(dtd.Id)).Returns(new PreValueCollection(new Dictionary<string, PreValue>()));

            var propValueEditor = new Mock<ArchetypePropertyEditor.ArchetypePropertyValueEditor>(new PropertyValueEditor());
            propValueEditor.Setup(pe => pe.GetPropertyEditor(dtd)).Returns(new ArchetypePropertyEditor());

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
                Mock.Of<ISectionService>(), 
                Mock.Of<IApplicationTreeService>(), 
                Mock.Of<ITagService>()
                );

            ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>()), serviceContext, cacheHelper, true);

            var archetype = ConvertModelToArchetype(model);

            foreach (var prop in archetype.Fieldsets.SelectMany(fs => fs.Properties))
            {
                prop.DataTypeGuid = propGuid.ToString();
                if (prop.Value.ToString().Contains("fieldsets"))
                    prop.PropertyEditorAlias = Umbraco.Constants.PropertyEditorAlias;
            }

            var archetypeJson = JsonConvert.SerializeObject(archetype);
            Assert.IsNotNullOrEmpty(archetypeJson);
            
            var propEditor = new ArchetypePropertyEditor.ArchetypePropertyValueEditor(new PropertyValueEditor());

            var convertedJson = (string)propEditor.ConvertEditorToDb(new ContentPropertyData(archetypeJson, 
                new PreValueCollection(new Dictionary<string, PreValue>()), new Dictionary<string, object>()), 
                model);

            Assert.IsNotNullOrEmpty(convertedJson);

            var resultfromArchetypeJson = ConvertArchetypeJsonToModel<T>(archetypeJson);
            var resultfromConvertedJson = ConvertArchetypeJsonToModel<T>(convertedJson);

            Assert.IsInstanceOf<T>(resultfromArchetypeJson);
            AssertAreEqual(model, resultfromArchetypeJson);

            Assert.IsInstanceOf<T>(resultfromConvertedJson);
            AssertAreEqual(model, resultfromConvertedJson);
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
