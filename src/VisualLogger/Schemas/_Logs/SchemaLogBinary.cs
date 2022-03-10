using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace VisualLogger.Schemas.Logs
{
    public class SchemaLogBinary :
        SchemaLog<SchemaLogBinary.SchemaBlockBinary,
            SchemaLogBinary.SchemaColumnHeadBinary,
            SchemaLogBinary.SchemaCellBinary>
    {
        #region Internal Class
        public class SchemaBlockBinary : SchemaBlock
        {
        }
        public class SchemaColumnHeadBinary : SchemaColumnHead
        {
            public string RowCount { get; set; } = string.Empty;
        }
        public class SchemaCellBinary : SchemaCell
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public SchemaLogBinaryType Type { get; set; }
            public int? Length { get; set; }
        }
        #endregion
        public override SchemaType Type => SchemaType.LogBinary;

        public SchemaLogBinary()
        {
        }

        public void SaveAsDefault()
        {
            Name = "schema_log_binary_rcv_windows_21.4.30";
            EncodingName = "utf-8";
            LogFileLoaderType = LogFileLoaderType.MemoryMapped;
            SupportedExtensions = new[] { "rcvlog" };
            var timeConvertor = new SchemaConvertor()
            {
                Name = "Time",
                Type = SchemaConvertorType.Math,
                Expression = "(long){Summary.StartTime}*1000+{Summary.StartTimeMS}+{CellValue}",
                ContinueWith = new SchemaConvertor()
                {
                    Type = SchemaConvertorType.Long2Time,
                    Expression = "yyyy-MM-dd HH:mm:ss,fff"
                }
            };
            var levelConvertor = new SchemaConvertor()
            {
                Name = "Level",
                Type = SchemaConvertorType.Enum,
                Expression = "0:DEBUG,1:INFO,2:WARNING,3:ERROR"
            };
            Convertors.Add(timeConvertor);
            Convertors.Add(levelConvertor);

            var header = new SchemaBlockBinary()
            {
                Name = "Header",
                Cells = new SchemaCellBinary[]
                {
                    new SchemaCellBinary{Name = "Signature",Type = SchemaLogBinaryType.StringWithLength,Length = 3},
                    //self.bom = bytes().join(struct.unpack('2c', buf[3:5])) self.os,self.logVersion,self.encode = struct.unpack('3c', buf[5:8])
                    new SchemaCellBinary{Type = SchemaLogBinaryType.Skip,Length = 2+3},
                    new SchemaCellBinary{Name = "MagicNumber",Type = SchemaLogBinaryType.Int},
                    new SchemaCellBinary{Name = "EncryptKey",Type = SchemaLogBinaryType.Int},
                    new SchemaCellBinary{Name = "SummarySize",Type = SchemaLogBinaryType.Int},
                    new SchemaCellBinary{Name = "FileMaxSize",Type = SchemaLogBinaryType.Int},
                    new SchemaCellBinary{Name = "FileCurSize",Type = SchemaLogBinaryType.Int},
                    new SchemaCellBinary{Type = SchemaLogBinaryType.Skip,Length = 32},
                }
            };
            Blocks.Add(header);

            var summary = new SchemaBlockBinary()
            {
                Name = "Summary",
                Cells = new SchemaCellBinary[]
                {
                    new SchemaCellBinary{Name = "TimeZone",Type = SchemaLogBinaryType.Int},
                    new SchemaCellBinary{Type = SchemaLogBinaryType.Skip,Length = 4},
                    new SchemaCellBinary{Name = "StartTime",Type = SchemaLogBinaryType.Long},
                    new SchemaCellBinary{Name = "StartTimeMS",Type = SchemaLogBinaryType.UInt},
                    new SchemaCellBinary{Name = "ProcessName",Type = SchemaLogBinaryType.StringWithLength,Length=256},
                    new SchemaCellBinary{Name = "ProcessId",Type = SchemaLogBinaryType.Int},
                    new SchemaCellBinary{Name = "ItemCount",Type = SchemaLogBinaryType.Int},
                    new SchemaCellBinary{Type = SchemaLogBinaryType.Skip,Length=32+4},
                }
            };
            Blocks.Add(summary);

            ColumnHeadTemplate = new SchemaColumnHeadBinary()
            {
                RowCount = $"{summary.Name}.{summary.Cells[6].Name}",
                Columns = new SchemaColumn[]
                {
                    new SchemaColumn
                    {
                        Cell=new SchemaCellBinary{Name = "TickOffset",Type = SchemaLogBinaryType.Long, ConvertorName=$"{timeConvertor.Name}"},
                    },
                    new SchemaColumn
                    {
                        Cell=new SchemaCellBinary{Name = "Module",Type = SchemaLogBinaryType.StringWithIntHead},
                        Filterable=true
                    },
                    new SchemaColumn
                    {
                        Cell=new SchemaCellBinary{Name = "Thread",Type = SchemaLogBinaryType.Int},
                        Filterable=true
                    },
                    new SchemaColumn
                    {
                        Cell=new SchemaCellBinary{Name = "Level",Type = SchemaLogBinaryType.Int, ConvertorName=$"{levelConvertor.Name}"},
                        Filterable=true
                    },
                    new SchemaColumn
                    {
                        Cell=new SchemaCellBinary{Name = "Hint",Type = SchemaLogBinaryType.StringWithIntHead},
                    },
                    new SchemaColumn
                    {
                        Cell=new SchemaCellBinary{Name = "Msg",Type = SchemaLogBinaryType.StringWithIntHead},
                        Enumeratable=true
                    }
                }
            };

            this.SaveAsJson($"schema_log.json");
        }
    }
}
