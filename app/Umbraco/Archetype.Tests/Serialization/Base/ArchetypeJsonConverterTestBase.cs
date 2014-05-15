using Archetype.PropertyConverters;
using at = Archetype.Models;
using Newtonsoft.Json;

namespace Archetype.Tests.Serialization.Base
{
    public abstract class ArchetypeJsonConverterTestBase
    {
        public at.ArchetypeModel ConvertModelToArchetype<T>(T model)
        {
            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(model);
            var archetype = (at.ArchetypeModel)converter.ConvertDataToSource(null, json, false);
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
