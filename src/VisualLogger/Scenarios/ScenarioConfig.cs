using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Scenarios
{
    public class ScenarioConfig : IJsonSerializable
    {
        public string? ScenarioGitRepo { get; set; }
    }
}
