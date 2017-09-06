namespace Archetype.Models
{
    public class ArchetypeUpdateNotification
    {
        public bool IsUpdateAvailable { get; set; }
        public string Headline { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
