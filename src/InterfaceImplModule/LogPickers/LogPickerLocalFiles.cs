using InterfaceModule;

namespace InterfaceImplModule.LogPickers
{
    public class LogPickerLocalFiles : ILogPicker
    {
        public string[] FileTypes => throw new NotImplementedException();

        public Task<string[]> PickerLogs()
        {
            throw new NotImplementedException();
        }
    }
}