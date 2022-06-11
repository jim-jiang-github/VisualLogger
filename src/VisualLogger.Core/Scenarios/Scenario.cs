using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Core.Schemas;
using VisualLogger.Core.Schemas.Logs;
using VisualLogger.Core.Schemas.Scenarios;

namespace VisualLogger.Core.Scenarios
{
    public class Scenario
    {
        private const string SCENARIOS_FOLDER = "Scenarios";
        private static readonly string _scenarioDirectory = Path.Combine(Directory.GetCurrentDirectory(), SCENARIOS_FOLDER);

        //public string[] SupportExtensions { get; }
        private Scenario(SchemaScenario schemaScenario)
        {
            //var a = schemaScenario.SchemaLogPaths
            //       .Select(p => Path.Combine(_scenarioDirectory, p))
            //       .Select(p => SchemaLog.GetAvailableExtensionsFromJsonFile(p))
            //       .ToArray();
        }
        private static Dictionary<SchemaType, string> LoadSchemaTypeMap()
        {
            var files = Directory.GetFiles(_scenarioDirectory);
            var schemaTypeMap = files.ToDictionary(f => Schema.GetSchemaTypeFromJsonFile(f), f => f);
            return schemaTypeMap;
        }
        public static bool Check()
        {
            var schemaTypeMap = LoadSchemaTypeMap();
            var schemaScenarioCount = schemaTypeMap.Count(x => x.Key == SchemaType.Scenario);
            if (schemaScenarioCount == 0)
            {
                Log.Information("Can not found schemaScenario in {scenarioDirectory}", _scenarioDirectory);
                return false;
            }
            if (schemaScenarioCount > 1)
            {
                Log.Information("Found multiple schemaScenario in {scenarioDirectory}", _scenarioDirectory);
                return false;
            }
            return true;
        }
        public static Scenario? Load()
        {
            if (!Check())
            {
                return null;
            }
            var schemaTypeMap = LoadSchemaTypeMap();
            var schemaScenarioPath = schemaTypeMap.FirstOrDefault(x => x.Key == SchemaType.Scenario).Value;
            var schemaScenario = Schema.LoadFromJsonFile<SchemaScenario>(schemaScenarioPath);
            if (schemaScenario == null)
            {
                Log.Information("Can not load schemaScenario from json file in {schemaScenarioPath}", schemaScenarioPath);
                return null;
            }
            return new Scenario(schemaScenario);
        }
    }
}
