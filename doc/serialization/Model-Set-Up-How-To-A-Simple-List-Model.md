You can use this model when there is a single fieldset. However, the fieldset has multiple items. For example, we may be storing multiple contact details.

```json
Datatype json
{
  "fieldsets": [
    {
      "alias": "contactDetails",
      "label": "Contact Details",
      "properties": [
        {
          "alias": "name",
          "label": "Name",
          "dataTypeId": "-88",
          "value": "",
          "dataTypeGuid": "0cc0eba1-9960-42c9-bf9b-60e150b429ae"
        },
        {
          "alias": "address",
          "label": "Address",
          "dataTypeId": "-89",
          "value": "",
          "dataTypeGuid": "c6bac0dd-4ab9-45b1-8e30-e4b619ee5da3"
        },
        {
          "alias": "telephone",
          "label": "Telephone",
          "dataTypeId": "-88",
          "value": "",
          "dataTypeGuid": "0cc0eba1-9960-42c9-bf9b-60e150b429ae"
        },
        {
          "alias": "email",
          "label": "Email",
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
          "alias": "name",
          "value": "Name 1"
        },
        {
          "alias": "address",
          "value": "Address 1"
        },
        {
          "alias": "telephone",
          "value": "12345"
        },
        {
          "alias": "email",
          "value": "1@one.com"
        }
      ],
      "alias": "contactDetails"
    },
    {
      "properties": [
        {
          "alias": "name",
          "value": "Name 2"
        },
        {
          "alias": "address",
          "value": "Address 2"
        },
        {
          "alias": "telephone",
          "value": "23456"
        },
        {
          "alias": "email",
          "value": "2@two.com"
        }
      ],
      "alias": "contactDetails"
    }
  ]
}
```

```csharp
[AsArchetype("contactDetails")]
[JsonConverter(typeof(ArchetypeJsonConverter))]
public class ContactDetailsListModel : List<ContactDetailsModel>
{
}

[AsArchetype("contactDetails")]
[JsonConverter(typeof(ArchetypeJsonConverter))]    
public class ContactDetailsModel
{        
	[JsonProperty("name")]
	public string Name { get; set; }
	[JsonProperty("address")]
	public string Address { get; set; }
	[JsonProperty("telephone")]
	public string Telephone { get; set; }
	[JsonProperty("email")]
	public string Email { get; set; }
}

...

ContactDetailsListModel GetContactDetails(IPublishedContent content)
{
	return content.GetModelFromArchetype<ContactDetailsListModel>(<umbracoFieldAlias>);
}

IEnumerable<ContactDetailsModel> GetContactDetails(IPublishedContent content)
{
	return content.GetModelFromArchetype<ContactDetailsListModel>(<umbracoFieldAlias>);
}

```