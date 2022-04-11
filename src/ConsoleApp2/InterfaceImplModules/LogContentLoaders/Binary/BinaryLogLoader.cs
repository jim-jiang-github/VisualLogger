using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Datas;
using VisualLogger.InterfaceImplModules.LogStreamLoaders;
using VisualLogger.InterfaceModules;

namespace VisualLogger.InterfaceImplModules.LogContentLoaders.Binary
{
    public class BinaryLogLoader : ILogContentLoader
    {
        private readonly BinaryContentParser _binaryContentParser;

        public static BinaryLogLoader Load(BinaryContentParser binaryContentParser)
        {
            try
            {
                if (binaryContentParser == null)
                {
                    return null;
                }
                return new BinaryLogLoader(binaryContentParser);
            }
            catch
            {
                return null;
            }
        }

        private BinaryLogLoader(BinaryContentParser binaryContentParser)
        {
            _binaryContentParser = binaryContentParser;
        }

        public LogContent LoadLogContent(string logPath)
        {
            var memoryMappedStreamLoader = new MemoryMappedStreamLoader();
            var stream = memoryMappedStreamLoader.LoadLogStream(logPath);
            if (stream == null)
            {
                return null;
            }
            var binaryContent = BinaryContent.Load(stream, _binaryContentParser);
            if (binaryContent == null)
            {
                return null;
            }
            var itemsTemplate = binaryContent.GetItemsTemplate(_binaryContentParser.LogItemsPath);
            var items = binaryContent.GetItems(_binaryContentParser.LogItemsPath);
            var content = new LogContent(itemsTemplate, items);
            return content;
        }
    }
}
