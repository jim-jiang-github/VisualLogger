using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Core.Schemas
{
    public abstract class Schema : IJsonSerializable
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract SchemaType Type { get; }
        public static string? LoadContentFromJsonFile(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                Log.Warning("Not found json file in {jsonFilePath}.", jsonFilePath);
                return null;
            }
            var jsonContent = File.ReadAllText(jsonFilePath);
            return jsonContent;
        }
        public static T? LoadFromJsonFile<T>(string jsonFilePath)
            where T : class
        {
            var jsonContent = LoadContentFromJsonFile(jsonFilePath);
            if (jsonContent == null)
            {
                return null;
            }
            try
            {
                var binaryContentParser = JsonConvert.DeserializeObject<T>(jsonContent);
                return binaryContentParser;
            }
            catch (Exception ex)
            {
                Log.Information("Load error {error message}.", ex);
                return null;
            }
        }
        protected static T? GetAnonymousTypeFromJsonContent<T>(dynamic anonymousType, Func<string, T> deserializeAnonymousTypeCallback, string jsonContent)
        {
            try
            {
                return deserializeAnonymousTypeCallback.Invoke(jsonContent);
            }
            catch (Exception ex)
            {
                Log.Warning("Load error {error message}.", ex);
                return default(T);
            }
        }
        protected static T? GetAnonymousTypeFromJsonFile<T>(dynamic anonymousType, Func<string, T> deserializeAnonymousTypeCallback, string jsonFilePath)
        {
            var jsonContent = LoadContentFromJsonFile(jsonFilePath);
            var result = GetAnonymousTypeFromJsonContent(anonymousType, deserializeAnonymousTypeCallback, jsonContent);
            return result;
        }
        public static SchemaType GetSchemaTypeFromJsonFile(string jsonFilePath)
        {
            var anonymousType = new { Type = SchemaType.Unknow };
            var result = GetAnonymousTypeFromJsonFile(anonymousType, (c) =>
            {
                var x = JsonConvert.DeserializeAnonymousType(c, anonymousType);
                if (x == null)
                {
                    return SchemaType.Unknow;
                }
                return x.Type;
            }, jsonFilePath);
            return result;
        }
    }
}
