using Microsoft.Extensions.DependencyInjection;
using VisualLogger.Viewer.Web.Interfaces;
using VisualLogger.Scenarios;
using VisualLogger.Viewer.Web.Localization;
using VisualLogger.Viewer.Web.ViewModels;

namespace VisualLogger.Viewer.Web.Services
{
    public class MenuBarService
    {
        #region Internal Class

        public class MenuBarItem
        {
            private Action? _clickAction;
            public string Title { get; }
            public IEnumerable<MenuBarItem>? MenuItems { get; }

            public MenuBarItem(string title, IEnumerable<MenuBarItem>? menuItems = null, Action? clickAction = null)
            {
                Title = title;
                MenuItems = menuItems;
                _clickAction = clickAction;
            }

            public void OnClick()
            {
                _clickAction?.Invoke();
            }
        }

        #endregion

        private readonly List<MenuBarItem> _menuBarItems = new List<MenuBarItem>();

        public IEnumerable<MenuBarItem> MenuItems => _menuBarItems;

        public IEnumerable<MenuBarItem> AllMenuItems => GetAllMenuItems(MenuItems);

        public MenuBarService(IServiceProvider serviceProvider,
            Scenario scenario,
            ScenarioOptionsViewModel scenarioOptions)
        {
            IEnumerable<MenuBarItem> GetOpenMenuItems()
            {
                IFilesPicker? filesPicker = serviceProvider.GetService<IFilesPicker>();
                if (filesPicker != null)
                {
                    yield return new MenuBarItem(I18nKeys.MenuBar.FileSub.OpenSub.FormFiles, clickAction: async () =>
                    {
                        var files = await filesPicker.PickFiles();
                        scenario.LoadLogFiles(files.ToArray());
                    });
                }
                IFolderPicker? folderPicker = serviceProvider.GetService<IFolderPicker>();
                if (folderPicker != null)
                {
                    yield return new MenuBarItem(I18nKeys.MenuBar.FileSub.OpenSub.FromFolder, clickAction: () =>
                    {
                        folderPicker.PickFolder();
                    });
                }
                yield return new MenuBarItem(I18nKeys.MenuBar.FileSub.OpenSub.FromWebsite, clickAction: () =>
                {
                    GC.Collect();
                });
            }
            IEnumerable<MenuBarItem> GetFileMenuItems()
            {
                yield return new MenuBarItem(I18nKeys.MenuBar.FileSub.Open, GetOpenMenuItems());
                yield return new MenuBarItem(I18nKeys.MenuBar.FileSub.Scenario, clickAction: () =>
                {
                    scenarioOptions.IsOpen = true;
                });
                yield return new MenuBarItem(I18nKeys.MenuBar.FileSub.Exit, clickAction: () =>
                {
                });
            }
            _menuBarItems.Add(new MenuBarItem(I18nKeys.MenuBar.File, GetFileMenuItems().ToArray()));
            IEnumerable<MenuBarItem> GetToolsMenuItems()
            {
                yield return new MenuBarItem(I18nKeys.MenuBar.ToolsSub.Options);
            }
            _menuBarItems.Add(new MenuBarItem(I18nKeys.MenuBar.Tools, GetToolsMenuItems()));
            _menuBarItems.Add(new MenuBarItem(I18nKeys.MenuBar.Help));
        }

        private IEnumerable<MenuBarItem> GetAllMenuItems(IEnumerable<MenuBarItem> menuBarItems)
        {
            List<MenuBarItem> allMenuBarItems = new List<MenuBarItem>();
            foreach (var menuBarItem in menuBarItems)
            {
                if (menuBarItem.MenuItems != null)
                {
                    allMenuBarItems.AddRange(GetAllMenuItems(menuBarItem.MenuItems));
                }
                else
                {
                    allMenuBarItems.Add(menuBarItem);
                }
            }
            return allMenuBarItems;
        }
    }
}
