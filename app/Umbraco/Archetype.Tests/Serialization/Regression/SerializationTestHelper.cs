using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archetype.Tests.Serialization.Regression
{
    public class SerializationTestHelper
    {
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

        public SimpleModelWithFieldsets GetSimpleModellWithFieldsets()
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

        public SimpleModelWithMixedFieldsets GetSimpleModellWithMixedFieldsets()
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
    }
}
