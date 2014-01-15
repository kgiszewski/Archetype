using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;

namespace Archetype.Umbraco.Models
{
    public class Fieldset
    {
        public string Alias { get; set; }
        public IEnumerable<Property> Properties;

        public Fieldset()
        {
            Properties = new List<Property>();
        }

        public string GetValue(string propertyAlias)
        {
            return GetValue<string>(propertyAlias);
        }

        public T GetValue<T>(string propertyAlias)
        {
            var property = GetProperty(propertyAlias);

            if (property == null || string.IsNullOrEmpty(property.Value))
                return default(T);

            var convertAttempt = property.Value.TryConvertTo<T>();

            if (convertAttempt.Success)
                return convertAttempt.Result;

            return default(T);
        }

        private Property GetProperty(string propertyAlias)
        {
            return Properties.FirstOrDefault(p => p.Alias.InvariantEquals(propertyAlias));
        }

    }
}
