using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Core.LogFileLoaders.Streams;
using VisualLogger.Core.Schemas;
using VisualLogger.Core.Schemas.Logs;

namespace VisualLogger.Core.Sources
{
    public class LogSourceLoader
    {
        public static ILogSource? Load(string logFilePath, string logSchemaPath)
        {
            var schemaType = Schema.GetSchemaTypeFromJsonFile(logSchemaPath);
            switch (schemaType)
            {
                case SchemaType.LogText:
                    var logSourceText = LoadLogSource<LogSourceText, SchemaLogText>(logFilePath, logSchemaPath);
                    return logSourceText;
                case SchemaType.LogBinary:
                    var logSourceBinary = LoadLogSource<LogSourceBinary, SchemaLogBinary>(logFilePath, logSchemaPath);
                    return logSourceBinary;
            }
            return null;
        }
        private static Stream? LoadLogFileStream(string logFilePath, SchemaLog schemaLog)
        {
            var extension = Path.GetExtension(logFilePath);
            var available = schemaLog.AvailableExtensions.Contains(extension);
            if (!available)
            {
                Log.Error("SchemaLog {AvailableExtensions} not available", string.Join(",", schemaLog.AvailableExtensions));
                return null;
            }
            var streamLoader = StreamLoaderProvider.GetStreamLoader(schemaLog.LoaderType);
            if (streamLoader == null)
            {
                Log.Error("{File} can not find stream loader {Type}", logFilePath, schemaLog.LoaderType);
                return null;
            }
            var stream = streamLoader.LoadLogStream(logFilePath);
            return stream;
        }
        private static ILogSource? LoadLogSource<TLogSource, TLogSchema>(string logFilePath, string logSchemaPath)
            where TLogSource : class, ILogSource
            where TLogSchema : SchemaLog, new()
        {
            var schemaLog = Schema.LoadFromJsonFile<TLogSchema>(logSchemaPath);
            if (schemaLog == null)
            {
                return null;
            }
            var stream = LoadLogFileStream(logFilePath, schemaLog);
            if (stream == null)
            {
                return null;
            }
            try
            {
                var logSourceConstructors = (typeof(TLogSource)).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                var logSource = logSourceConstructors[0].Invoke(new object[] { stream, schemaLog }) as TLogSource;
                return logSource;
            }
            catch (Exception ex)
            {
                Log.Error("Load {LogSource} error {Error}", typeof(TLogSource), ex);
                return null;
            }
        }
    }
}
