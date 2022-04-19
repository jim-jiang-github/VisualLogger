using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.InterfaceModules;
using VisualLogger.Schemas.Convertors;

namespace VisualLogger.Schemas.LogElements
{
    /// <summary>
    /// LogSchema|
    ///          |Block1|     
    ///          |      |Cell1,Cell2...CellN
    ///          |Block2|     
    ///          .      |Cell1,Cell2...CellN
    ///          .
    ///          . 
    ///          |BlockN|     
    ///                 |Cell1,Cell2...CellN
    ///          |Body|     
    ///               |TemplateCell1,TemplateCell12...TemplateCell1N
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <typeparam name="TBlock"></typeparam>
    /// <typeparam name="TBody"></typeparam>
    /// <typeparam name="TCell"></typeparam>
    public class LogSchema<TSelf, TBlock, TBody, TCell> : Schema<TSelf>,
        ILogSchemaLoader
        where TSelf : LogSchema<TSelf, TBlock, TBody, TCell>, new()
        where TBlock : LogSchema<TSelf, TBlock, TBody, TCell>.BlockSchema, new()
        where TBody : LogSchema<TSelf, TBlock, TBody, TCell>.BodySchema, new()
        where TCell : LogSchema<TSelf, TBlock, TBody, TCell>.CellSchema, new()
    {
        #region Internal Class
        public class BlockSchema
        {
            public string Name { get; set; } = string.Empty;
            public TCell[] Cells { get; set; } = Array.Empty<TCell>();
        }
        public class BodySchema
        {
            public TCell[] BodyTemplate { get; set; } = Array.Empty<TCell>();
        }
        public class CellSchema
        {
            public string Name { get; set; } = string.Empty;
            public string? ConvertorName { get; set; }
        }
        #endregion

        public string? Name { get; set; }
        public string[]? ExtensionNames { get; set; }
        public List<ConvertorSchema> Convertors { get; } = new List<ConvertorSchema>();
        public List<TBlock> Blocks { get; } = new List<TBlock>();
        public TBody Body { get; set; } = new TBody();
    }
}
