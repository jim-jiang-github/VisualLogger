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

        public static T? LoadFromJsonFile<T>(string jsonFilePath)
            where T : class
        {
            if (!File.Exists(jsonFilePath))
            {
                return null;
            }
            var jsonContent = File.ReadAllText(jsonFilePath);
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
        public static SchemaType GetSchemaTypeFromJsonFile(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                return SchemaType.Unknow;
            }
            var jsonContent = File.ReadAllText(jsonFilePath);
            try
            {
                var anonymousType = new { Type = "" };
                var x = JsonConvert.DeserializeAnonymousType(jsonContent, anonymousType);
                if (x == null)
                {
                    return SchemaType.Unknow;
                }
                var schemaType = (SchemaType)Enum.Parse(typeof(SchemaType), x.Type, false);
                return schemaType;
            }
            catch (Exception ex)
            {
                Log.Information("Load error {error message}.", ex);
                return SchemaType.Unknow;
            }
        }
    }
}
