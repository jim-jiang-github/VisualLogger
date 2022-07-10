using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Shared.Data;
using VisualLogger.Shared.Services;

namespace VisualLogger.Maui.WinUI
{
    internal class WinUIMenuBar : Microsoft.UI.Xaml.Controls.MenuBar
    {
        public WinUIMenuBar()
        {
            VerticalContentAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top;
            Padding = new Microsoft.UI.Xaml.Thickness(0);
            var menuBarService = App.Current.Services.GetService<MenuTopBarService>();
            if (menuBarService == null)
            {
                return;
            }
            LoadMenuItem(menuBarService);
        }
        private void LoadMenuItem(MenuTopBarService menuBarService)
        {
            foreach (var item in menuBarService.GetMenuItems())
            {
                var menuBarItem = new Microsoft.UI.Xaml.Controls.MenuBarItem()
                {
                    RenderTransform = new Microsoft.UI.Xaml.Media.TranslateTransform()
                    {
                        Y = 2
                    },
                    Height = 28,
                    VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Bottom,
                    Title = item.Title,
                };
                if (item.MenuItems != null)
                {
                    foreach (var subItem in item.MenuItems)
                    {
                        menuBarItem.Items.Add(LoadMenuItem(subItem));
                    }
                }
                Items.Add(menuBarItem);
            }
        }
        private Microsoft.UI.Xaml.Controls.MenuFlyoutItemBase LoadMenuItem(MenuTopBarItem menuItem)
        {
            if (menuItem.MenuItems == null)
            {
                var menuFlyoutItem = new Microsoft.UI.Xaml.Controls.MenuFlyoutItem()
                {
                    Text = menuItem.Title
                };
                menuFlyoutItem.Click += (s, e) => menuItem.OnClick();
                return menuFlyoutItem;
            }
            else
            {
                var menuBarItem = new Microsoft.UI.Xaml.Controls.MenuFlyoutSubItem()
                {
                    Text = menuItem.Title,
                };
                foreach (var subItem in menuItem.MenuItems)
                {
                    menuBarItem.Items.Add(LoadMenuItem(subItem));
                }
                return menuBarItem;
            }
        }
    }
}
