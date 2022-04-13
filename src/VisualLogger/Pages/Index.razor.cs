using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;
using VisualLogger.InterfaceImplModules.LogContentLoaders.Binary;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using VisualLogger.Datas;

namespace VisualLogger.Pages
{
    public sealed partial class Index
    {
        public VirtualizeTable virtualizeTable = new VirtualizeTable();
        private LogContent logContent;

        public Index()
        {
            Cultures = new Dictionary<string, string>();
            Cultures.Add("en-US", "en-US");
            Cultures.Add("zh-CN", "zh-CN");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var parserFile = @"C:\Users\Jim.Jiang\Documents\VisualLogger\src\ConsoleApp1\RCRooms_Windows_Binary_Parser.json";
            var binaryContentParser = BinaryContentParser.LoadFromJsonFile(parserFile);
            var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_1112e3df-80f9-435d-8b5d-2b7c5a76ee1f_20220407-172316\RoomsHost-20220407165140.rcvlog";
            //var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_1112e3df-80f9-435d-8b5d-2b7c5a76ee1f_20220407-172316\RoomsServiceHost-20220407163855.rcvlog";
            var binaryContentLoader = BinaryContentLoader.Load(binaryContentParser);
            logContent = binaryContentLoader.LoadLogContent(logFile);
            virtualizeTable.ColumnNames = logContent.ColumnsName;
            virtualizeTable.TotalCount = logContent.Count;
        }
        protected async ValueTask<ItemsProviderResult<StreamCell[]>> LoadForecasts(ItemsProviderRequest request)
        {
            var result = logContent.GetItems(request.StartIndex, request.Count);

            return new ItemsProviderResult<StreamCell[]>(result, virtualizeTable.TotalCount);
        }
        public class VirtualizeTable
        {
            public string[] ColumnNames { get; set; }
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
