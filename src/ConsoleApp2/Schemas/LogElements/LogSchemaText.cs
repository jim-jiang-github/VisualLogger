using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VisualLogger.Schemas.LogElements
{
    public class LogSchemaText :
        LogSchema<LogSchemaText,
            LogSchemaText.BlockText,
            LogSchemaText.BodyText,
            LogSchemaText.CellText>
    {
        #region Internal Class
        public class BlockText : BlockSchema
        {
            public string? RegexStart { get; set; }
            public string? RegexEnd { get; set; }
            public string? RegexContent { get; set; }
        }
        public class BodyText : BodySchema
        {
            public string? RegexStart { get; set; }
            public string? RegexEnd { get; set; }
            public string? RegexContent { get; set; }
        }
        public class CellText : CellSchema
        {
            public int RegexGroupIndex { get; set; }
        }
        #endregion


        public LogSchemaText()
        {
            //Name = "Rcv Android log";
            //ExtensionNames = new string[] { "txt", "log" };
            //var header = new BlockText()
            //{
            //    Name = "Header",
            //    RegexStart = @"app: (RoomsController)/(.*?)/(SHA\(.*\))",
            //    RegexEnd = @"^(?!\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}).*",
            //    RegexContent = @"app: (RoomsController)/(.*?)/(SHA\(.*\))[\r|\n]*os: (.*?)/(.*?)[\r|\n]",
            //    Cells = new CellText[]
            //    {
            //        new CellText{Name = "Type", RegexGroupIndex=1},
            //        new CellText{Name = "Version", RegexGroupIndex=2},
            //        new CellText{Name = "EncryptKey", RegexGroupIndex=3},
            //        new CellText{Name = "OS", RegexGroupIndex=4},
            //        new CellText{Name = "OSVersion", RegexGroupIndex=5},
            //    }
            //};
            //Blocks.Add(header);

            //Body = new BodyText
            //{
            //    RegexStart = @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3})",
            //    RegexEnd = @"^(?!\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}).*",
            //    RegexContent = @"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}) : (.*?) : \[(.*?)\] \[(.*?)\] (.*)",
            //    BodyTemplate = new CellText[]
            //    {
            //        new CellText{Name = "Time", RegexGroupIndex=1},
            //        new CellText{Name = "Level", RegexGroupIndex=2},
            //        new CellText{Name = "Module", RegexGroupIndex=3},
            //        new CellText{Name = "Thread", RegexGroupIndex=4},
            //        new CellText{Name = "Msg", RegexGroupIndex=5},
            //    }
            //};

            //SaveAsJson("LogSchemaText.json");
        }
    }
}
