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
    public sealed partial class LogTableView
    {
        private IJSObjectReference? module;
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public string[] ColumnNames { get; set; }
        public int TotalCount { get; set; } = 0;
        private static LogContent logContent;
        static LogTableView() 
        {

            var parserFile = @"C:\Users\Jim.Jiang\Documents\VisualLogger\src\ConsoleApp1\RCRooms_Windows_Binary_Parser.json";
            var binaryContentParser = BinaryContentParser.LoadFromJsonFile(parserFile);
            var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_1112e3df-80f9-435d-8b5d-2b7c5a76ee1f_20220407-172316\RoomsHost-20220407165140.rcvlog";
            //var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_1112e3df-80f9-435d-8b5d-2b7c5a76ee1f_20220407-172316\RoomsServiceHost-20220407163855.rcvlog";
            var binaryContentLoader = BinaryContentLoader.Load(binaryContentParser);
            logContent = binaryContentLoader.LoadLogContent(logFile);
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();

            ColumnNames = logContent.ColumnsName;
            TotalCount = logContent.Count;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/logTableView.js");
                await module.InvokeVoidAsync("init");
            }
        }
        protected async ValueTask<ItemsProviderResult<StreamCell[]>> LoadForecasts(ItemsProviderRequest request)
        {
            var result = logContent.GetItems(request.StartIndex, request.Count);

            return new ItemsProviderResult<StreamCell[]>(result, TotalCount);
        }
    }
}
