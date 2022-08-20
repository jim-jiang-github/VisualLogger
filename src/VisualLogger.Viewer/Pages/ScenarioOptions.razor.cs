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

        private string? e1 = null;
        private string? e2 = null;

        List<Item> _items = new()
        {
            new Item("Foo", "1"),
            new Item("Bar", "2"),
            new Item("Fizz", "3"),
            new Item("Buzz", "4"),
            new Item("Bar", "5"),
            new Item("Fizz", "6"),
            new Item("Buzz", "7"),
            new Item("Bar", "8"),
            new Item("Fizz", "9"),
            new Item("Buzz", "10"),
            new Item("Bar", "11"),
            new Item("Fizz", "12"),
            new Item("Buzz", "13"),
            new Item("Bar", "14"),
            new Item("Fizz", "15"),
            new Item("Buzz", "16"),
            new Item("Bar", "17"),
            new Item("Fizz", "18"),
            new Item("Buzz", "19"),
            new Item("Bar", "20"),
            new Item("Fizz", "21"),
            new Item("Buzz", "22"),
            new Item("Bar", "23"),
            new Item("Fizz", "24"),
            new Item("Buzz", "25"),
            new Item("Bar", "26"),
            new Item("Fizz", "27"),
            new Item("Buzz", "28"),
            new Item("Bar", "29"),
            new Item("Fizz", "30"),
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
