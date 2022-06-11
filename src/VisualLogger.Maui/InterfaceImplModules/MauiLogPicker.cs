using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.InterfaceModules;
using VisualLogger.Resources.Languages;

namespace VisualLogger.Maui.InterfaceImplModules
{
    internal class MauiLogPicker : ILogPicker
    {
        private readonly IStringLocalizer<Strings> _stringLocalizer;
        protected readonly IFolderPicker _folderPicker;
        public MauiLogPicker(IStringLocalizer<Strings> stringLocalizer,
            IFolderPicker folderPicker)
        {
            _stringLocalizer = stringLocalizer;
            _folderPicker = folderPicker;
        }

        public async Task<IEnumerable<string>> PickFromFile()
        {
            var fileTypes = new[] { "txt", "log", "zip", "7z", "rar" };
            var filePickerFileType = new Dictionary<DevicePlatform, IEnumerable<string>>();
            filePickerFileType.Add(DevicePlatform.WinUI, fileTypes);
            filePickerFileType.Add(DevicePlatform.MacCatalyst, fileTypes);
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
