using System;

namespace Archetype.Tests.Serialization.Regression
{       
    public class SerializationTestHelper
    {
        public T GetModel<T>()
        {
            try
            {
                return (T)GetType().GetMethod("Get" + typeof(T).Name).Invoke(this, null);
            }
            catch
            {
                return default(T);                
            }
        }        
        
        public SimpleModel GetSimpleModel()
        {
            return new SimpleModel
            {
                Amount = 5.67,
                NullableAmount = null,
                DateOne = new DateTime(2000, 1, 1),
                DateTwo = null,
                Id = 123,
                NullableId = null,
                Text = "Test Text"
            };
        }

        public SimpleModelWithFieldsets GetSimpleModelWithFieldsets()
        {
            return new SimpleModelWithFieldsets
            {
                Amount = 5.67,
                NullableAmount = null,
                DateOne = new DateTime(2000, 1, 1),
                DateTwo = null,
                Id = 123,
                NullableId = null,
                Text = "Test Text"
            };
        }

        public SimpleModelWithMixedFieldsets GetSimpleModelWithMixedFieldsets()
        {
            return new SimpleModelWithMixedFieldsets
            {
                Amount = 5.67,
                NullableAmount = null,
                DateOne = new DateTime(2000, 1, 1),
                DateTwo = null,
                Id = 123,
                NullableId = null,
                Text = "Test Text"
            };
        }

        public SimpleModels GetSimpleModels()
        {
            return new SimpleModels
            {
                GetSimpleModel(),
                GetSimpleModel(),
                GetSimpleModel()
            };
        }

        public SimpleModelsWithFieldsets GetSimpleModelsWithFieldsets()
        {
            return new SimpleModelsWithFieldsets
            {
                GetSimpleModelWithFieldsets(),
                GetSimpleModelWithFieldsets(),
                GetSimpleModelWithFieldsets()
            };
        }

        public SimpleModelsWithMixedFieldsets GetSimpleModelsWithMixedFieldsets()
        {
            return new SimpleModelsWithMixedFieldsets
            {
                GetSimpleModelWithMixedFieldsets(),
                GetSimpleModelWithMixedFieldsets(),
                GetSimpleModelWithMixedFieldsets()
            };
        }

        public CompoundModel GetCompoundModel()
        {
            return new CompoundModel
            {
                Id = -1,
                SimpleModel = GetModel<SimpleModel>(),
                Text = typeof(CompoundModel).Name
            };
        }

        public CompoundModelWithMixedFieldsetVariant1 GetCompoundModelWithMixedFieldsetVariant1()
        {
            return new CompoundModelWithMixedFieldsetVariant1
            {
                Id = -1,
                SimpleModel = GetModel<SimpleModel>(),
                Text = typeof(CompoundModelWithMixedFieldsetVariant1).Name
            };
        }

        public CompoundModelWithMixedFieldsetVariant2 GetCompoundModelWithMixedFieldsetVariant2()
        {
            return new CompoundModelWithMixedFieldsetVariant2
            {
                Id = -1,
                SimpleModel = GetModel<SimpleModel>(),
                Text = typeof(CompoundModelWithMixedFieldsetVariant2).Name
            };
        }

        public CompoundModelWithFieldset GetCompoundModelWithFieldset()
        {
            return new CompoundModelWithFieldset
            {
                Id = -1,
                SimpleModel = GetModel<SimpleModel>(),
                Text = typeof(CompoundModelWithFieldset).Name
            };
        }

        public CompoundModelWithList GetCompoundModelWithList()
        {
            return new CompoundModelWithList
            {
                Id = 10,
                SimpleModelList = GetModel<SimpleModels>(),
                Text = typeof(CompoundModelWithList).Name
            };
        }

        public CompoundModelWithMixedFieldsetWithList GetCompoundModelWithMixedFieldsetWithList()
        {
            return new CompoundModelWithMixedFieldsetWithList
            {
                Id = 11,
                SimpleModelList = GetModel<SimpleModels>(),
                Text = typeof(CompoundModelWithMixedFieldsetWithList).Name
            };
        }

        public CompoundModelWithFieldsetWithList GetCompoundModelWithFieldsetWithList()
        {
            return new CompoundModelWithFieldsetWithList
            {
                Id = 12,
                SimpleModelList = GetModel<SimpleModels>(),
                Text = typeof(CompoundModelWithFieldsetWithList).Name
            };
        }

        public NestedModel GetNestedModel()
        {
            return new NestedModel
            {
                CompoundModel = GetModel<CompoundModel>(),
                Id = 53,
                SimpleModel = GetModel<SimpleModel>(),
                Text = typeof(NestedModel).Name
            };
        }

        public NestedModelWithMixedFieldset GetNestedModelWithMixedFieldset()
        {
            return new NestedModelWithMixedFieldset
            {
                CompoundModel = GetModel<CompoundModel>(),
                Id = 53,
                SimpleModel = GetModel<SimpleModel>(),
                Text = typeof(NestedModel).Name
            };
        }

        public NestedModelWithFieldset GetNestedModelWithFieldset()
        {
            return new NestedModelWithFieldset
            {
                CompoundModel = GetModel<CompoundModel>(),
                Id = 53,
                SimpleModel = GetModel<SimpleModel>(),
                Text = typeof(NestedModel).Name
            };
        }
    }
}
