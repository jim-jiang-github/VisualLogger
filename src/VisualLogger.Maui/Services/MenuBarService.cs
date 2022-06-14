using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.InterfaceModules;
using VisualLogger.Resources.Languages;

namespace VisualLogger.Maui.Services
{
    public class MenuBarService
    {
        private readonly List<MenuItem> _menuItems = new List<MenuItem>();
        public MenuBarService(IStringLocalizer<Strings> stringLocalizer,
            ILogPicker logPicker,
            IScenarioOptions scenarioOptions)
        {
            IEnumerable<MenuItem> GetOpenMenuItems()
            {
                yield return new MenuItem(stringLocalizer["MenuBar.File.Open.FromFiles"], clickAction: () =>
                {
                    logPicker?.PickFromFile();
                });
                yield return new MenuItem(stringLocalizer["MenuBar.File.Open.FromFolder"], clickAction: () =>
                {
                    logPicker?.PickFromFolder();
                });
                yield return new MenuItem(stringLocalizer["MenuBar.File.Open.FromWebsite"], clickAction: () =>
                {
                    logPicker?.PickFromWebsite();
                });
            }
            IEnumerable<MenuItem> GetFileMenuItems()
            {
                yield return new MenuItem(stringLocalizer["MenuBar.File.Open"], GetOpenMenuItems());
                yield return new MenuItem(stringLocalizer["MenuBar.File.Scenario"], clickAction: async () =>
                {
                    await scenarioOptions.OpenScenarioDialog();
                });
                yield return new MenuItem(stringLocalizer["MenuBar.Exit"], clickAction: () =>
                {
                    App.Current.CloseWindow(App.Current.Windows[0]);
                });
            }
            _menuItems.Add(new MenuItem(stringLocalizer["MenuBar.File"], GetFileMenuItems().ToArray()));
            IEnumerable<MenuItem> GetToolsMenuItems()
            {
                yield return new MenuItem(stringLocalizer["MenuBar.Tools.Options"]);
            }
            _menuItems.Add(new MenuItem(stringLocalizer["MenuBar.Tools"], GetToolsMenuItems()));
            _menuItems.Add(new MenuItem(stringLocalizer["MenuBar.Help"]));
        }
        public IEnumerable<MenuItem> GetMenuItems()
        {
            return _menuItems;
        }
        public class MenuItem
        {
            private Action _clickAction;
            public string Title { get; }
            public IEnumerable<MenuItem> Items { get; }

            public MenuItem(string title, IEnumerable<MenuItem> items = null, Action clickAction = null)
            {
                Title = title;
                Items = items;
                _clickAction = clickAction;
            }

            public void OnClick()
            {
                _clickAction?.Invoke();
            }
        }
    }
}
