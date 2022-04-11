using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VisualLogger.InterfaceImplModules.LogContentLoaders.Text
{
    public class TextParser
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
            public string RegexPatternLineStart { get; set; }
            public string RegexPatternLineContent { get; set; }
            public string RegexPattern { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public PropertyParser[] Properties { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public SubObjectsParser SubObjects { get; set; }
        }
        public class PropertyParser
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Name { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int RegexGroupIndex { get; set; }
        }

        public string[] Columns { get; set; }
        public List<ObjectParser> Objects { get; set; } = new List<ObjectParser>();

        public TextParser()
        {
            Columns = new string[] { "Time", "Level", "Module", "Thread", "Msg" };
            var logFileHeader = new ObjectParser()
            {
                Name = "LogFileHeader",
                RegexPatternLineStart = @"app: (RoomsController)/([^\s]+)/(SHA)\(\)",
                RegexPatternLineContent = @"^(?!\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}).*",
                RegexPattern = @"app: (RoomsController)/([^\s]+)/([^\s]+)\(\)\r\nos: ([^\s]+)/([^\s]+)\r\n",
                Properties = new PropertyParser[]
                {
                    new PropertyParser{Name = "Type", RegexGroupIndex=1},
                    new PropertyParser{Name = "Version", RegexGroupIndex=2},
                    new PropertyParser{Name = "EncryptKey", RegexGroupIndex=3},
                    new PropertyParser{Name = "OS", RegexGroupIndex=4},
                    new PropertyParser{Name = "OSVersion", RegexGroupIndex=5},
                }
            };
            Objects.Add(logFileHeader); 
            
            var logItem = new ObjectParser()
            {
                Name = "logItems",
                RegexPatternLineStart = @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3})",
                RegexPatternLineContent = @"^(?!\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}).*",
                Properties = new PropertyParser[]
                {
                    new PropertyParser{Name = "Type", RegexGroupIndex=1},
                    new PropertyParser{Name = "Version", RegexGroupIndex=2},
                    new PropertyParser{Name = "EncryptKey", RegexGroupIndex=3},
                }
            };
            Objects.Add(logItem);

            //var logSummary = new ObjectParser()
            //{
            //    Name = "LogSummary",
            //    Properties = new PropertyParser[]
            //    {
            //        new PropertyParser{Name = "TimeZone",Type = BinaryType.Int},
            //        new PropertyParser{Type = BinaryType.Skip,Length = 4},
            //        new PropertyParser{Name = "StartTime",Type = BinaryType.Long},
            //        new PropertyParser{Name = "StartTimeMS",Type = BinaryType.UInt},
            //        new PropertyParser{Name = "ProcessName",Type = BinaryType.StringWithLength,Length=256},
            //        new PropertyParser{Name = "ProcessId",Type = BinaryType.Int},
            //        new PropertyParser{Name = "ItemCount",Type = BinaryType.Int},
            //        new PropertyParser{Type = BinaryType.Skip,Length=32+4},
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
            //            Name = "LogItem",
            //            Properties = new PropertyParser[]
            //            {
            //                new PropertyParser{Name = "TickOffset",Type = BinaryType.Long},
            //                new PropertyParser{Name = "ModuleName",Type = BinaryType.StringWithIntHead},
            //                new PropertyParser{Name = "ThreadId",Type = BinaryType.Int},
            //                new PropertyParser{Name = "Level",Type = BinaryType.Int},
            //                new PropertyParser{Name = "Hint",Type = BinaryType.StringWithIntHead},
            //                new PropertyParser{Name = "Msg",Type = BinaryType.StringWithIntHead},
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

        public static TextParser LoadFromJsonFile(string jsonFile)
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
                var binaryContentParser = JsonSerializer.Deserialize<TextParser>(jsonContent, options);
                return binaryContentParser;
            }
            catch
            {
                return null;
            }
        }
    }
}
