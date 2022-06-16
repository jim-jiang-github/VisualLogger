using VisualLogger.Maui.Services;

namespace VisualLogger.Maui
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            MainPage = new MainPage();

#if MACCATALYST
            var menuBarService = serviceProvider.GetService<MenuBarService>();
            LoadMenuItem(menuBarService);
#endif
        }
#if MACCATALYST
        private void LoadMenuItem(MenuBarService menuBarService)
        {
            foreach (var item in menuBarService.GetMenuItems())
            {
                var menuBarItem = new MenuBarItem()
                {
                    Text = item.Title,
                };
                if (item.Items != null)
                {
                    foreach (var subItem in item.Items)
                    {
                        menuBarItem.Add(LoadMenuItem(subItem));
                    }
                }
                MainPage.MenuBarItems.Add(menuBarItem);
            }
        }
        private MenuFlyoutItem LoadMenuItem(MenuBarService.MenuItem menuItem)
        {
            if (menuItem == null)
            {
                return null;
            }
            if (menuItem.Items == null)
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
                foreach (var subItem in menuItem.Items)
                {
                    menuBarItem.Add(LoadMenuItem(subItem));
                }
                return menuBarItem;
            }
        }
#endif
    }
}