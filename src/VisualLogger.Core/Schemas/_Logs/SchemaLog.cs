using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Core.Schemas.Logs
{
    public abstract class SchemaLog : Schema
    {
        #region Internal Class
        public class SchemaConvertor
        {
            public string? Name { get; set; }
            [JsonConverter(typeof(StringEnumConverter))]
            public SchemaConvertorType Type { get; set; }
            public string? Expression { get; set; }
            public SchemaConvertor? ContinueWith { get; set; }
        }
        #endregion
        public string Name { get; set; } = string.Empty;
        [JsonConverter(typeof(StringEnumConverter))]
        public LogFileLoaderType LoaderType { get; set; } = LogFileLoaderType.Unknow;
        public string[] AvailableExtensions { get; set; } = Array.Empty<string>();
        public List<SchemaConvertor> Convertors { get; } = new List<SchemaConvertor>();

        public static string[] GetAvailableExtensionsFromJsonFile(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                return Array.Empty<string>();
            }
            var jsonContent = File.ReadAllText(jsonFilePath);
            try
            {
                var anonymousType = new { AvailableExtensions = Array.Empty<string>() };
                var x = JsonConvert.DeserializeAnonymousType(jsonContent, anonymousType);
                if (x == null)
                {
                    return Array.Empty<string>();
                }
                return x.AvailableExtensions;
            }
            catch (Exception ex)
            {
                Log.Information("Load error {error message}.", ex);
                return Array.Empty<string>();
            }
        }
    }
    /// <summary>
    /// SchemaLog|
    ///          |Block1|     
    ///          |      |Cell1,Cell2...CellN
    ///          |Block2|     
    ///          .      |Cell1,Cell2...CellN
    ///          .
    ///          . 
    ///          |BlockN|     
    ///                 |Cell1,Cell2...CellN
    ///          |ColumnHeadTemplate|     
    ///                 |Cell1,Cell2...CellN
    /// </summary>
    /// <typeparam name="TBlockSchema"></typeparam>
    /// <typeparam name="TColumnHeadSchema"></typeparam>
    /// <typeparam name="TCellSchema"></typeparam>
    public abstract class SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema> : SchemaLog
        where TBlockSchema : SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema>.SchemaBlock, new()
        where TColumnHeadSchema : SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema>.SchemaColumnHead, new()
        where TCellSchema : SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema>.SchemaCell, new()
    {
        #region Internal Class
        public class SchemaBlock
        {
            public string Name { get; set; } = string.Empty;
            public TCellSchema[] Cells { get; set; } = Array.Empty<TCellSchema>();
        }
        public class SchemaColumnHead
        {
            public TCellSchema[] Cells { get; set; } = Array.Empty<TCellSchema>();
        }
        public class SchemaCell
        {
            public string Name { get; set; } = string.Empty;
            public string? ConvertorName { get; set; }
        }
        #endregion
        public List<TBlockSchema> Blocks { get; } = new List<TBlockSchema>();
        public TColumnHeadSchema ColumnHeadTemplate { get; set; } = new TColumnHeadSchema();
    }
}
