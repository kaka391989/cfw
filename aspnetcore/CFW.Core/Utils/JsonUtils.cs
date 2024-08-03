using Newtonsoft.Json;

namespace CFW.Core.Utils
{
    public static class JsonUtils
    {
        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        public static string ToJson(this object? obj) => JsonConvert.SerializeObject(obj, _serializerSettings);

        public static string ToJsonWithCamelCase(this object? obj) => JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        });


        public static IEnumerable<T> Clone<T>(this IEnumerable<T> source)
        {
            var sourceStr = JsonConvert.SerializeObject(source, _serializerSettings);
            var result = JsonConvert.DeserializeObject<IEnumerable<T>>(sourceStr, _serializerSettings);
            return result!;
        }

        public static T Clone<T>(this T source)
        {
            var sourceStr = JsonConvert.SerializeObject(source, _serializerSettings);
            var result = JsonConvert.DeserializeObject<T>(sourceStr, _serializerSettings);
            return result!;
        }

        public static TTarget Convert<TTarget>(this object source)
        {
            if (source is TTarget target)
            {
                return target;
            }

            var sourceStr = JsonConvert.SerializeObject(source, _serializerSettings);
            var result = JsonConvert.DeserializeObject<TTarget>(sourceStr, _serializerSettings);
            return result!;
        }
    }
}
