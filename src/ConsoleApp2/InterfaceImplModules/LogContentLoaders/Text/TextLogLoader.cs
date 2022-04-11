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
    public class TextLogLoader : ILogContentLoader
    {
        private readonly TextParser _textParser;

        public static TextLogLoader Load( TextParser textParser)
        {
            try
            {
                if ( textParser == null)
                {
                    return null;
                }
                return new TextLogLoader(textParser);
            }
            catch
            {
                return null;
            }
        }
        private TextLogLoader(TextParser textParser)
        {
            _textParser = textParser;
        }

        public LogContent LoadLogContent(string logPath)
        {
            FileStreamLoader fileStreamLoader = new FileStreamLoader();
            var stream = fileStreamLoader.LoadLogStream(logPath);
            if (stream == null)
            {
                return null;
            }
            var fileObject = FileObject.Load(stream, _textParser);
            if (fileObject == null)
            {
                return null;
            }
            return null;
        }
    }
}
