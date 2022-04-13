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
    public class TextContentParser
    {
        private class LowerCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) => name.ToLower();
        }
        public class Block
        {
            public string Name { get; set; }
            public string RegexPatternItemStart { get; set; }
            public string RegexPatternItemContent { get; set; }
            public string RegexPattern { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public Cell[] Cells { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public Items Items { get; set; }
        }
        public class Cell
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Name { get; set; }
            public int RegexGroupIndex { get; set; }
        }
        public class Items
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public Cell[] CellsTemplate { get; set; }
        }

        public string LogItemsPath { get; set; }
        public List<Block> Blocks { get; set; } = new List<Block>();

        public TextContentParser()
        {
            //var header = new Block()
            //{
            //    Name = "Header",
            //    RegexPatternItemStart = @"app: (RoomsController)/(.*?)/(SHA)\(\)",
            //    RegexPatternItemContent = @"^(?!\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}).*",
            //    RegexPattern = @"app: (RoomsController)/(.*?)/(.*?)\(\)[\r|\n]*os: (.*?)/(.*?)[\r|\n]",
            //    Cells = new Cell[]
            //    {
            //        new Cell{Name = "Type", RegexGroupIndex=1},
            //        new Cell{Name = "Version", RegexGroupIndex=2},
            //        new Cell{Name = "EncryptKey", RegexGroupIndex=3},
            //        new Cell{Name = "OS", RegexGroupIndex=4},
            //        new Cell{Name = "OSVersion", RegexGroupIndex=5},
            //    }
            //};
            //Blocks.Add(header);

            //var content = new Block()
            //{
            //    Name = "Content",
            //    RegexPatternItemStart = @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3})",
            //    RegexPatternItemContent = @"^(?!\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}).*",
            //    RegexPattern = @"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}) : (.*?) : \[(.*?)\] \[(.*?)\] (.*)",
            //    Items = new Items
            //    {
            //        CellsTemplate = new Cell[]
            //        {
            //            new Cell{Name = "Time", RegexGroupIndex=1},
            //            new Cell{Name = "Level", RegexGroupIndex=2},
            //            new Cell{Name = "Module", RegexGroupIndex=3},
            //            new Cell{Name = "Thread", RegexGroupIndex=4},
            //            new Cell{Name = "Msg", RegexGroupIndex=5},
            //        }
            //    }
            //};
            //Blocks.Add(content);
            //LogItemsPath = $"{content.Name}";

            //var options = new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = new LowerCaseNamingPolicy(),
            //    WriteIndented = true
            //};
            //var json = System.Text.Json.JsonSerializer.Serialize(this, options);
        }

        public static TextContentParser LoadFromJsonFile(string jsonFile)
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
                var binaryContentParser = JsonSerializer.Deserialize<TextContentParser>(jsonContent, options);
                return binaryContentParser;
            }
            catch
            {
                return null;
            }
        }
    }
}
