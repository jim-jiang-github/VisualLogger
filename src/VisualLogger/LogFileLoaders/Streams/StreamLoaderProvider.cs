using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Schemas.Logs;

namespace VisualLogger.LogFileLoaders.Streams
{
    internal class StreamLoaderProvider
    {
        public static StreamLoader? GetStreamLoader(LogFileLoaderType loaderType)
        {
            switch (loaderType)
            {
                case LogFileLoaderType.Txt:
                    return new StreamLoaderText();
                case LogFileLoaderType.MemoryMapped:
                    return new StreamLoaderMemoryMapped();
                default:
                    return null;
            }
        }
    }
}
