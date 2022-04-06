using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using VisualLogger.InterfaceModules;

namespace VisualLogger.Pages
{
    public sealed partial class FetchData
    {
        private IEnumerable<TestData> Lines { get; } = Enumerable.Range(0, 100)
        .Select(i => $"{i}------{Guid.NewGuid()}")
        .Select(s => new TestData()
        {
            Name = s
        }).ToArray();

        public FetchData()
        {
        }
        public async Task OpenFiles()
        {
            await DialogService.Show(new DialogOption()
            {
            });
            return;
            var a1 = await LogPicker.PickerLogs();
        }
        private int DemoValue1 { get; set; } = 1;
        private async Task OnResultDialogClick()
        {
            Logger.LogDebug("LogDebug");
            var a = await LogDownloader.DownloadLogs();

        }

        public class TestData
        {
            public string Name { get; set; }
        }
    }
}
