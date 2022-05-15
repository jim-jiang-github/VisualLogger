using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Services;

namespace VisualLogger.WinUI
{
    internal class WinUIMenuBar : Microsoft.UI.Xaml.Controls.MenuBar
    {
        public WinUIMenuBar()
        {
            VerticalContentAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top;
            Padding = new Microsoft.UI.Xaml.Thickness(0);
            var menuBarService = App.Current.Services.GetService<MenuBarService>();
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
                if (item.Items != null)
                {
                    foreach (var subItem in item.Items)
                    {
                        menuBarItem.Items.Add(LoadMenuItem(subItem));
                    }
                }
                Items.Add(menuBarItem);
            }
        }

        private Microsoft.UI.Xaml.Controls.MenuFlyoutItemBase LoadMenuItem(MenuBarService.MenuItem menuItem)
        {
            if (menuItem == null)
            {
                return null;
            }
            if (menuItem.Items == null)
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
                foreach (var subItem in menuItem.Items)
                {
                    menuBarItem.Items.Add(LoadMenuItem(subItem));
                }
                return menuBarItem;
            }
        }
    }
}
