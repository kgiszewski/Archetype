Here is a very simple set-up:

```json
//datatype json
{
  "fieldsets": [
    {
      "alias": "webSitePortfolio",
      "label": "Web Site Portfolio",
      "properties": [
        {
          "alias": "portfolioCaption",
          "label": "Portfolio Caption",
          "dataTypeGuid": "0cc0eba1-9960-42c9-bf9b-60e150b429ae",
          "value": ""
        },
        {
          "alias": "webSiteUrls",
          "label": "Web Site Urls",
          "helpText": "",
          "dataTypeGuid": "...", //is generated for url picker below
          "value": ""
        }
      ]
    }
  ]
}

{
  "fieldsets": [
    {
      "alias": "urlPicker",
      "label": "URL Picker",
      "properties": [
        {
          "alias": "title",
          "label": "Title",
          "helpText": "",
          "dataTypeId": "-88",
          "value": "",
          "dataTypeGuid": "0cc0eba1-9960-42c9-bf9b-60e150b429ae"
        },
        {
          "alias": "url",
          "label": "URL",
          "helpText": "",
          "dataTypeId": "-88",
          "value": "",
          "dataTypeGuid": "0cc0eba1-9960-42c9-bf9b-60e150b429ae"
        },
        {
          "alias": "content",
          "label": "Content",
          "helpText": "",
          "dataTypeId": "1034",
          "value": "",
          "dataTypeGuid": "a6857c73-d6e9-480c-b6e6-f15f6ad11125"
        },
        {
          "alias": "media",
          "label": "Media",
          "helpText": "",
          "dataTypeId": "1035",
          "value": "",
          "dataTypeGuid": "93929b9a-93a2-4e2a-b239-d99334440a59"
        },
        {
          "alias": "openInNewWindow",
          "label": "Open In New Window",
          "helpText": "",
          "dataTypeId": "-49",
          "value": "",
          "dataTypeGuid": "92897bc6-a5f3-4ffe-ae27-f2e7e33dda49"
        }
      ]
    }
  ]
}

//data json example
{
  "fieldsets": [
    {
      "properties": [
        {
          "alias": "portfolioCaption",
          "value": "Sites in portfolio"
        },
        {
          "alias": "webSiteUrls",
          "value": {
            "fieldsets": [
              {
                "properties": [
                  {
                    "alias": "url",
                    "value": "http://1.com"
                  },
                  {
                    "alias": "title",
                    "value": "Site 1"
                  },
                  {
                    "alias": "media",
                    "value": null
                  },
                  {
                    "alias": "openInNewWindow",
                    "value": 0
                  },
                  {
                    "alias": "content",
                    "value": null
                  }
                ],
                "alias": "urlPicker"
              },
              {
                "properties": [
                  {
                    "alias": "content",
                    "value": null
                  },
                  {
                    "alias": "media",
                    "value": null
                  },
                  {
                    "alias": "openInNewWindow",
                    "value": 0
                  },
                  {
                    "alias": "title",
                    "value": "Site 2"
                  },
                  {
                    "alias": "url",
                    "value": "http://2.com"
                  }
                ],
                "alias": "urlPicker"
              },
              {
                "properties": [
                  {
                    "alias": "openInNewWindow",
                    "value": 0
                  },
                  {
                    "alias": "media",
                    "value": null
                  },
                  {
                    "alias": "content",
                    "value": null
                  },
                  {
                    "alias": "title",
                    "value": "Site 3"
                  },
                  {
                    "alias": "url",
                    "value": "http://3.com"
                  }
                ],
                "alias": "urlPicker"
              }
            ]
          }
        }
      ],
      "alias": "webSitePortfolio"
    }
  ]
}
```

```csharp
[AsArchetype("webSitePortfolio")]
[JsonConverter(typeof(ArchetypeJsonConverter))] 
public class WebSitePortfolio
{
	[JsonProperty("portfolioCaption")]
	public string Caption { get; set; }
	[JsonProperty("webSiteUrls")]
	public List<UrlPickerModel> Urls { get; set; }
}

[AsArchetype("urlPicker")]
[JsonConverter(typeof(ArchetypeJsonConverter))]    
public class UrlPickerModel
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

//data retrieval
WebSitePortfolio GetWebSitePortfolio(IPublishedContent content)
{
    return content.GetModelFromArchetype<WebSitePortfolio>(<umbracoFieldAlias>);
}

//simple nunit test
[Test]
public void WebSitePortfolioJsonDeserializes()
{

	const string _JSON = @"{""fieldsets"":[{""properties"":[{""alias"":""portfolioCaption"",""value"":""Sites in portfolio""},{""alias"":""webSiteUrls"",""value"":{""fieldsets"":[{""properties"":[{""alias"":""url"",""value"":""http://1.com""},{""alias"":""title"",""value"":""Site 1""},{""alias"":""media"",""value"":null},{""alias"":""openInNewWindow"",""value"":0},{""alias"":""content"",""value"":null}],""alias"":""urlPicker""},{""properties"":[{""alias"":""content"",""value"":null},{""alias"":""media"",""value"":null},{""alias"":""openInNewWindow"",""value"":0},{""alias"":""title"",""value"":""Site 2""},{""alias"":""url"",""value"":""http://2.com""}],""alias"":""urlPicker""},{""properties"":[{""alias"":""openInNewWindow"",""value"":0},{""alias"":""media"",""value"":null},{""alias"":""content"",""value"":null},{""alias"":""title"",""value"":""Site 3""},{""alias"":""url"",""value"":""http://3.com""}],""alias"":""urlPicker""}]}}],""alias"":""webSitePortfolio""}]}";

	var model = JsonConvert.DeserializeObject<WebSitePortfolio>(_JSON);

	Assert.IsNotNull(model);
	Assert.IsInstanceOf<WebSitePortfolio>(model);

}
```