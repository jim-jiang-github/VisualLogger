using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Scenarios;
using VisualLogger.Shared.Resources.Languages;
using VisualLogger.Shared.InterfaceModules;

namespace VisualLogger.Maui.InterfaceImplModules
{
    internal class MauiLogPicker : IFilesPicker
    {
        private readonly IStringLocalizer<Strings> _stringLocalizer;
        protected readonly IFolderPicker _folderPicker;
        protected readonly Scenario _scenario;

        public MauiLogPicker(IStringLocalizer<Strings> stringLocalizer,
            IFolderPicker folderPicker,
            Scenario scenario)
        {
            _stringLocalizer = stringLocalizer;
            _folderPicker = folderPicker;
            _scenario = scenario;
        }

        public async Task<IEnumerable<string>> PickFiles()
        {
            var fileTypes = new[] { "zip", "7z", "rar" };
            fileTypes = fileTypes.Concat(_scenario.SupportedExtensions).ToArray();
            var filePickerFileType = new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, fileTypes },
                { DevicePlatform.MacCatalyst, fileTypes }
            };
            var pickOptions = new PickOptions()
            {
                PickerTitle = _stringLocalizer["SelectLogFiles"],
                FileTypes = new FilePickerFileType(filePickerFileType)
            };
            var fileResults = await FilePicker.PickMultipleAsync(pickOptions);
            return fileResults.Select(r => r.FullPath);
        }
    }
}
