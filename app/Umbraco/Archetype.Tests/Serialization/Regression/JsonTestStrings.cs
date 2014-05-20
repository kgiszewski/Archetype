namespace Archetype.Tests.Serialization.Regression
{
    public class JsonTestStrings
    {
        public const string _SIMPLE_JSON = @"{
  ""fieldsets"": [
    {
      ""alias"": ""simpleModel"",
      ""properties"": [
        {
          ""alias"": ""text"",
          ""value"": ""Test \r\nText \""quote\"" \r\n""
        },
        {
          ""alias"": ""integer"",
          ""value"": ""123""
        },
        {
          ""alias"": ""nullableInteger"",
          ""value"": """"
        },
        {
          ""alias"": ""double"",
          ""value"": ""5.67""
        },
        {
          ""alias"": ""nullableDouble"",
          ""value"": """"
        },
        {
          ""alias"": ""dateTime"",
          ""value"": ""01/01/2000 00:00:00""
        },
        {
          ""alias"": ""nullableDateTime"",
          ""value"": """"
        }
      ]
    }
  ]
}";
        public const string _ESCAPED_JSON =
@"{
   ""fieldsets"":[
      {
         ""properties"":[
            {
               ""alias"":""pages"",
               ""value"":""2439,2440,2441,2442,2443,2444,2445,2446,2447,2448,2449,2450,2451,2452,2453""
            },
            {
               ""alias"":""captions"",
               ""value"":""{\""fieldsets\"":[{\""properties\"":[{\""alias\"":\""captions\"",\""value\"":\""{\\\""fieldsets\\\"":[{\\\""properties\\\"":[{\\\""alias\\\"":\\\""textString\\\"",\\\""value\\\"":\\\""{\\\\\\\""fieldsets\\\\\\\"":[{\\\\\\\""properties\\\\\\\"":[{\\\\\\\""alias\\\\\\\"":\\\\\\\""textString\\\\\\\"",\\\\\\\""value\\\\\\\"":\\\\\\\""First Page\\\\\\\"",\\\\\\\""propertyEditorAlias\\\\\\\"":\\\\\\\""Umbraco.Textbox\\\\\\\"",\\\\\\\""dataTypeId\\\\\\\"":-88,\\\\\\\""dataTypeGuid\\\\\\\"":\\\\\\\""0cc0eba1-9960-42c9-bf9b-60e150b429ae\\\\\\\""}],\\\\\\\""alias\\\\\\\"":\\\\\\\""textItem\\\\\\\""}]}\\\"",\\\""propertyEditorAlias\\\"":\\\""Imulus.Archetype\\\"",\\\""dataTypeId\\\"":3764,\\\""dataTypeGuid\\\"":\\\""496cfe35-448f-4b59-9c75-050798afdce4\\\""}],\\\""alias\\\"":\\\""textList\\\""},{\\\""properties\\\"":[{\\\""alias\\\"":\\\""textString\\\"",\\\""value\\\"":\\\""{\\\\\\\""fieldsets\\\\\\\"":[{\\\\\\\""properties\\\\\\\"":[{\\\\\\\""alias\\\\\\\"":\\\\\\\""textString\\\\\\\"",\\\\\\\""value\\\\\\\"":\\\\\\\""Second Page\\\\\\\"",\\\\\\\""propertyEditorAlias\\\\\\\"":\\\\\\\""Umbraco.Textbox\\\\\\\"",\\\\\\\""dataTypeId\\\\\\\"":-88,\\\\\\\""dataTypeGuid\\\\\\\"":\\\\\\\""0cc0eba1-9960-42c9-bf9b-60e150b429ae\\\\\\\""}],\\\\\\\""alias\\\\\\\"":\\\\\\\""textItem\\\\\\\""}]}\\\"",\\\""propertyEditorAlias\\\"":\\\""Imulus.Archetype\\\"",\\\""dataTypeId\\\"":3764,\\\""dataTypeGuid\\\"":\\\""496cfe35-448f-4b59-9c75-050798afdce4\\\""}],\\\""alias\\\"":\\\""textList\\\""},{\\\""properties\\\"":[{\\\""alias\\\"":\\\""textString\\\"",\\\""value\\\"":\\\""{\\\\\\\""fieldsets\\\\\\\"":[{\\\\\\\""properties\\\\\\\"":[{\\\\\\\""alias\\\\\\\"":\\\\\\\""textString\\\\\\\"",\\\\\\\""value\\\\\\\"":\\\\\\\""Third Page\\\\\\\"",\\\\\\\""propertyEditorAlias\\\\\\\"":\\\\\\\""Umbraco.Textbox\\\\\\\"",\\\\\\\""dataTypeId\\\\\\\"":-88,\\\\\\\""dataTypeGuid\\\\\\\"":\\\\\\\""0cc0eba1-9960-42c9-bf9b-60e150b429ae\\\\\\\""}],\\\\\\\""alias\\\\\\\"":\\\\\\\""textItem\\\\\\\""}]}\\\"",\\\""propertyEditorAlias\\\"":\\\""Imulus.Archetype\\\"",\\\""dataTypeId\\\"":3764,\\\""dataTypeGuid\\\"":\\\""496cfe35-448f-4b59-9c75-050798afdce4\\\""}],\\\""alias\\\"":\\\""textList\\\""},{\\\""properties\\\"":[{\\\""alias\\\"":\\\""textString\\\"",\\\""value\\\"":\\\""{\\\\\\\""fieldsets\\\\\\\"":[{\\\\\\\""properties\\\\\\\"":[{\\\\\\\""alias\\\\\\\"":\\\\\\\""textString\\\\\\\"",\\\\\\\""value\\\\\\\"":\\\\\\\""Fourth Page\\\\\\\"",\\\\\\\""propertyEditorAlias\\\\\\\"":\\\\\\\""Umbraco.Textbox\\\\\\\"",\\\\\\\""dataTypeId\\\\\\\"":-88,\\\\\\\""dataTypeGuid\\\\\\\"":\\\\\\\""0cc0eba1-9960-42c9-bf9b-60e150b429ae\\\\\\\""}],\\\\\\\""alias\\\\\\\"":\\\\\\\""textItem\\\\\\\""}]}\\\"",\\\""propertyEditorAlias\\\"":\\\""Imulus.Archetype\\\"",\\\""dataTypeId\\\"":3764,\\\""dataTypeGuid\\\"":\\\""496cfe35-448f-4b59-9c75-050798afdce4\\\""}],\\\""alias\\\"":\\\""textList\\\""}]}\"",\""propertyEditorAlias\"":\""Imulus.Archetype\"",\""dataTypeId\"":3765,\""dataTypeGuid\"":\""4ed439b3-0ee7-447a-a64e-b2b486b86379\""}],\""alias\"":\""captions\""}]}""
            }
         ],
         ""alias"":""pages""
      }
   ]
}";
    }
}
