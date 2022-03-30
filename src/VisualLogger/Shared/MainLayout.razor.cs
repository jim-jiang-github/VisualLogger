using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Collections.Generic;

namespace VisualLogger.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class MainLayout
    {
        private bool UseTabSet { get; set; } = true;

        private string Theme { get; set; } = "";

        private bool IsOpen { get; set; }

        private bool IsFixedHeader { get; set; } = true;

        private bool IsFixedFooter { get; set; } = true;

        private bool IsFullSide { get; set; } = true;

        private bool ShowFooter { get; set; } = true;

        private List<BootstrapBlazor.Components.MenuItem> Menus { get; set; }

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            Menus = GetIconSideMenuItems();
        }

        private static List<BootstrapBlazor.Components.MenuItem> GetIconSideMenuItems()
        {
            var menus = new List<BootstrapBlazor.Components.MenuItem>
            {
                new BootstrapBlazor.Components.MenuItem() { Text = "返回组件库", Icon = "fa fa-fw fa-home", Url = "https://www.blazor.zone/components" },
                new BootstrapBlazor.Components.MenuItem() { Text = "Index", Icon = "fa fa-fw fa-fa", Url = "/" , Match = NavLinkMatch.All},
                new BootstrapBlazor.Components.MenuItem() { Text = "Counter", Icon = "fa fa-fw fa-check-square-o", Url = "/counter" },
                new BootstrapBlazor.Components.MenuItem() { Text = "FetchData", Icon = "fa fa-fw fa-database", Url = "fetchdata" },
                new BootstrapBlazor.Components.MenuItem() { Text = "Table", Icon = "fa fa-fw fa-table", Url = "table" },
                new BootstrapBlazor.Components.MenuItem() { Text = "花名册", Icon = "fa fa-fw fa-users", Url = "users" }
            };

            return menus;
        }
    }
}
