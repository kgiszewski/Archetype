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
          ""value"": ""Test Text""
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
    }
}
