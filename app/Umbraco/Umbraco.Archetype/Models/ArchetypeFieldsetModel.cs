using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Umbraco.Core;
using System;

namespace Archetype.Models
{
    public class ArchetypeFieldsetModel
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("properties")]
        public IEnumerable<ArchetypePropertyModel> Properties;

        [JsonProperty("id")]
        public Guid Id { get; set; }

        public ArchetypeFieldsetModel()
        {
            Properties = new List<ArchetypePropertyModel>();
        }

        #region Helper Methods

        public string GetValue(string propertyAlias)
        {
            return GetValue<string>(propertyAlias);
        }

        public T GetValue<T>(string propertyAlias)
        {
            var property = GetProperty(propertyAlias);

            if (property == null || property.Value == null || string.IsNullOrEmpty(property.Value.ToString()))
                return default(T);

            return property.GetValue<T>();
        }

        public bool HasProperty(string propertyAlias)
        {
            return GetProperty(propertyAlias) != null;
        }

        public bool HasValue(string propertyAlias)
        {
            var property = GetProperty(propertyAlias);
            if (property == null || property.Value == null)
                return false;

            return !string.IsNullOrEmpty(property.Value.ToString());
        }

        private ArchetypePropertyModel GetProperty(string propertyAlias)
        {
            return Properties.FirstOrDefault(p => p.Alias.InvariantEquals(propertyAlias));
        }

        #endregion

    }
}
