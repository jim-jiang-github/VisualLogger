using InterfaceModule;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class BinaryLogLoader : ILogContentLoader
    {
        private string[] _columns;
        private BinaryObject _binaryObject;

        public static BinaryLogLoader Load(string descriptionFile)
        {
            try
            {
                var binaryParser = BinaryParser.LoadFromJsonFile(descriptionFile);
                var binaryObject = BinaryObject.LoadFromBinaryDescription(binaryParser);
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
            using var stream = memoryMappedFile.CreateViewStream();
            _binaryObject.LoadFromStream(stream);
            var logContent = _binaryObject.GetValueFromRecursivePath("Root.LogContent") as IEnumerable<IEnumerable<object>>;
            LogContent content = new LogContent(_columns, logContent.Select(x => new LogContent.LogItem(x)));
            return content;
        }
    }
}
