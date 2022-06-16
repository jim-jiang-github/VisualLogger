using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Data;
using VisualLogger.InterfaceModules;

namespace VisualLogger.InterfaceImplModules
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
