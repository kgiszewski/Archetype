using Archetype.Umbraco.PropertyConverters;
using at = Archetype.Umbraco.Models;
using Newtonsoft.Json;

namespace Archetype.Tests.Serialization.Base
{
    public abstract class ArchetypeJsonConverterTestBase
    {
        public at.Archetype ConvertModelToArchetype<T>(T model)
        {
            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(model);
            var archetype = (Umbraco.Models.Archetype)converter.ConvertDataToSource(null, json, false);
            return archetype;
        }

        public string ConvertModelToArchetypeJson<T>(T model, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(model, formatting);
        }

        public T ConvertArchetypeJsonToModel<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public T ConvertModelToArchetypeAndBack<T>(T model)
        {
            var json = ConvertModelToArchetypeJson(model);
            return ConvertArchetypeJsonToModel<T>(json);
        }
    }
}
