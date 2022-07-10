using VisualLogger.Shared.Data;
using VisualLogger.Shared.Services;

namespace VisualLogger.Maui
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            MainPage = new MainPage();

#if MACCATALYST
            var menuBarService = serviceProvider.GetService<MenuTopBarService>();
            if (menuBarService==null)
            {
                return;
            }
            LoadMenuItem(menuBarService);
#endif
        }
#if MACCATALYST
        private void LoadMenuItem(MenuTopBarService menuBarService)
        {
            foreach (var item in menuBarService.GetMenuItems())
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
        private MenuFlyoutItem LoadMenuItem(MenuTopBarItem menuItem)
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