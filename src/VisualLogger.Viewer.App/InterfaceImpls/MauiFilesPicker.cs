using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Scenarios;
using VisualLogger.Viewer.Web.Interfaces;
using VisualLogger.Localization;

namespace VisualLogger.Viewer.App.InterfaceImpls
{
    internal class MauiFilesPicker : IFilesPicker
    {
        protected readonly IFolderPicker _folderPicker;
        protected readonly Scenario _scenario;

        public MauiFilesPicker(IFolderPicker folderPicker,
            Scenario scenario)
        {
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
                PickerTitle = I18nKeys.Picker.SelectFiles,
                FileTypes = new FilePickerFileType(filePickerFileType)
            };
            var fileResults = await FilePicker.PickMultipleAsync(pickOptions);
            return fileResults.Select(r => r.FullPath);
        }
    }
}
