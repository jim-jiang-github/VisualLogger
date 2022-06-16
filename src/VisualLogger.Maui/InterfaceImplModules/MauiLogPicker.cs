using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Core.Scenarios;
using VisualLogger.InterfaceModules;
using VisualLogger.Maui.InterfaceModules;
using VisualLogger.Resources.Languages;

namespace VisualLogger.Maui.InterfaceImplModules
{
    internal class MauiLogPicker
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

        public async Task<IEnumerable<string>> PickFromFiles()
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

        public async Task<IEnumerable<string>> PickFromFolder()
        {
            var folder = await _folderPicker.PickFolder();
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> PickFromWebsite()
        {
            throw new NotImplementedException();
        }
    }
}
