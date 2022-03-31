using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualLogger.LogDownloaders;

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
            var a = LogDownloader.GetLogs("");
            var a1 = await LogPicker.PickerLogs();
        }
        private int DemoValue1 { get; set; } = 1;
        private async Task OnResultDialogClick()
        {
            var result = await DialogService.ShowModal<LogDownloaderPage>(new ResultDialogOption()
            {
                Title = "带返回值模态弹出框",
                ComponentParamters = new Dictionary<string, object>
                {
                    [nameof(LogDownloaderPage.Value)] = DemoValue1,
                    [nameof(LogDownloaderPage.ValueChanged)] = EventCallback.Factory.Create<int>(this, v => DemoValue1 = v)
                }
            });

        }

        public class TestData
        {
            public string Name { get; set; }
        }
    }
}
