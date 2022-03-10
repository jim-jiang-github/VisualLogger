using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Extensions
{
    /// <summary>
    /// Implement save method in VisualLogger.Extensions.SerializeExtension
    /// </summary>
    internal interface IJsonSerializable
    {
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
    }
}
