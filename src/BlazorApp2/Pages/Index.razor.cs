using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;

namespace BlazorApp2.Pages
{
    public sealed partial class Index
    {
        [Inject]
        public IJSRuntime jSRuntime { get; set; }

        private IJSObjectReference? module;
        VirtualizeTable virtualizeTable = new VirtualizeTable();
        //protected async ValueTask<ItemsProviderResult<string[]>> LoadForecasts(ItemsProviderRequest request)
        //{

        //    var result = virtualizeTable.RowDatas.Skip(request.StartIndex).Take(request.Count);

        //    return new ItemsProviderResult<string[]>(result, virtualizeTable.TotalCount);
        //}
        private async Task<QueryData<RowData>> OnQueryAsync(QueryPageOptions options)
        {
            await Task.Delay(200);
            var items = virtualizeTable.RowDatas.Skip(options.StartIndex).Take(options.PageItems);
            return new QueryData<RowData>()
            {
                Items = items,
                TotalCount = virtualizeTable.TotalCount
            };
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/Index.razor.js");
                await module.InvokeVoidAsync("init");
            }
        }
    }
    public class VirtualizeTable
    {
        public string[] ColumnNames { get; }
        public IEnumerable<RowData> RowDatas { get; }
        public int TotalCount { get; } = 0;

        public VirtualizeTable()
        {
            TotalCount = 99;
            ColumnNames = new string[] { "C1", "C2", "C3", "C4", "C5", "C6", "C7" };
            RowDatas = Enumerable.Range(0, TotalCount).Select(i => new RowData()
            {
                Data = Enumerable.Range(0, ColumnNames.Length).Select(d => $"Row:{i}-Data:{d}").ToArray()
            }).ToArray();
        }
    }

    public class RowData
    {
        public string[] Data { get; set; }
    }
}
