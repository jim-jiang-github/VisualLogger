using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Scenarios;

namespace VisualLogger.Shared.InterfaceImplModules
{
    internal class FilesPickerWeb
    {
        public event Func<Task>? FilesPickerDialog;
        private readonly Scenario _scenario;

        public string[] SupportedExtensions => _scenario.SupportedExtensions;

        public FilesPickerWeb(Scenario scenario) 
        {
            _scenario = scenario;
        }

        public async Task<string?> PickFiles()
        {
            await (FilesPickerDialog?.Invoke() ?? Task.CompletedTask);
            return "";
        }
    }
}
