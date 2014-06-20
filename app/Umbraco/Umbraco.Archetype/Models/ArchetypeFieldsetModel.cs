using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Umbraco.Core;

namespace Archetype.Models
{
    public class ArchetypeFieldsetModel
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("properties")]
        public IEnumerable<ArchetypePropertyModel> Properties;

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

			if (IsEmptyProperty(property)) 
			{
				return default(T);
			}

            return property.GetValue<T>();
        }

		// issue 142: support default T value supplied by caller
		// this code would look nicer if the two GetValue<T>() methods had one common implementation.
		// however, this would require GetValue<T>(string propertyAlias) to call the common implementation
		// with a default(T) value, which could in theory result in a performance hit, if T for some reason
		// is costly to instantiate.
		public T GetValue<T>(string propertyAlias, T defaultValue)
        {
            var property = GetProperty(propertyAlias);

			if (IsEmptyProperty(property)) 
			{
				return defaultValue;
			}

            return property.GetValue<T>();
        }

		private bool IsEmptyProperty(ArchetypePropertyModel property) 
		{
			return (property == null || property.Value == null || string.IsNullOrEmpty(property.Value.ToString()));
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