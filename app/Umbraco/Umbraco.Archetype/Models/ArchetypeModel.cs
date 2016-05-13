using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Archetype.Models
{
    /// <summary>
    /// Model that represents an entire Archetype.
    /// </summary>
    [JsonObject]
    public class ArchetypeModel : IEnumerable<ArchetypeFieldsetModel>
    {
        [JsonProperty("fieldsets")]
        public IEnumerable<ArchetypeFieldsetModel> Fieldsets { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchetypeModel"/> class.
        /// </summary>
        public ArchetypeModel()
        {
            Fieldsets = new List<ArchetypeFieldsetModel>();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of fieldsets.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ArchetypeFieldsetModel> GetEnumerator()
        {
            return this.Fieldsets.Where(f => f.IsAvailable()).GetEnumerator();
        }

        //possibly obsolete?
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Serializes for persistence. This should be used for serialization as it cleans up the JSON before saving.
        /// </summary>
        /// <returns></returns>
        public string SerializeForPersistence()
        {
            var json = JObject.Parse(JsonConvert.SerializeObject(this, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

            var propertiesToRemove = new String[] { "propertyEditorAlias", "dataTypeId", "dataTypeGuid", "hostContentType", "editorState" };

            json.Descendants().OfType<JProperty>()
              .Where(p => propertiesToRemove.Contains(p.Name))
              .ToList()
              .ForEach(x => x.Remove());

            return json.ToString(Formatting.None);
        }
    }
}
