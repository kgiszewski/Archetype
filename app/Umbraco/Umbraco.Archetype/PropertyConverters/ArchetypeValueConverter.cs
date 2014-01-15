using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Models.PublishedContent;
using Archetype.Umbraco.Models;
using Archetype.Umbraco.Extensions;

namespace Archetype.Umbraco.PropertyConverters
{
    /* based on the Tim Geyssens sample at:  https://github.com/TimGeyssens/MatrixPropEditor/blob/master/SamplePropertyValueConverter/SamplePropertyValueConverter/MatrixValueConverter.cs */

    /* sample model
     * 
     * {
  "fieldsets": [
    {
      "alias": "FS1",
      "remove": false,
      "properties": [
        {
          "alias": "age",
          "value": "abc"
        },
        {
          "alias": "name",
          "value": "123"
        },
        {
          "alias": "blah",
          "value": ""
        },
        {
          "alias": "age2",
          "value": ""
        }
      ]
    },
    {
      "alias": "FS2",
      "remove": false,
      "properties": [
        {
          "alias": "foo",
          "value": ""
        },
        {
          "alias": "bar",
          "value": ""
        }
      ]
    }
  ]
}
     * 
     */

    [PropertyValueType(typeof(Archetype.Umbraco.Models.Archetype))]
    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
    public class ArchetypeValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("Imulus.Archetype");
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null) return null;
            
            var sourceString = source.ToString();         

            if (sourceString.DetectIsJson())
            {
                try
                {
                    var archetype = JsonConvert.DeserializeObject <Archetype.Umbraco.Models.Archetype> (sourceString);
                    return archetype;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return sourceString;
        }
    }
}
