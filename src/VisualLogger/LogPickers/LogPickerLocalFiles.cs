using InterfaceModules;

namespace VisualLogger.LogPickers
{
    public class LogPickerLocalFiles : ILogPicker
    {
        public string[] FileTypes => new[] { "", "" };

        public async Task<string[]> PickerLogs()
        {
            var a = await FilePicker.PickMultipleAsync();
            return null;
        }
    }
}