using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Datas.LogContents;
using VisualLogger.InterfaceModules;

namespace VisualLogger.InterfaceImplModules.LogContentLoaders.Binary
{
    public class BinaryLogLoader : ILogContentLoader
    {
        private readonly string[] _columns;
        private readonly BinaryObject _binaryObject;

        public string[] Columns => _columns;

        public static BinaryLogLoader Load(string parserFile)
        {
            try
            {
                var binaryParser = BinaryParser.LoadFromJsonFile(parserFile);
                if (binaryParser == null)
                {
                    return null;
                }
                var binaryObject = BinaryObject.LoadFromBinaryParser(binaryParser);
                return new BinaryLogLoader(binaryParser.Columns, binaryObject);
            }
            catch
            {
                return null;
            }
        }

        private BinaryLogLoader(string[] columns, BinaryObject binaryObject)
        {
            _columns = columns;
            _binaryObject = binaryObject;
        }

        public LogContent LoadLogContent(string logPath)
        {
            using MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateFromFile(logPath);
            var stream = memoryMappedFile.CreateViewStream();
            //var memoryStream = new MemoryStream();
            //stream.CopyTo(memoryStream);
            _binaryObject.LoadFromStream(stream);
            var logContent = _binaryObject.GetValueFromRecursivePath("Root.LogItems") as IEnumerable<LogCell[]>;
            LogContent content = new LogContent(_columns, logContent.Select(x => new LogItem(x)).ToArray());
            return content;
        }
    }
}
