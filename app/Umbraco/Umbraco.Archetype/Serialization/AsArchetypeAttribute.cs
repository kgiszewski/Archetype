using System;

namespace Archetype.Serialization
{
    public class AsArchetypeAttribute : Attribute
    {
        public string FieldsetName { get; set; }

        public AsArchetypeAttribute(string fieldsetname)
        {
            FieldsetName = fieldsetname;
        }
    }
}
