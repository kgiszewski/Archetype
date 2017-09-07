using System;

namespace Archetype.Models
{
    public class ArchetypeConfigFileModel
    {
        public Guid Id { get; set; }
        public bool OptInNewVersionNotification { get; set; }
    }
}
