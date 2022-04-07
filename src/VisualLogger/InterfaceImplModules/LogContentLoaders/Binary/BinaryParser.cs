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
        public class SubObjectsParser
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string LengthParser { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public ObjectParser Object { get; set; }
        }
        public class ObjectParser
        {
            public string Name { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public PropertyParser[] Properties { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public SubObjectsParser SubObjects { get; set; }
        }
        public class PropertyParser
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Name { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public BinaryType Type { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Parameter { get; set; }

            public override string ToString()
            {
                return $"Name:{Name}-Type:{Type}";
            }
        }

        public string[] Columns { get; set; }
        public List<ObjectParser> Objects { get; set; } = new List<ObjectParser>();

        public BinaryParser()
        {
            //Columns = new string[] { "Time", "Module", "Thread", "Level", "Hint", "Msg" };
            //var logFileHeader = new ObjectParser()
            //{
            //    Name = "LogFileHeader",
            //    Properties = new PropertyParser[]
            //    {
            //        new PropertyParser{Name = "Signature",Type = BinaryType.String,Parameter = "3"},
            //        //self.bom = bytes().join(struct.unpack('2c', buf[3:5])) self.os,self.logVersion,self.encode = struct.unpack('3c', buf[5:8])
            //        new PropertyParser{Type = BinaryType.Skip,Parameter = $"{2+3}"},
            //        new PropertyParser{Name = "MagicNumber",Type = BinaryType.Int},
            //        new PropertyParser{Name = "EncryptKey",Type = BinaryType.Int},
            //        new PropertyParser{Name = "SummarySize",Type = BinaryType.Int},
            //        new PropertyParser{Name = "FileMaxSize",Type = BinaryType.Int},
            //        new PropertyParser{Name = "FileCurSize",Type = BinaryType.Int},
            //        new PropertyParser{Type = BinaryType.Skip,Parameter = "32"},
            //    }
            //};
            //Objects.Add(logFileHeader);

            //var logSummary = new ObjectParser()
            //{
            //    Name = "LogSummary",
            //    Properties = new PropertyParser[]
            //    {
            //        new PropertyParser{Name = "TimeZone",Type = BinaryType.Int},
            //        new PropertyParser{Type = BinaryType.Skip,Parameter = "4"},
            //        new PropertyParser{Name = "StartTime",Type = BinaryType.Long},
            //        new PropertyParser{Name = "StartTimeMS",Type = BinaryType.UInt},
            //        new PropertyParser{Name = "ProcessName",Type = BinaryType.String,Parameter="256"},
            //        new PropertyParser{Name = "ProcessId",Type = BinaryType.Int},
            //        new PropertyParser{Name = "ItemCount",Type = BinaryType.Int},
            //        new PropertyParser{Type = BinaryType.Skip,Parameter=$"{32+4}"},
            //    }
            //};
            //Objects.Add(logSummary);

            //var logItems = new ObjectParser()
            //{
            //    Name = "LogItems",
            //    SubObjects = new SubObjectsParser()
            //    {
            //        LengthParser = "Root.LogSummary.ItemCount",
            //        Object = new ObjectParser()
            //        {
            //            Name = "LogContent",
            //            Properties = new PropertyParser[]
            //            {
            //                new PropertyParser{Name = "TickOffset",Type = BinaryType.Long},
            //                new PropertyParser{Name = "ModuleName",Type = BinaryType.String,Parameter="HeadInt"},
            //                new PropertyParser{Name = "ThreadId",Type = BinaryType.Int},
            //                new PropertyParser{Name = "Level",Type = BinaryType.Int},
            //                new PropertyParser{Name = "Hint",Type = BinaryType.String,Parameter="HeadInt"},
            //                new PropertyParser{Name = "Msg",Type = BinaryType.String,Parameter="HeadInt"},
            //            }
            //        }
            //    }
            //};
            //Objects.Add(logItems);

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
