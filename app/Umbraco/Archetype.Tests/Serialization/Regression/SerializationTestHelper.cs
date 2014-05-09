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
    }
}
