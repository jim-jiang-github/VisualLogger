using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VisualLogger.Core.Schemas.Logs
{
    public class SchemaLogText :
        SchemaLog<SchemaLogText.SchemaBlockText,
            SchemaLogText.SchemaColumnHeadText,
            SchemaLogText.SchemaCellText>
    {
        #region Internal Class
        public class SchemaBlockText : SchemaBlock
        {
            public string RegexStart { get; set; } = string.Empty;
            public string RegexEnd { get; set; } = string.Empty;
            public string RegexContent { get; set; } = string.Empty;
        }
        public class SchemaColumnHeadText : SchemaColumnHead
        {
            public string RegexStart { get; set; } = string.Empty;
            public string RegexEnd { get; set; } = string.Empty;
            public string RegexContent { get; set; } = string.Empty;
        }
        public class SchemaCellText : SchemaCell
        {
            public int RegexGroupIndex { get; set; }
        }
        #endregion
        public override SchemaType Type => SchemaType.LogText;
        public SchemaLogText()
        {
        }
        public void SaveAsDefault()
        {
            Name = "schema_log_text_rcv_windows_22.2.20";
            LoaderType = LogFileLoaderType.Txt;
            AvailableExtensions = new string[] { ".txt", ".log" };
            var timeConvertor = new SchemaConvertor()
            {
                Name = "Time",
                Type = SchemaConvertorType.Time2Time,
                Expression = "[MM/dd/yy HH:mm:ss.fff][yyyy-MM-dd HH:mm:ss,fff]"
            };
            Convertors.Add(timeConvertor);
            ColumnHeadTemplate = new SchemaColumnHeadText
            {
                RegexStart = @"^(\d{2}\/\d{2}\/\d{2} \d{2}:\d{2}:\d{2}.\d{3})",
                RegexEnd = @"^(?!\d{2}\/\d{2}\/\d{2} \d{2}:\d{2}:\d{2}.\d{3}).*",
                RegexContent = @"^(\d{2}\/\d{2}\/\d{2} \d{2}:\d{2}:\d{2}.\d{3}) \<(.*?)\> \[(.*?)\] (.*?) (.*)",
                Cells = new SchemaCellText[]
                {
                    new SchemaCellText{Name = "Time", RegexGroupIndex=1, ConvertorName="Time"},
                    new SchemaCellText{Name = "Module", RegexGroupIndex=2},
                    new SchemaCellText{Name = "Thread", RegexGroupIndex=3},
                    new SchemaCellText{Name = "Level", RegexGroupIndex=4},
                    new SchemaCellText{Name = "Msg", RegexGroupIndex=5},
                }
            };
            this.SaveAsJson($"{Name}.json");
        }
        public void SaveAsDefault1()
        {
            Name = "schema_log_text_rcv_android_22.2.20";
            LoaderType = LogFileLoaderType.Txt;
            AvailableExtensions = new string[] { ".txt", ".log" };
            var header = new SchemaBlockText()
            {
                Name = "Header",
                RegexStart = @"app: (RoomsController)/(.*?)/(SHA\(.*\))",
                RegexEnd = @"^(?!\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}).*",
                RegexContent = @"app: (RoomsController)/(.*?)/(SHA\(.*\))[\r|\n]*os: (.*?)/(.*?)[\r|\n]",
                Cells = new SchemaCellText[]
                {
                    new SchemaCellText{Name = "Type", RegexGroupIndex=1},
                    new SchemaCellText{Name = "Version", RegexGroupIndex=2},
                    new SchemaCellText{Name = "EncryptKey", RegexGroupIndex=3},
                    new SchemaCellText{Name = "OS", RegexGroupIndex=4},
                    new SchemaCellText{Name = "OSVersion", RegexGroupIndex=5},
                }
            };
            Blocks.Add(header);

            ColumnHeadTemplate = new SchemaColumnHeadText
            {
                RegexStart = @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3})",
                RegexEnd = @"^(?!\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}).*",
                RegexContent = @"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}) : (.*?) : \[(.*?)\] \[(.*?)\] (.*)",
                Cells = new SchemaCellText[]
                {
                    new SchemaCellText{Name = "Time", RegexGroupIndex=1},
                    new SchemaCellText{Name = "Level", RegexGroupIndex=2},
                    new SchemaCellText{Name = "Module", RegexGroupIndex=3},
                    new SchemaCellText{Name = "Thread", RegexGroupIndex=4},
                    new SchemaCellText{Name = "Msg", RegexGroupIndex=5},
                }
            };

            this.SaveAsJson("log_schema_text_default.json");
        }
    }
}
