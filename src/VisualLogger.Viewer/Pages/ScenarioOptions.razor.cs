using BlazorComponent;
using Microsoft.AspNetCore.Components;
using VisualLogger.Viewer.Components;
using VisualLogger.Viewer.Data;
using VisualLogger.Viewer.Interfaces;
using VisualLogger.Viewer.ViewModels;

namespace VisualLogger.Viewer.Pages
{
    public partial class ScenarioOptions : IDisposable
    {
        [Inject]
        private ScenarioOptionsViewModel? ViewModel { get; set; }

        private string? e1 = "ssss";
        private string? e2 = null;

        List<string> _items = new()
        {
            "111",
            "112",
            "113",
            "114",
            "115",
            "116",
            "117",
            "118",
            "119",
            "120",
            "121",
            "122",
            "123",
            "124",
            "125",
            "126",
            "127",
            "128",
            "129",
            "130",
        };

        public class Item
        {
            public string Label { get; set; }
            public string Value { get; set; }
            public Item(string label, string value)
            {
                Label = label;
                Value = value;
            }
        }
    }
}
