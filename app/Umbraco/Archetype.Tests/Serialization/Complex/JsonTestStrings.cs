namespace Archetype.Tests.Serialization.Complex
{
    public class JsonTestStrings
    {
        public const string _PAGE_DETAILS_JSON =
            @"{
  ""fieldsets"": [
    {
      ""alias"": ""pages"",
      ""properties"": [
        {
          ""alias"": ""pages"",
          ""value"": ""2439,2440,2441,2442,2443,2444,2445,2446,2447,2448,2449,2450,2451,2452,2453""
        }
      ]
    },
    {
      ""alias"": ""captions"",
      ""properties"": [
        {
          ""alias"": ""captions"",
          ""value"": {
            ""fieldsets"": [
              {
                ""alias"": ""textstringArray"",
                ""properties"": [
                  {
                    ""alias"": ""textstring"",
                    ""value"": ""First Page""
                  }
                ]
              },
              {
                ""alias"": ""textstringArray"",
                ""properties"": [
                  {
                    ""alias"": ""textstring"",
                    ""value"": ""Second Page""
                  }
                ]
              },
              {
                ""alias"": ""textstringArray"",
                ""properties"": [
                  {
                    ""alias"": ""textstring"",
                    ""value"": ""Third Page""
                  }
                ]
              },
              {
                ""alias"": ""textstringArray"",
                ""properties"": [
                  {
                    ""alias"": ""textstring"",
                    ""value"": ""Fourth Page""
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

        public const string _PAGES_JSON =
            @"{
  ""fieldsets"": [
    {
      ""alias"": ""Pages"",
      ""properties"": [
        {
          ""alias"": ""Pages"",
          ""value"": {
            ""fieldsets"": [
              {
                ""alias"": ""TextPage"",
                ""properties"": [
                  {
                    ""alias"": ""Media"",
                    ""value"": {
                      ""fieldsets"": [
                        {
                          ""alias"": ""slideShow"",
                          ""properties"": [
                            {
                              ""alias"": ""Slides"",
                              ""value"": ""1,2,3,4,5,6,7,8""
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    ""alias"": ""Seo"",
                    ""value"": {
                      ""fieldsets"": [
                        {
                          ""alias"": ""seo"",
                          ""properties"": [
                            {
                              ""alias"": ""MetaTitle"",
                              ""value"": ""Test Meta Title""
                            },
                            {
                              ""alias"": ""MetaDescription"",
                              ""value"": ""Test Meta Description""
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    ""alias"": ""PageTitle"",
                    ""value"": ""Home Page""
                  },
                  {
                    ""alias"": ""BodyText"",
                    ""value"": ""Home Page Body""
                  }
                ]
              },
              {
                ""alias"": ""TextPage"",
                ""properties"": [
                  {
                    ""alias"": ""Media"",
                    ""value"": {
                      ""fieldsets"": [
                        {
                          ""alias"": ""slideShow"",
                          ""properties"": [
                            {
                              ""alias"": ""Slides"",
                              ""value"": ""1,2,3,4,5,6,7,8""
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    ""alias"": ""Seo"",
                    ""value"": {
                      ""fieldsets"": [
                        {
                          ""alias"": ""seo"",
                          ""properties"": [
                            {
                              ""alias"": ""MetaTitle"",
                              ""value"": ""Test Meta Title""
                            },
                            {
                              ""alias"": ""MetaDescription"",
                              ""value"": ""Test Meta Description""
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    ""alias"": ""PageTitle"",
                    ""value"": ""About us Page""
                  },
                  {
                    ""alias"": ""BodyText"",
                    ""value"": ""About us Page Body""
                  }
                ]
              },
              {
                ""alias"": ""TextPage"",
                ""properties"": [
                  {
                    ""alias"": ""Media"",
                    ""value"": {
                      ""fieldsets"": [
                        {
                          ""alias"": ""slideShow"",
                          ""properties"": [
                            {
                              ""alias"": ""Slides"",
                              ""value"": ""1,2,3,4,5,6,7,8""
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    ""alias"": ""Seo"",
                    ""value"": {
                      ""fieldsets"": [
                        {
                          ""alias"": ""seo"",
                          ""properties"": [
                            {
                              ""alias"": ""MetaTitle"",
                              ""value"": ""Test Meta Title""
                            },
                            {
                              ""alias"": ""MetaDescription"",
                              ""value"": ""Test Meta Description""
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    ""alias"": ""PageTitle"",
                    ""value"": ""Contact us Page""
                  },
                  {
                    ""alias"": ""BodyText"",
                    ""value"": ""Contact us Page Body""
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
