using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Toolbelt.Blazor.HotKeys;
using VisualLogger.Viewer.Data;
using VisualLogger.Viewer.Interfaces;
using VisualLogger.Viewer.ViewModels;

namespace VisualLogger.Viewer.Pages
{
    public partial class MainLayout : IHotkeysable
    {
       

        private bool _drawer = false;
        private string[] _links = new string[3] { "Home", "Contacts", "Settings" };

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ((IHotkeysable)this).AddHotkeys(HotKeys);
            //HotKeys.CreateContext().Add(ModKeys.Ctrl, Keys.S, () => this.AppBarNavIconClick());
        }

        public void AppBarNavIconClick()
        {
            _drawer = !_drawer;
            this.StateHasChanged();
        }
        List<string> items1 = new()
    {
        "Dog Photos",
        "Cat Photos",
        "Potatoes",
        "Carrots",
        "Carrots",
        "Carrots",
        "Carrots",
        "Carrots",
        "Carrots",
        "Carrots",
        "Carrots",
        "Carrots",
    };

        List<StringNumber> model = new() { "Carrots", "Potatoes" };
        bool showMenu = false;
        public void InputFocus()
        {
            int i = 1;
        }

        public void Dispose()
        {
        }

        public Task OpenScenarioDialog()
        {
            return Task.CompletedTask;
        }

        class Item
        {
            public string? Title { get; set; }
            public string? Icon { get; set; }
            public void OnClick(MouseEventArgs mouseEventArgs)
            {

            }
        }
        string[] menuitems =
          {
        "Click Me",
        "Click Me",
        "Click Me",
        "Click Me 2"
    };
        private Item[] _items = new Item[]
        {
         new Item{ Title= "Logs", Icon= "mdi-file-document-multiple-outline" },
         new Item { Title= "Photos", Icon= "mdi-image" },
         new Item { Title= "About", Icon= "mdi-help-box" }
        };

        public string Page => "Main view";

        public IEnumerable<HotkeyItem> Keys
        {
            get
            {
                yield return new HotkeyItem(HotKeys, "asd", () =>
                {

                });
                yield return new HotkeyItem(HotKeys, "asd1", () =>
                {

                });
                yield return new HotkeyItem(HotKeys, "asd2", () =>
                {

                });
            }
        }
    }
}
