using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using VisualLogger.Datas;
using VisualLogger.Schemas.Convertors;

namespace VisualLogger.Schemas.Logs
{
    public class LogSchemaBinary :
        LogSchema<LogSchemaBinary,
            LogSchemaBinary.BlockBinary,
            LogSchemaBinary.BodyBinary,
            LogSchemaBinary.CellBinary>
    {
        #region Internal Class
        public class BlockBinary : BlockSchema
        {
        }
        public class BodyBinary : BodySchema
        {
            public string? Count { get; set; }
        }
        public class CellBinary : CellSchema
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public LogSchemaBinaryType Type { get; set; }
            public int? Length { get; set; }
        }
        #endregion
        public LogSchemaBinary()
        {
            //Name = "Rcv Windows log";
            //ExtensionNames = new string[] { "rcvlog" };
            //var timeConvertor = new ConvertorSchema()
            //{
            //    Name = "Time",
            //    Type = ConvertorSchemaType.Math,
            //    Expression = "(long){Summary.StartTime}*1000+{Summary.StartTimeMS}+{CellValue}",
            //    ContinueConvertor = new ConvertorSchema()
            //    {
            //        Type = ConvertorSchemaType.Time,
            //        Expression = "yyyy-MM-dd HH:mm:ss,fff"
            //    }
            //};
            //var levelConvertor = new ConvertorSchema()
            //{
            //    Name = "Level",
            //    Type = ConvertorSchemaType.Enum,
            //    Expression = "0:DEBUG,1:INFO,2:WARNING,3:ERROR"
            //};
            //Convertors.Add(timeConvertor);
            //Convertors.Add(levelConvertor);

            //var header = new BlockBinary()
            //{
            //    Name = "Header",
            //    Cells = new CellBinary[]
            //    {
            //        new CellBinary{Name = "Signature",Type = LogSchemaBinaryType.StringWithLength,Length = 3},
            //        //self.bom = bytes().join(struct.unpack('2c', buf[3:5])) self.os,self.logVersion,self.encode = struct.unpack('3c', buf[5:8])
            //        new CellBinary{Type = LogSchemaBinaryType.Skip,Length = 2+3},
            //        new CellBinary{Name = "MagicNumber",Type = LogSchemaBinaryType.Int},
            //        new CellBinary{Name = "EncryptKey",Type = LogSchemaBinaryType.Int},
            //        new CellBinary{Name = "SummarySize",Type = LogSchemaBinaryType.Int},
            //        new CellBinary{Name = "FileMaxSize",Type = LogSchemaBinaryType.Int},
            //        new CellBinary{Name = "FileCurSize",Type = LogSchemaBinaryType.Int},
            //        new CellBinary{Type = LogSchemaBinaryType.Skip,Length = 32},
            //    }
            //};
            //Blocks.Add(header);

            //var summary = new BlockBinary()
            //{
            //    Name = "Summary",
            //    Cells = new CellBinary[]
            //    {
            //        new CellBinary{Name = "TimeZone",Type = LogSchemaBinaryType.Int},
            //        new CellBinary{Type = LogSchemaBinaryType.Skip,Length = 4},
            //        new CellBinary{Name = "StartTime",Type = LogSchemaBinaryType.Long},
            //        new CellBinary{Name = "StartTimeMS",Type = LogSchemaBinaryType.UInt},
            //        new CellBinary{Name = "ProcessName",Type = LogSchemaBinaryType.StringWithLength,Length=256},
            //        new CellBinary{Name = "ProcessId",Type = LogSchemaBinaryType.Int},
            //        new CellBinary{Name = "ItemCount",Type = LogSchemaBinaryType.Int},
            //        new CellBinary{Type = LogSchemaBinaryType.Skip,Length=32+4},
            //    }
            //};
            //Blocks.Add(summary);

            //Body = new BodyBinary()
            //{
            //    Count = $"{summary.Name}.{summary.Cells[6].Name}",
            //    BodyTemplate = new CellBinary[]
            //    {
            //        new CellBinary{Name = "TickOffset",Type = LogSchemaBinaryType.Long, ConvertorName=$"{timeConvertor.Name}"},
            //        new CellBinary{Name = "Module",Type = LogSchemaBinaryType.StringWithIntHead},
            //        new CellBinary{Name = "Thread",Type = LogSchemaBinaryType.Int},
            //        new CellBinary{Name = "Level",Type = LogSchemaBinaryType.Int, ConvertorName=$"{levelConvertor.Name}"},
            //        new CellBinary{Name = "Hint",Type = LogSchemaBinaryType.StringWithIntHead},
            //        new CellBinary{Name = "Msg",Type = LogSchemaBinaryType.StringWithIntHead},
            //    }
            //};

            //SaveAsJson("LogSchemaBinary.json");
        }
    }
}
