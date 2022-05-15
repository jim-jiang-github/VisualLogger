using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Datas;
using VisualLogger.InterfaceImplModules.LogContentLoaders.Binary;

namespace VisualLogger.BlazorComponents
{
    public sealed partial class LogTableView
    {
        private IJSObjectReference? module;
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public string[] ColumnNames { get; set; } = Array.Empty<string>();
        public int TotalCount { get; set; } = 0;
        private LogContent logContent;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //var parserFile = @"C:\Users\Jim.Jiang\Documents\VisualLogger\src\ConsoleApp1\RCRooms_Windows_Binary_Parser.json";
                //var binaryContentParser = BinaryContentParser.LoadFromJsonFile(parserFile);
                //var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_1112e3df-80f9-435d-8b5d-2b7c5a76ee1f_20220407-172316\RoomsHost-20220407165140.rcvlog";
                ////var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_1112e3df-80f9-435d-8b5d-2b7c5a76ee1f_20220407-172316\RoomsServiceHost-20220407163855.rcvlog";
                //var binaryContentLoader = BinaryContentLoader.Load(binaryContentParser);
                //logContent = binaryContentLoader.LoadLogContent(logFile);
                //ColumnNames = logContent.ColumnsName;
                //TotalCount = logContent.Count;
                module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/Index.razor.js");
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
