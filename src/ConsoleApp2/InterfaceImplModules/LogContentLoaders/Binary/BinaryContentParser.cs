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
    public class BinaryContentParser
    {
        private class LowerCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) => name.ToLower();
        }
        public class Block
        {
            public string Name { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public Cell[] Cells { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public Items Items { get; set; }
        }
        public class Cell
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Name { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public BinaryType Type { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? Length { get; set; }
        }
        public class Items
        {
            public string Count { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public Cell[] CellsTemplate { get; set; }
        }

        public string LogItemsPath { get; set; }
        public List<Block> Blocks { get; set; } = new List<Block>();

        public BinaryContentParser()
        {
            //var header = new Block()
            //{
            //    Name = "Header",
            //    Cells = new Cell[]
            //    {
            //        new Cell{Name = "Signature",Type = BinaryType.StringWithLength,Length = 3},
            //        //self.bom = bytes().join(struct.unpack('2c', buf[3:5])) self.os,self.logVersion,self.encode = struct.unpack('3c', buf[5:8])
            //        new Cell{Type = BinaryType.Skip,Length = 2+3},
            //        new Cell{Name = "MagicNumber",Type = BinaryType.Int},
            //        new Cell{Name = "EncryptKey",Type = BinaryType.Int},
            //        new Cell{Name = "SummarySize",Type = BinaryType.Int},
            //        new Cell{Name = "FileMaxSize",Type = BinaryType.Int},
            //        new Cell{Name = "FileCurSize",Type = BinaryType.Int},
            //        new Cell{Type = BinaryType.Skip,Length = 32},
            //    }
            //};
            //Blocks.Add(header);

            //var summary = new Block()
            //{
            //    Name = "Summary",
            //    Cells = new Cell[]
            //    {
            //        new Cell{Name = "TimeZone",Type = BinaryType.Int},
            //        new Cell{Type = BinaryType.Skip,Length = 4},
            //        new Cell{Name = "StartTime",Type = BinaryType.Long},
            //        new Cell{Name = "StartTimeMS",Type = BinaryType.UInt},
            //        new Cell{Name = "ProcessName",Type = BinaryType.StringWithLength,Length=256},
            //        new Cell{Name = "ProcessId",Type = BinaryType.Int},
            //        new Cell{Name = "ItemCount",Type = BinaryType.Int},
            //        new Cell{Type = BinaryType.Skip,Length=32+4},
            //    }
            //};
            //Blocks.Add(summary);

            //var content = new Block()
            //{
            //    Name = "Content",
            //    Items = new Items()
            //    {
            //        Count = $"{summary.Name}.{summary.Cells[6].Name}",
            //        CellsTemplate = new Cell[]
            //        {
            //            new Cell{Name = "Time",Type = BinaryType.Long},
            //            new Cell{Name = "Module",Type = BinaryType.StringWithIntHead},
            //            new Cell{Name = "Thread",Type = BinaryType.Int},
            //            new Cell{Name = "Level",Type = BinaryType.Int},
            //            new Cell{Name = "Hint",Type = BinaryType.StringWithIntHead},
            //            new Cell{Name = "Msg",Type = BinaryType.StringWithIntHead},
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

        public static BinaryContentParser LoadFromJsonFile(string jsonFile)
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
                var binaryContentParser = JsonSerializer.Deserialize<BinaryContentParser>(jsonContent, options);
                return binaryContentParser;
            }
            catch
            {
                return null;
            }
        }
    }
}
