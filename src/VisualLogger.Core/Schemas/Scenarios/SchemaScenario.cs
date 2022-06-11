using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Core.Schemas.Scenarios
{
    public class SchemaScenario : Schema
    {
        public string Name { get; set; } = string.Empty;
        public string SchemaLogName { get; set; } = string.Empty;
        public override SchemaType Type => SchemaType.Scenario;

        public void SaveAsDefault()
        {
            Name = "schema_scenario_rcv_windows_22.2.20";
            SchemaLogName = "schema_log_text_rcv_windows_22.2.20.json";
            this.SaveAsJson($"{Name}.json");
        }
    }
}
