using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Archetype.Models
{
    public class ArchetypePublishedContent : IPublishedContent
    {
        private ArchetypeFieldsetModel _fieldset;

        private readonly Dictionary<string, IPublishedProperty> _properties;

        public ArchetypePublishedContent(ArchetypeFieldsetModel fieldset)
        {
            if (fieldset == null)
                throw new ArgumentNullException("fieldset");

            _fieldset = fieldset;

            _properties = fieldset.Properties
                .ToDictionary(
                    x => x.Alias,
                    x => new ArchetypePublishedProperty(x) as IPublishedProperty,
                    StringComparer.InvariantCultureIgnoreCase);
        }

        internal ArchetypeFieldsetModel ArchetypeFieldset
        {
            get { return _fieldset; }
        }

        public IEnumerable<IPublishedContent> Children
        {
            get { return Enumerable.Empty<IPublishedContent>(); }
        }

        public IEnumerable<IPublishedContent> ContentSet
        {
            get { return Enumerable.Empty<IPublishedContent>(); }
        }

        public PublishedContentType ContentType
        {
            get { return default(PublishedContentType); }
        }

        public DateTime CreateDate
        {
            get { return DateTime.MinValue; }
        }

        public int CreatorId
        {
            get { return default(int); }
        }

        public string CreatorName
        {
            get { return default(string); }
        }

        public string DocumentTypeAlias
        {
            get { return _fieldset.Alias; }
        }

        public int DocumentTypeId
        {
            get { return default(int); }
        }

        public int GetIndex()
        {
            return default(int);
        }

        public IPublishedProperty GetProperty(string alias, bool recurse)
        {
            IPublishedProperty property;
            return _properties.TryGetValue(alias, out property) ? property : null;
        }

        public IPublishedProperty GetProperty(string alias)
        {
            return this.GetProperty(alias, false);
        }

        public int Id
        {
            get { return default(int); }
        }

        public bool IsDraft
        {
            get { return _fieldset.Disabled; }
        }

        public PublishedItemType ItemType
        {
            get { return PublishedItemType.Content; }
        }

        public int Level
        {
            get { return default(int); }
        }

        public string Name
        {
            get { return default(string); }
        }

        public IPublishedContent Parent
        {
            get { return default(IPublishedContent); }
        }

        public string Path
        {
            get { return default(string); }
        }

        public ICollection<IPublishedProperty> Properties
        {
            get { return _properties.Values; }
        }

        public int SortOrder
        {
            get { return default(int); }
        }

        public int TemplateId
        {
            get { return default(int); }
        }

        public DateTime UpdateDate
        {
            get { return default(DateTime); }
        }

        public string Url
        {
            get { return default(string); }
        }

        public string UrlName
        {
            get { return default(string); }
        }

        public Guid Version
        {
            get { return Guid.Empty; }
        }

        public int WriterId
        {
            get { return default(int); }
        }

        public string WriterName
        {
            get { return default(string); }
        }

        public object this[string alias]
        {
            get
            {
                var property = this.GetProperty(alias);

                return property == null
                    ? null
                    : property.Value;
            }
        }
    }
}