using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Datas;
using VisualLogger.InterfaceImplModules.LogStreamLoaders;
using VisualLogger.InterfaceModules;

namespace VisualLogger.InterfaceImplModules.LogContentLoaders.Text
{
    public class TextContentLoader : ILogContentLoader
    {
        private readonly TextContentParser _textContentParser;

        public static TextContentLoader Load(TextContentParser textContentParser)
        {
            try
            {
                if (textContentParser == null)
                {
                    return null;
                }
                return new TextContentLoader(textContentParser);
            }
            catch
            {
                return null;
            }
        }
        private TextContentLoader(TextContentParser textContentParser)
        {
            _textContentParser = textContentParser;
        }

        public LogContent LoadLogContent(string logPath)
        {
            var fileStreamLoader = new FileStreamLoader();
            var stream = fileStreamLoader.LoadLogStream(logPath);
            if (stream == null)
            {
                return null;
            }
            var textContent = TextContent.Load(stream, _textContentParser);
            if (textContent == null)
            {
                return null;
            }
            var itemsTemplate = textContent.GetItemsTemplate(_textContentParser.LogItemsPath);
            var items = textContent.GetItems(_textContentParser.LogItemsPath);
            var content = new LogContent(itemsTemplate, items);
            return content;
        }
    }
}
