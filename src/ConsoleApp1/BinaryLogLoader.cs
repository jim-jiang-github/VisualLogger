using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.LogContent;

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
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            _binaryObject.LoadFromStream(memoryStream);
            var logContent = _binaryObject.GetValueFromRecursivePath("Root.LogContent") as IEnumerable<StreamDataBlock[]>;
            LogContent content = new LogContent(_columns, logContent.Select(x => new LogItem(x)).ToArray());
            var a = content.ToArray();
            foreach (var item in content)
            {
                foreach (var data in item.Datas)
                {
                    var aa = data.GetData();
                }
            }
            return null;
        }
    }
}
