namespace Archetype.Tests.Serialization.Enumerable
{
    public class JsonTestStrings
    {
        public const string _ABOUT_US_JSON =
            @"{
  ""fieldsets"": [
    {
      ""alias"": ""aboutUs"",
      ""properties"": [
        {
          ""alias"": ""contacts"",
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
				},
				{
				  ""alias"": ""contactDetails"",
				  ""properties"": [
					{
					  ""alias"": ""name"",
					  ""value"": ""Test1""
					},
					{
					  ""alias"": ""address"",
					  ""value"": ""Test Address 1""
					},
					{
					  ""alias"": ""telephone"",
					  ""value"": ""222""
					},
					{
					  ""alias"": ""mobile"",
					  ""value"": ""111""
					},
					{
					  ""alias"": ""fax"",
					  ""value"": ""111""
					},
					{
					  ""alias"": ""email"",
					  ""value"": ""test1@test.com""
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
								""value"": ""The Test Company 1""
							  },
							  {
								""alias"": ""url"",
								""value"": ""http://test1.com""
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
				},
				{
				  ""alias"": ""contactDetails"",
				  ""properties"": [
					{
					  ""alias"": ""name"",
					  ""value"": ""Test 2""
					},
					{
					  ""alias"": ""address"",
					  ""value"": ""Test Address 2""
					},
					{
					  ""alias"": ""telephone"",
					  ""value"": ""333""
					},
					{
					  ""alias"": ""mobile"",
					  ""value"": ""222""
					},
					{
					  ""alias"": ""fax"",
					  ""value"": ""222""
					},
					{
					  ""alias"": ""email"",
					  ""value"": ""test2@test.com""
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
								""value"": ""The Test Company 2""
							  },
							  {
								""alias"": ""url"",
								""value"": ""http://test2.com""
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
								""value"": ""0""
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

        public const string _FEEDBACK_JSON =
            @"{
  ""fieldsets"": [
    {
        {
          ""alias"": ""feedback"",
          ""value"": {
			  ""fieldsets"": [
				{
				  ""alias"": ""testimonials"",
				  ""properties"": [
					{
					  ""alias"": ""String"",
					  ""value"": ""Testimonial 1""
					}
				  ]
				},
				{
				  ""alias"": ""testimonials"",
				  ""properties"": [
					{
					  ""alias"": ""String"",
					  ""value"": ""Testimonial 2""
					}
				  ]
				},
								{
				  ""alias"": ""testimonials"",
				  ""properties"": [
					{
					  ""alias"": ""String"",
					  ""value"": ""Testimonial 3""
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
