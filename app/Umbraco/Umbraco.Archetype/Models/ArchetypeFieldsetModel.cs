using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Umbraco.Core;
using System;

namespace Archetype.Models
{
    /// <summary>
    /// Model that represents a fieldset stored as content JSON.
    /// </summary>
    public class ArchetypeFieldsetModel
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("properties")]
        public IEnumerable<ArchetypePropertyModel> Properties;

        [JsonProperty("id")]
        public Guid Id { get; set; }

		[JsonProperty("releaseDate")]
		public DateTime? ReleaseDate { get; set; }

		[JsonProperty("expireDate")]
		public DateTime? ExpireDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchetypeFieldsetModel"/> class.
        /// </summary>
        public ArchetypeFieldsetModel()
        {
            Properties = new List<ArchetypePropertyModel>();
        }

        #region Helper Methods

        /// <summary>
        /// Gets the value and returns as string.
        /// </summary>
        /// <param name="propertyAlias">The property alias.</param>
        /// <returns></returns>
        public string GetValue(string propertyAlias)
        {
            return GetValue<string>(propertyAlias);
        }

        /// <summary>
        /// Gets the value based on the type passed in as a generic.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyAlias">The property alias.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyAlias">The property alias.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public T GetValue<T>(string propertyAlias, T defaultValue)
        {
            var property = GetProperty(propertyAlias);

            if (IsEmptyProperty(property)) 
            {
                return defaultValue;
            }

            return property.GetValue<T>();
        }

        /// <summary>
        /// Determines whether the property is empty.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        private bool IsEmptyProperty(ArchetypePropertyModel property) 
        {
            return (property == null || property.Value == null || string.IsNullOrEmpty(property.Value.ToString()));
        }

        /// <summary>
        /// Determines whether the specified property alias has property.
        /// </summary>
        /// <param name="propertyAlias">The property alias.</param>
        /// <returns></returns>
        public bool HasProperty(string propertyAlias)
        {
            return GetProperty(propertyAlias) != null;
        }

        /// <summary>
        /// Determines whether the specified property alias has value.
        /// </summary>
        /// <param name="propertyAlias">The property alias.</param>
        /// <returns></returns>
        public bool HasValue(string propertyAlias)
        {
            var property = GetProperty(propertyAlias);
            if (property == null || property.Value == null)
                return false;

            return !string.IsNullOrEmpty(property.Value.ToString());
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyAlias">The property alias.</param>
        /// <returns></returns>
        private ArchetypePropertyModel GetProperty(string propertyAlias)
        {
            return Properties.FirstOrDefault(p => p.Alias.InvariantEquals(propertyAlias));
        }

        #endregion
    }
}
