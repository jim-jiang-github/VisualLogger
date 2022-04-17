using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Schemas
{
    public class Schema<T>
        where T : class
    {
        protected void SaveAsJson(string jsonFilePath)
        {
            var jsonContent = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            File.WriteAllText(jsonFilePath, jsonContent);
        }

        public static T? LoadFromJsonFile(string jsonFilePath, out string? errorMsg)
        {
            errorMsg = null;
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
                errorMsg = ex.Message;
                return null;
            }
        }
    }
}
