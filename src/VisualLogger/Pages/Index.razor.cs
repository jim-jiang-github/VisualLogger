using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;

namespace VisualLogger.Pages
{
    public sealed partial class Index
    {
        public VirtualizeTable virtualizeTable = new VirtualizeTable();

        public Index()
        {
            virtualizeTable.ColumnNames = new string[] { "C1", "C2", "C3" };
            virtualizeTable.Rows = Enumerable.Range(0, 100)
                .Select(i => $"{i}------{Guid.NewGuid()}")
                .Select(s => new string[]
                {
                    $"C1:{s}",
                    $"C2:{s}",
                    $"C3:{s}",
                }).ToArray();
            Cultures = new Dictionary<string, string>();
            Cultures.Add("en-US", "en-US");
            Cultures.Add("zh-CN", "zh-CN");
        }

        public class VirtualizeTable
        {
            public string[] ColumnNames { get; set; }
            public string[][] Rows { get; set; }
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
