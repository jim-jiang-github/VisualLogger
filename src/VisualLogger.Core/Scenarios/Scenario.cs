using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Core.LogFileLoaders.Streams;
using VisualLogger.Core.Schemas;
using VisualLogger.Core.Schemas.Logs;
using VisualLogger.Core.Schemas.Scenarios;
using VisualLogger.Core.Sources;

namespace VisualLogger.Core.Scenarios
{
    public class Scenario : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private const string SCENARIOS_FOLDER = "Scenarios";
        private static readonly string _scenarioDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, SCENARIOS_FOLDER);

        private StreamLoader? _streamLoader;
        private string? _schemaLogPath;

        public string[] SupportedExtensions { get; private set; } = Array.Empty<string>();
        public string[] LoadedLogFiles { get; private set; } = Array.Empty<string>();
        public ILogSource? LogSource { get; private set; }
        public bool Initialized => SupportedExtensions.Length > 0;
        public bool LogSourceLoaded => LogSource != null;
        public Scenario()
        {

        }
        protected void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool Init()
        {
            if (!Directory.Exists(_scenarioDirectory))
            {
                return false;
            }
            var files = Directory.GetFiles(_scenarioDirectory);
            var schemaTypeMap = files.ToDictionary(f => Schema.GetSchemaTypeFromJsonFile(f), f => f);
            var schemaScenarioCount = schemaTypeMap.Count(x => x.Key == SchemaType.Scenario);
            if (schemaScenarioCount == 0)
            {
                Log.Warning("Can not found schemaScenario in {scenarioDirectory}", _scenarioDirectory);
                return false;
            }
            if (schemaScenarioCount > 1)
            {
                Log.Warning("Found multiple schemaScenario in {scenarioDirectory}", _scenarioDirectory);
                return false;
            }
            var schemaScenarioPath = schemaTypeMap.FirstOrDefault(x => x.Key == SchemaType.Scenario).Value;
            var schemaScenario = IJsonSerializable.LoadFromJsonFile<SchemaScenario>(schemaScenarioPath);
            if (schemaScenario == null)
            {
                Log.Warning("Can not load schemaScenario from json file in {schemaScenarioPath}", schemaScenarioPath);
                return false;
            }
            if (schemaScenario.SchemaLogName == string.Empty)
            {
                Log.Warning("Schema scenario do not have schema log name");
                return false;
            }
            var schemaLogPath = Path.Combine(_scenarioDirectory, schemaScenario.SchemaLogName);
            var schemaLogContent = IJsonSerializable.LoadContentFromJsonFile(schemaLogPath);
            if (schemaLogContent == null)
            {
                Log.Warning("Can not load schema log content from json file in {schemaLogPath}", schemaLogPath);
                return false;
            }
            var supportExtensions = SchemaLog.GetSupportedExtensionsFromJsonContent(schemaLogContent);
            if (supportExtensions.Length <= 0)
            {
                Log.Warning("Can not support any extensions from json file in {schemaLogPath}", schemaLogPath);
                return false;
            }
            var logFileLoaderType = SchemaLog.GetLogFileLoaderTypeFromJsonContent(schemaLogContent);
            var streamLoader = StreamLoaderProvider.GetStreamLoader(logFileLoaderType);
            if (streamLoader == null)
            {
                Log.Warning("Can not load stream from json file by {loader} in {schemaLogContent}", streamLoader, schemaLogContent);
                return false;
            }
            _schemaLogPath = schemaLogPath;
            _streamLoader = streamLoader;
            SupportedExtensions = supportExtensions;
            return true;
        }

        public void Dispose()
        {
            LogSource?.Dispose();
            LogSource = null;
            _schemaLogPath = null;
            _streamLoader = null;
            SupportedExtensions = Array.Empty<string>();
        }
        public void LoadLogFiles(string[] logFiles)
        {
            LoadedLogFiles = logFiles;
            LoadLogSource(logFiles[0]);
            OnPropertyChanged(nameof(LoadedLogFiles));
        }
        public bool LoadLogSource(string logFilePath)
        {
            if (!Initialized)
            {
                Log.Warning("No initialized");
                return false;
            }
            if (_streamLoader == null || _schemaLogPath == null)
            {
                return false;
            }
            if (LogSourceLoaded)
            {
                Log.Information("Reset LogSource");
                LogSource?.Dispose();
                LogSource = null;
            }
            Log.Information("Load LogSource from {logFilePath}", logFilePath);
            var stream = _streamLoader.LoadLogStreamFromPath(logFilePath);
            LogSource = LoadLogSource(stream, _schemaLogPath);
            OnPropertyChanged(nameof(LogSource));
            return true;
        }
        private ILogSource? LoadLogSource(Stream logFileStream, string schemaLogPath)
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
        private ILogSource? LoadLogSource<TLogSource, TSchemaLog>(Stream logFileStream, string schemaLogPath)
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
                var logSourceConstructors = (typeof(TLogSource)).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                var logSource = logSourceConstructors[0].Invoke(new object[] { logFileStream, schemaLog }) as TLogSource;
                return logSource;
            }
            catch (Exception ex)
            {
                Log.Warning("Load {LogSource} error {Error}", typeof(TLogSource), ex);
                return null;
            }
        }
    }
}
