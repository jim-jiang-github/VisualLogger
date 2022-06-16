using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Shared.Data;
using VisualLogger.Shared.InterfaceModules;

namespace VisualLogger.Shared.InterfaceImplModules
{
    internal class ScenarioOptions : IScenarioOptions
    {
        public event Func<Task>? ScenarioDialog;
        public async Task OpenScenarioDialog()
        {
            await (ScenarioDialog?.Invoke() ?? Task.CompletedTask);
        }
    }
}
