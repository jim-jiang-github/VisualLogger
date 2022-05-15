using Microsoft.Extensions.Localization;
using VisualLogger.InterfaceModules;
using VisualLogger.Resources.Languages;

namespace VisualLogger.LogPickers
{
    internal class LogPickerLocalFiles : ILogPicker
    {
        private readonly IStringLocalizer<Strings> _stringLocalizer;
        private readonly IFolderPicker _folderPicker;
        private readonly IWebsitePicker _websitePicker;

        public LogPickerLocalFiles(IStringLocalizer<Strings> stringLocalizer,
            IFolderPicker folderPicker,
            IWebsitePicker websitePicker)
        {
            _stringLocalizer = stringLocalizer;
            _folderPicker = folderPicker;
            _websitePicker = websitePicker;
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
            //Directory.Exists(folder)
            return null;
        }

        public async Task<IEnumerable<string>> PickFromWebsite()
        {
            var website = await _websitePicker.PickWebsite();
            return null;
        }

        //Load Logs
    }
}