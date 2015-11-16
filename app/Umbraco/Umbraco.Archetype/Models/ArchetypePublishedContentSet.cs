using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Archetype.Models
{
    public class ArchetypePublishedContentSet : IEnumerable<ArchetypePublishedContent>
    {
        private IEnumerable<ArchetypePublishedContent> _items { get; set; }

        public ArchetypePublishedContentSet(ArchetypeModel archetype)
        {
            if (archetype == null)
                throw new ArgumentNullException("archetype");

            this.ArchetypeModel = archetype;

            _items = archetype.Fieldsets
                .Where(x => x.Disabled == false)
                .Select(x => new ArchetypePublishedContent(x));
        }

        internal ArchetypeModel ArchetypeModel { get; private set; }

        public IEnumerator<ArchetypePublishedContent> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}