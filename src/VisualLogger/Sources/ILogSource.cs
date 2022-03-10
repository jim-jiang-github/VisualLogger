using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Schemas;
using VisualLogger.Streams;
using System.Reflection;
using VisualLogger.Schemas.Logs;

namespace VisualLogger.Sources
{
    public interface ILogSource : IDisposable
    {
        #region Provider

        public static ILogSource? LoadLogSource(Stream logFileStream, string schemaLogPath)
        {
            var schemaType = Schema.GetSchemaTypeFromJsonFile(schemaLogPath);
            switch (schemaType)
            {
                case SchemaType.LogText:
                    var logSourceText = LoadLogSource<LogSourceText, SchemaLogText>(logFileStream, schemaLogPath);
                    return logSourceText;
                case SchemaType.LogBinary:
                    var logSourceBinary = LoadLogSource<LogSourceBinary, SchemaLogBinary>(logFileStream, schemaLogPath);
                    return logSourceBinary;
            }
            return null;
        }
        private static ILogSource? LoadLogSource<TLogSource, TSchemaLog>(Stream logFileStream, string schemaLogPath)
           where TLogSource : class, ILogSource
           where TSchemaLog : SchemaLog, new()
        {
            var schemaLog = IJsonSerializable.LoadFromJsonFile<TSchemaLog>(schemaLogPath);
            if (schemaLog == null)
            {
                Log.Warning("Can not load schemaLog from {schemaLogPath}", schemaLogPath);
                return null;
            }
            try
            {
                var logSourceType = typeof(TLogSource);
                var logSourceConstructors = logSourceType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                var logSource = logSourceConstructors[0].Invoke(new object[] { logFileStream, schemaLog }) as TLogSource;
                var init = logSourceType.GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);

                if (init != null)
                {
                    init.Invoke(logSource, null);
                }

                return logSource;
            }
            catch (Exception ex)
            {
                Log.Warning("Load {LogSource} error {Error}", typeof(TLogSource), ex);
                return null;
            }
        }

        #endregion

        int TotalRowsCount { get; }
        string[] ColumnNames { get; }
        IEnumerable<string> EnumerateWords { get; }
        LogFilter Filter { get; }
        IEnumerable<LogRow> GetRows(int start, int length);
    }
}
