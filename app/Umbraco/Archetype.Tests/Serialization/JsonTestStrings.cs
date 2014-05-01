namespace Archetype.Tests.Serialization
{
    public class JsonTestStrings
    {
        public const string _MERGER_DETAILS_JSON =
            @"{
  ""fieldsets"": [
    {
      ""alias"": ""mergerDetails"",
      ""properties"": [
        {
          ""alias"": ""MergerDate"",
          ""value"": ""07/06/2014 08:09:10""
        }
      ]
    }
  ]
}";

        public const string _ANNUAL_STATEMENT_JSON =
            @"{
  ""fieldsets"": [
    {
      ""alias"": ""annualStatement"",
      ""properties"": [
        {
          ""alias"": ""FiscalYearStart"",
          ""value"": ""01/09/2013 00:00:00""
        },
        {
          ""alias"": ""FiscalYearEnd"",
          ""value"": ""31/08/2014 00:00:00""
        },
        {
          ""alias"": ""DividendPaymentDate"",
          ""value"": """"
        },
        {
          ""alias"": ""TotalShares"",
          ""value"": ""345678""
        },
        {
          ""alias"": ""Sales"",
          ""value"": ""123456700.89""
        },
        {
          ""alias"": ""Profit"",
          ""value"": ""1123456.78""
        }
      ]
    }
  ]
}";

        public const string _CONTACT_DETAILS_JSON =
            @"{
  ""fieldsets"": [
    {
      ""alias"": ""contactDetails"",
      ""properties"": [
        {
          ""alias"": ""name"",
          ""value"": ""Test""
        },
        {
          ""alias"": ""address"",
          ""value"": ""Test Address""
        },
        {
          ""alias"": ""telephone"",
          ""value"": ""111""
        },
        {
          ""alias"": ""mobile"",
          ""value"": ""000""
        },
        {
          ""alias"": ""fax"",
          ""value"": ""000""
        },
        {
          ""alias"": ""email"",
          ""value"": ""test@test.com""
        },
        {
          ""alias"": ""webSite"",
          ""value"": {
            ""fieldsets"": [
              {
                ""alias"": ""urlPicker"",
                ""properties"": [
                  {
                    ""alias"": ""title"",
                    ""value"": ""The Test Company""
                  },
                  {
                    ""alias"": ""url"",
                    ""value"": ""http://test.com""
                  },
                  {
                    ""alias"": ""content"",
                    ""value"": """"
                  },
                  {
                    ""alias"": ""media"",
                    ""value"": """"
                  },
                  {
                    ""alias"": ""openInNewWindow"",
                    ""value"": ""1""
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

        public const string _COMPANY_DETAILS_JSON =
            @"{
  ""fieldsets"": [
    {
      ""alias"": ""companyDetails"",
      ""properties"": [
        {
          ""alias"": ""region"",
          ""value"": ""Test Region""
        },
        {
          ""alias"": ""contactDetails"",
          ""value"": {
            ""fieldsets"": [
              {
                ""alias"": ""contactDetails"",
                ""properties"": [
                  {
                    ""alias"": ""name"",
                    ""value"": ""Test""
                  },
                  {
                    ""alias"": ""address"",
                    ""value"": ""Test Address""
                  },
                  {
                    ""alias"": ""telephone"",
                    ""value"": ""111""
                  },
                  {
                    ""alias"": ""mobile"",
                    ""value"": ""000""
                  },
                  {
                    ""alias"": ""fax"",
                    ""value"": ""000""
                  },
                  {
                    ""alias"": ""email"",
                    ""value"": ""test@test.com""
                  },
                  {
                    ""alias"": ""webSite"",
                    ""value"": {
                      ""fieldsets"": [
                        {
                          ""alias"": ""urlPicker"",
                          ""properties"": [
                            {
                              ""alias"": ""title"",
                              ""value"": ""The Test Company""
                            },
                            {
                              ""alias"": ""url"",
                              ""value"": ""http://test.com""
                            },
                            {
                              ""alias"": ""content"",
                              ""value"": """"
                            },
                            {
                              ""alias"": ""media"",
                              ""value"": """"
                            },
                            {
                              ""alias"": ""openInNewWindow"",
                              ""value"": ""1""
                            }
                          ]
                        }
                      ]
                    }
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

        public const string _NULL_VALUES_JSON = @"{""fieldsets"":[{""alias"":""contactDetails"",""properties"":[{""alias"":""name"",""value"":""""},{""alias"":""address"",""value"":""""},{""alias"":""telephone"",""value"":""""},{""alias"":""mobile"",""value"":""""},{""alias"":""fax"",""value"":""""},{""alias"":""email"",""value"":""""},{""alias"":""webSite"",""value"":""""}]}]}";
    }
}
