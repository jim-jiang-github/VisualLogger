using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.InterfaceModules;
using VisualLogger.Resources.Languages;

namespace VisualLogger.Services
{
    public class MenuBarService
    {
        private readonly IStringLocalizer<Strings> _stringLocalizer;
        private readonly ILogPicker _logPicker;
        private List<MenuItem> _menuItems = new List<MenuItem>();
        public MenuBarService(IStringLocalizer<Strings> stringLocalizer,
            ILogPicker logPicker)
        {
            _stringLocalizer = stringLocalizer;
            _logPicker = logPicker;
            IEnumerable<MenuItem> GetOpenMenuItems()
            {
                yield return new MenuItem(_stringLocalizer["MenuBar.File.Open.FromFiles"], clickAction: () =>
                {
                    _logPicker?.PickFromFile();
                });
                yield return new MenuItem(_stringLocalizer["MenuBar.File.Open.FromFolder"], clickAction: () =>
                {
                    _logPicker?.PickFromFolder();
                });
                yield return new MenuItem(_stringLocalizer["MenuBar.File.Open.FromWebsite"], clickAction: () =>
                {
                    _logPicker?.PickFromWebsite();
                });
            }
            IEnumerable<MenuItem> GetFileMenuItems()
            {
                yield return new MenuItem(_stringLocalizer["MenuBar.File.Open"], GetOpenMenuItems());
                yield return new MenuItem(_stringLocalizer["MenuBar.Exit"], clickAction: () =>
                {
                    App.Current.CloseWindow(App.Current.Windows[0]);
                });
            }
            _menuItems.Add(new MenuItem(_stringLocalizer["MenuBar.File"], GetFileMenuItems().ToArray()));
            _menuItems.Add(new MenuItem(_stringLocalizer["MenuBar.Help"]));
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
