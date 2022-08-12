using VisualLogger.Viewer.Web;
using VisualLogger.Viewer.Web.Services;

namespace VisualLogger.Viewer.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

#if MACCATALYST
            var menuBarService = Global.ServiceProvider?.GetService<MenuBarService>();
            if (menuBarService == null)
            {
                return;
            }
            LoadMenuItem(menuBarService);
#endif
        }

#if MACCATALYST
        private void LoadMenuItem(MenuBarService menuBarService)
        {
            foreach (var item in menuBarService.MenuItems)
            {
                var menuBarItem = new MenuBarItem()
                {
                    Text = item.Title,
                };
                if (item.MenuItems != null)
                {
                    foreach (var subItem in item.MenuItems)
                    {
                        menuBarItem.Add(LoadMenuItem(subItem));
                    }
                }
                MainPage?.MenuBarItems.Add(menuBarItem);
            }
        }
        private MenuFlyoutItem LoadMenuItem(MenuBarService.MenuBarItem menuItem)
        {
            if (menuItem.MenuItems == null)
            {
                var menuFlyoutItem = new MenuFlyoutItem()
                {
                    Text = menuItem.Title
                };
                menuFlyoutItem.Clicked += (s, e) => menuItem.OnClick();
                return menuFlyoutItem;
            }
            else
            {
                var menuBarItem = new MenuFlyoutSubItem()
                {
                    Text = menuItem.Title,
                };
                foreach (var subItem in menuItem.MenuItems)
                {
                    menuBarItem.Add(LoadMenuItem(subItem));
                }
                return menuBarItem;
            }
        }
#endif
    }
}