using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Schemas.Convertors
{
    public class ConvertorSchema
    {
        public string? Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ConvertorSchemaType Type { get; set; }
        public string? Expression { get; set; }
        public ConvertorSchema? ContinueConvertor { get; set; }
    }
}
