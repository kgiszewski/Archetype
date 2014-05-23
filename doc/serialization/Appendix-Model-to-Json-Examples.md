Here are a few more Model to Json conversions.

### Simple Model
```csharp
[AsArchetype("urlPicker")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class UrlPicker
{
	[JsonProperty("title")]
	public string Title { get; set; }
	[JsonProperty("url")]
	public string Url { get; set; }
	[JsonProperty("content")]
	public int? Content { get; set; }
	[JsonProperty("media")]
	public int? Media { get; set; }
	[JsonProperty("openInNewWindow")]
	public bool OpenInNewWindow { get; set; }
}

[AsArchetype("contactDetails")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class ContactDetails
{
	[JsonProperty("name")]
	public string Name { get; set; }
	[JsonProperty("address")]
	public string Address { get; set; }
	[JsonProperty("telephone")]
	public string Telephone { get; set; }
	[JsonProperty("mobile")]
	public string Mobile { get; set; }
	[JsonProperty("fax")]
	public string Fax { get; set; }
	[JsonProperty("email")]
	public string Email { get; set; }
	[JsonProperty("webSite")]
	public UrlPicker WebSite { get; set; }
}
```
```json
{
  "fieldsets": [
    {
      "alias": "contactDetails",
      "properties": [
        {
          "alias": "name",
          "value": "Test"
        },
        {
          "alias": "address",
          "value": "Test Address"
        },
        {
          "alias": "telephone",
          "value": "111"
        },
        {
          "alias": "mobile",
          "value": "000"
        },
        {
          "alias": "fax",
          "value": "000"
        },
        {
          "alias": "email",
          "value": "test@test.com"
        },
        {
          "alias": "webSite",
          "value": {
            "fieldsets": [
              {
                "alias": "urlPicker",
                "properties": [
                  {
                    "alias": "title",
                    "value": "The Test Company"
                  },
                  {
                    "alias": "url",
                    "value": "http://test.com"
                  },
                  {
                    "alias": "content",
                    "value": ""
                  },
                  {
                    "alias": "media",
                    "value": ""
                  },
                  {
                    "alias": "openInNewWindow",
                    "value": "1"
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
```

### Complex Nested Model
```csharp
[AsArchetype("pages")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class PageDetails
{
	[JsonProperty("pages")]
	public string Pages { get; set; }
	[AsFieldset]
	[JsonProperty("captions")]
	public Captions Captions { get; set; }
}

[AsArchetype("captions")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class Captions
{
	[JsonProperty("captions")]
	public TextList TextStringArray { get; set; }
}

[AsArchetype("textstringArray")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class TextList : List<TextItem>
{
}

[AsArchetype("textstringArray")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class TextItem
{
	[JsonProperty("textstring")]
	public string TextString { get; set; }
}
```
```json
{
  "fieldsets": [
    {
      "alias": "pages",
      "properties": [
        {
          "alias": "pages",
          "value": "2439,2440,2441,2442,2443,2444,2445,2446,2447,2448,2449,2450,2451,2452,2453"
        }
      ]
    },
    {
      "alias": "captions",
      "properties": [
        {
          "alias": "captions",
          "value": {
            "fieldsets": [
              {
                "alias": "textstringArray",
                "properties": [
                  {
                    "alias": "textstring",
                    "value": "First Page"
                  }
                ]
              },
              {
                "alias": "textstringArray",
                "properties": [
                  {
                    "alias": "textstring",
                    "value": "Second Page"
                  }
                ]
              },
              {
                "alias": "textstringArray",
                "properties": [
                  {
                    "alias": "textstring",
                    "value": "Third Page"
                  }
                ]
              },
              {
                "alias": "textstringArray",
                "properties": [
                  {
                    "alias": "textstring",
                    "value": "Fourth Page"
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
```

### Complex Nested Tree Model
```csharp
public abstract class PageBase
{
	public string PageTitle { get; set; }
	public string BodyText { get; set; }
}

[AsArchetype("slideShow")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class SlideShow
{
	public string Slides { get; set; }
}

[AsArchetype("seo")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class Seo
{
	public string MetaTitle { get; set; }
	public string MetaDescription { get; set; }
}

[AsArchetype("TextPage")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class TextPage : PageBase
{
	[AsFieldset]
	public SlideShow Media { get; set; }

	[AsFieldset]
	public Seo Seo { get; set; }
}

[AsArchetype("TextPageList")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class TextPageList : List<TextPage>
{
}

[AsArchetype("Pages")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class PageList
{
	public TextPageList Pages { get; set; }
}
```
```json
{
  "fieldsets": [
    {
      "alias": "Pages",
      "properties": [
        {
          "alias": "Pages",
          "value": {
            "fieldsets": [
              {
                "alias": "TextPage",
                "properties": [
                  {
                    "alias": "Media",
                    "value": {
                      "fieldsets": [
                        {
                          "alias": "slideShow",
                          "properties": [
                            {
                              "alias": "Slides",
                              "value": "1,2,3,4,5,6,7,8"
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    "alias": "Seo",
                    "value": {
                      "fieldsets": [
                        {
                          "alias": "seo",
                          "properties": [
                            {
                              "alias": "MetaTitle",
                              "value": "Test Meta Title"
                            },
                            {
                              "alias": "MetaDescription",
                              "value": "Test Meta Description"
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    "alias": "PageTitle",
                    "value": "Home Page"
                  },
                  {
                    "alias": "BodyText",
                    "value": "Home Page Body"
                  }
                ]
              },
              {
                "alias": "TextPage",
                "properties": [
                  {
                    "alias": "Media",
                    "value": {
                      "fieldsets": [
                        {
                          "alias": "slideShow",
                          "properties": [
                            {
                              "alias": "Slides",
                              "value": "1,2,3,4,5,6,7,8"
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    "alias": "Seo",
                    "value": {
                      "fieldsets": [
                        {
                          "alias": "seo",
                          "properties": [
                            {
                              "alias": "MetaTitle",
                              "value": "Test Meta Title"
                            },
                            {
                              "alias": "MetaDescription",
                              "value": "Test Meta Description"
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    "alias": "PageTitle",
                    "value": "About us Page"
                  },
                  {
                    "alias": "BodyText",
                    "value": "About us Page Body"
                  }
                ]
              },
              {
                "alias": "TextPage",
                "properties": [
                  {
                    "alias": "Media",
                    "value": {
                      "fieldsets": [
                        {
                          "alias": "slideShow",
                          "properties": [
                            {
                              "alias": "Slides",
                              "value": "1,2,3,4,5,6,7,8"
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    "alias": "Seo",
                    "value": {
                      "fieldsets": [
                        {
                          "alias": "seo",
                          "properties": [
                            {
                              "alias": "MetaTitle",
                              "value": "Test Meta Title"
                            },
                            {
                              "alias": "MetaDescription",
                              "value": "Test Meta Description"
                            }
                          ]
                        }
                      ]
                    }
                  },
                  {
                    "alias": "PageTitle",
                    "value": "Contact us Page"
                  },
                  {
                    "alias": "BodyText",
                    "value": "Contact us Page Body"
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
```