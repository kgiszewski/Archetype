Here is a simple slide show example using the serializer:

### Models
```csharp
[AsArchetype("slides")]
[JsonConverter(typeof(ArchetypeJsonConverter))]      
public class SlideShowViewModel
{
	[JsonProperty("slides")]
	public string Slides { get; set; }
	[AsFieldset]
	[JsonProperty("captions")]
	public Captions Captions { get; set; }

	public IEnumerable<IPublishedContent> GetSlidesAsMedia(UmbracoHelper umbracoHelper)
	{
		if (String.IsNullOrEmpty(Slides))
			return new List<IPublishedContent>();

		var slides = Slides.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

		return !slides.Any() ? new List<IPublishedContent>() : umbracoHelper.TypedMedia(slides);
	}

	public IEnumerable<TextString> GetCaptions()
	{
		return Captions.TextStringArray.Where(caption => !String.IsNullOrEmpty(caption.Text));
	}
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
```

### Controller
```csharp
public class SlideShowController : SurfaceController
{
	[ChildActionOnly]
	public ActionResult GetSlideShow()
	{
		var model = Umbraco.AssignedContentItem.GetModelFromArchetype<SlideShowViewModel>("cmsPropertyAlias");
		return PartialView("SlideShow", model);
	}
}
```

### View
```razor
@inherits UmbracoViewPage<SlideShowViewModel>
@{
    // Add your own slide show logic
	var slides = Model.GetSlidesAsMedia(Umbraco);
    var captions = Model.GetCaptions().ToList();
}
<div>
    <ul id="slides" >
        @foreach (var slide in slides)
        {
            <li><img src="@slide.Url" /></li>
        }
    </ul>
</div>
@if (captions.Any())
{
 <div>
	<ul id="captions">
		@foreach (var caption in captions)
		{
			<li>@caption.Text</li>
		}
	</ul>
</div>    
}
```