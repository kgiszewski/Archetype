In this set-up the Archetype is complex: In the first level, there are two fieldsets, each of which will not have repeated items. In the second level, there is another fieldset with a repeated item.

Because of the multiple levels of nesting, the set-up is involved.

```json
Structure
/
|-> Pages (Single item)
|-> Captions (Single item)
	|-> Textstring Array (Repeated item)

datatype json
{
  "fieldsets": [
    {
      "alias": "pages",
      "label": "Pages",
      "properties": [
        {
          "alias": "pages",
          "label": "Pages",
          "dataTypeId": "...",
          "value": "",
          "dataTypeGuid": "0cc0eba1-9960-42c9-bf9b-60e150b429ae"
        }
      ]
    },
    {
      "alias": "captions",
      "label": "Captions",
      "properties": [
        {
          "alias": "captions",
          "label": "Captions",
          "dataTypeId": "...",
          "value": "",
          "dataTypeGuid": "80e69e2b-dcaf-4a90-ada2-15eb91391ea9"
        }
      ]
    }
  ]
}

{
  "fieldsets": [
    {
      "alias": "textstringArray",
      "label": "Textstring Array",
      "properties": [
        {
          "alias": "textstring",
          "label": "Textstring",
          "dataTypeId": "-88",
          "value": "",
          "dataTypeGuid": "0cc0eba1-9960-42c9-bf9b-60e150b429ae"
        }
      ]
    }
  ]
}

data json example
{
  "fieldsets": [
    {
      "properties": [
        {
          "alias": "pages",
          "value": "2639,2640,2641,2642,2643,2644"
        }
      ],
      "alias": "pages"
    },
    {
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
                    "value": "Caption 1"
                  }
                ]
              },
              {
                "alias": "textstringArray",
                "properties": [
                  {
                    "alias": "textstring",
                    "value": "Caption 2"
                  }
                ]
              },
              {
                "alias": "textstringArray",
                "properties": [
                  {
                    "alias": "textstring",
                    "value": "Caption 3"
                  }
                ]
              },
              {
                "alias": "textstringArray",
                "properties": [
                  {
                    "alias": "textstring",
                    "value": "Caption 4"
                  }
                ]
              }
            ]
          }
        }
      ],
      "alias": "captions"
    }
  ]
}
```

```csharp
[AsArchetype("pages")]
[JsonConverter(typeof(ArchetypeJsonConverter))]      
public class PagesModel
{
	[AsFieldset] //optional for first item
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
	public TextStringArray TextStringArray { get; set; }
}

[AsArchetype("textstringArray")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class TextStringArray : List<TextString>
{

}

[AsArchetype("textstringArray")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class TextString
{
	[JsonProperty("textstring")]
	public String Text { get; set; }
} 
...
PagesModel GetPages(IPublishedContent content)
{
	return content.GetModelFromArchetype<PagesModel>(<umbracoFieldAlias>);
}
...
var model = GetPages(...)
foreach(var caption in model.Captions.TextStringArray)
{
   @caption.Text
}
```