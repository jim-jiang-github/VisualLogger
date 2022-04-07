using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.InterfaceModules;
using VisualLogger.Resources.Languages;

namespace VisualLogger.InterfaceImplModules.LogDownloaders
{
    internal class LogDownloaderFromUrl : ILogDownloader
    {
        private DialogService _dialogService;
        private IStringLocalizer<Strings> _localizer;

        public string Name => _localizer["LogDownloader.Name"];

        public string RegexPattern => @"^((https)?:\/\/)cdn.filestackcontent.com/[^\s]+";

        public LogDownloaderFromUrl(DialogService dialogService, IStringLocalizer<Strings> localizer)
        {
            _dialogService = dialogService;
            _localizer = localizer;
        }

        public async Task<string[]> DownloadLogs()
        {
            int demoValue1 = 1;
            var result = await _dialogService.ShowModal<LogDownloaderPage>(new ResultDialogOption()
            {
                Title = _localizer["LogDownloader.Title", Name].Value,
                ComponentParamters = new Dictionary<string, object>
                {
                    [nameof(LogDownloaderPage.WaterMark)] = _localizer["LogDownloader.WaterMark"].Value,
                    [nameof(LogDownloaderPage.Value)] = demoValue1,
                    [nameof(LogDownloaderPage.ValueChanged)] = EventCallback.Factory.Create<int>(this, v => demoValue1 = v)
                }
            }) ;
            return null;
        }
    }
}
