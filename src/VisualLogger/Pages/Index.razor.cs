using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;
using VisualLogger.InterfaceImplModules.LogContentLoaders.Binary;
using static VisualLogger.InterfaceModules.LogContent;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace VisualLogger.Pages
{
    public sealed partial class Index
    {
        public VirtualizeTable virtualizeTable = new VirtualizeTable();

        public Index()
        {
            Cultures = new Dictionary<string, string>();
            Cultures.Add("en-US", "en-US");
            Cultures.Add("zh-CN", "zh-CN");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_1112e3df-80f9-435d-8b5d-2b7c5a76ee1f_20220407-172316\RoomsHost-20220407165140.rcvlog";
            //var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_1112e3df-80f9-435d-8b5d-2b7c5a76ee1f_20220407-172316\RoomsServiceHost-20220407163855.rcvlog";
            var parserFile = @"C:\Users\Jim.Jiang\Documents\VisualLogger\src\ConsoleApp1\RCRooms_Windows_Binary_Parser.json";
            var parser = BinaryLogLoader.Load(parserFile);
            virtualizeTable.ColumnNames = parser.Columns;
            var content = parser.LoadLogContent(logFile);
            virtualizeTable.Rows = content.AsEnumerable();
            virtualizeTable.TotalCount = content.Count;
        }
        protected async ValueTask<ItemsProviderResult<LogItem>> LoadForecasts(ItemsProviderRequest request)
        {
            var result = virtualizeTable.Rows.Skip(request.StartIndex).Take(request.Count);

            return new ItemsProviderResult<LogItem>(result, virtualizeTable.TotalCount);
        }
        public class VirtualizeTable
        {
            public string[] ColumnNames { get; set; }
            public IEnumerable<LogItem> Rows { get; set; } = new LogItem[0];
            public int TotalCount { get; set; } = 0;
        }
        public Dictionary<string, string> Cultures { get; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        private string SelectedCulture { get; set; } = CultureInfo.CurrentUICulture.Name;

        private async Task SetCulture(SelectedItem item)
        {
            if (item.Value != Thread.CurrentThread.CurrentUICulture.Name)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(item.Value);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(item.Value);
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(item.Value);
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(item.Value);
                NavigationManager.NavigateTo("/", true);
            }
        }
    }
}
