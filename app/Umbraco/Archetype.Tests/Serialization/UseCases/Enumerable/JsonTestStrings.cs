namespace Archetype.Tests.Serialization.UseCases.Enumerable
{
    public class JsonTestStrings
    {
        public const string _ABOUT_US_JSON =
            @"";

        public const string _FEEDBACK_JSON =
            @"{
  ""fieldsets"": [
    {
      ""alias"": ""feedback"",
      ""properties"": [
        {
          ""alias"": ""testimonial"",
          ""value"": ""Testimonial 1""
        }
      ]
    },
    {
      ""alias"": ""feedback"",
      ""properties"": [
        {
          ""alias"": ""testimonial"",
          ""value"": ""Testimonial 2""
        }
      ]
    },
    {
      ""alias"": ""feedback"",
      ""properties"": [
        {
          ""alias"": ""testimonial"",
          ""value"": ""Testimonial 3""
        }
      ]
    }
  ]
}";

        public const string _CAPTIONS_JSON =
            @"{
  ""fieldsets"": [
    {
      ""alias"": ""captions"",
      ""properties"": [
        {
          ""alias"": ""captions"",
          ""value"": {
            ""fieldsets"": [
              {
                ""alias"": ""textArray"",
                ""properties"": [
                  {
                    ""alias"": ""textstring"",
                    ""value"": ""Caption 1""
                  }
                ]
              },
              {
                ""alias"": ""textArray"",
                ""properties"": [
                  {
                    ""alias"": ""textstring"",
                    ""value"": ""Caption 2""
                  }
                ]
              },
              {
                ""alias"": ""textArray"",
                ""properties"": [
                  {
                    ""alias"": ""textstring"",
                    ""value"": ""Caption 3""
                  }
                ]
              }
            ]
          }
        }
      ]
    }
  ]
}";
    }
}
