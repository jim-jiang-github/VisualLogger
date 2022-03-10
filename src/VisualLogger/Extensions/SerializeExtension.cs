using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Extensions
{
    internal static class SerializeExtension
    {
        public static void SaveAsJson(this IJsonSerializable jsonSerializable, string jsonFilePath)
        {
            var jsonContent = JsonConvert.SerializeObject(jsonSerializable, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            File.WriteAllText(jsonFilePath, jsonContent);
        }
    }
}
