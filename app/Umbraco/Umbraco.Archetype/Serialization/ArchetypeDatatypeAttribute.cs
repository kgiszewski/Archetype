using System;

namespace Archetype.Umbraco.Serialization
{
    public class ArchetypeDatatypeAttribute : Attribute
    {
        public string FieldsetName { get; set; }

        public ArchetypeDatatypeAttribute(string fieldsetname)
        {
            FieldsetName = fieldsetname;
        }
    }
}
