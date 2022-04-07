using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace VisualLogger.InterfaceImplModules.LogContentLoaders.Binary
{
    public class BinaryParser
    {
        private class LowerCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) => name.ToLower();
        }
        public class ArrayParser
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string LengthParser { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public ObjectParser ArrayItem { get; set; }
        }
        public class ObjectParser
        {
            public string Name { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public List<PropertyParser> Properties { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public ArrayParser Array { get; set; }
        }
        public class PropertyParser
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Name { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public BinaryType Type { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? Length { get; set; } = null;

            public override string ToString()
            {
                return $"{Name}-{Type}";
            }
        }

        public string[] Columns { get; set; }
        public List<ObjectParser> Objects { get; set; } = new List<ObjectParser>();

        public BinaryParser()
        {
            //Columns = new string[] { "Time", "Module", "Thread", "Level", "Hint", "Msg" };
            //var logFileHeader = new ObjectParser()
            //{
            //    Name = "LogFileHeader"
            //};
            //logFileHeader.Properties = new List<PropertyParser>();
            //logFileHeader.Properties.Add(new PropertyParser
            //{
            //    Name = "Signature",
            //    Type = BinaryType.StringUTF8,
            //    Length = 3
            //});
            ////self.bom = bytes().join(struct.unpack('2c', buf[3:5]))
            ////self.os,self.logVersion,self.encode = struct.unpack('3c', buf[5:8])
            //logFileHeader.Properties.Add(new PropertyParser
            //{
            //    Type = BinaryType.Skip,
            //    Length = 2 + 3
            //});
            //logFileHeader.Properties.Add(new PropertyParser
            //{
            //    Name = "MagicNumber",
            //    Type = BinaryType.Int
            //});
            //logFileHeader.Properties.Add(new PropertyParser
            //{
            //    Name = "EncryptKey",
            //    Type = BinaryType.Int
            //});
            //logFileHeader.Properties.Add(new PropertyParser
            //{
            //    Name = "SummarySize",
            //    Type = BinaryType.Int
            //});
            //logFileHeader.Properties.Add(new PropertyParser
            //{
            //    Name = "FileMaxSize",
            //    Type = BinaryType.Int
            //});
            //logFileHeader.Properties.Add(new PropertyParser
            //{
            //    Name = "FileCurSize",
            //    Type = BinaryType.Int
            //});
            //logFileHeader.Properties.Add(new PropertyParser
            //{
            //    Type = BinaryType.Skip,
            //    Length = 32
            //});

            //Objects.Add(logFileHeader);

            //var logSummary = new ObjectParser()
            //{
            //    Name = "LogSummary"
            //};
            //logSummary.Properties = new List<PropertyParser>();
            //logSummary.Properties.Add(new PropertyParser
            //{
            //    Name = "TimeZone",
            //    Type = BinaryType.Int
            //});
            //logSummary.Properties.Add(new PropertyParser
            //{
            //    Type = BinaryType.Skip,
            //    Length = 4
            //});
            //logSummary.Properties.Add(new PropertyParser
            //{
            //    Name = "StartTime",
            //    Type = BinaryType.Long
            //});
            //logSummary.Properties.Add(new PropertyParser
            //{
            //    Name = "StartTimeMS",
            //    Type = BinaryType.UInt
            //});
            //logSummary.Properties.Add(new PropertyParser
            //{
            //    Name = "ProcessName",
            //    Type = BinaryType.StringUTF8,
            //    Length = 256
            //});
            //logSummary.Properties.Add(new PropertyParser
            //{
            //    Name = "ProcessId",
            //    Type = BinaryType.Int
            //});
            //logSummary.Properties.Add(new PropertyParser
            //{
            //    Name = "ItemCount",
            //    Type = BinaryType.Int
            //});
            //logSummary.Properties.Add(new PropertyParser
            //{
            //    Type = BinaryType.Skip,
            //    Length = 32 + 4
            //});
            //Objects.Add(logSummary);

            //var arrayItem = new ObjectParser()
            //{
            //    Name = "LogItem"
            //};
            //arrayItem.Properties = new List<PropertyParser>();
            //arrayItem.Properties.Add(new PropertyParser
            //{
            //    Name = "TickOffset",
            //    Type = BinaryType.Long
            //});
            //arrayItem.Properties.Add(new PropertyParser
            //{
            //    Name = "ModuleName",
            //    Type = BinaryType.StringUTF8WithIntHead
            //});
            //arrayItem.Properties.Add(new PropertyParser
            //{
            //    Name = "ThreadId",
            //    Type = BinaryType.Int
            //});
            //arrayItem.Properties.Add(new PropertyParser
            //{
            //    Name = "Level",
            //    Type = BinaryType.Int
            //});
            //arrayItem.Properties.Add(new PropertyParser
            //{
            //    Name = "Hint",
            //    Type = BinaryType.StringUTF8WithIntHead
            //});
            //arrayItem.Properties.Add(new PropertyParser
            //{
            //    Name = "Msg",
            //    Type = BinaryType.StringUTF8WithIntHead
            //});

            //var array = new ObjectParser()
            //{
            //    Name = "LogContent",
            //    Array = new ArrayParser()
            //    {
            //        LengthParser = "Root.LogSummary.ItemCount",
            //        ArrayItem = arrayItem
            //    }
            //};
            //Objects.Add(array);

            //var options = new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = new LowerCaseNamingPolicy(),
            //    WriteIndented = true
            //};
            //var a = System.Text.Json.JsonSerializer.Serialize(this, options);
        }

        public static BinaryParser LoadFromJsonFile(string jsonFile)
        {
            if (!File.Exists(jsonFile))
            {
                return null;
            }
            var jsonContent = File.ReadAllText(jsonFile);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowerCaseNamingPolicy(),
                WriteIndented = true
            };
            try
            {
                var binaryParser = JsonSerializer.Deserialize<BinaryParser>(jsonContent, options);
                return binaryParser;
            }
            catch
            {
                return null;
            }
        }
    }
}
