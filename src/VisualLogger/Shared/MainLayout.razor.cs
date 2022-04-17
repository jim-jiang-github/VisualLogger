using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Collections.Generic;
using System.Reflection;

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

        private bool IsCollapsed { get; set; }

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

        /// <summary>
        /// 点击 收缩展开按钮时回调此方法
        /// </summary>
        /// <returns></returns>
        protected async Task CollapseMenu()
        {
            IsCollapsed = !IsCollapsed;
            //if (IsCollapsedChanged.HasDelegate)
            //{
            //    await IsCollapsedChanged.InvokeAsync(IsCollapsed);
            //}

            //if (OnCollapsed != null)
            //{
            //    await OnCollapsed(IsCollapsed);
            //}
        }
        private static List<BootstrapBlazor.Components.MenuItem> GetIconSideMenuItems()
        {
            var openMenus = new List<BootstrapBlazor.Components.MenuItem>
            {
                new BootstrapBlazor.Components.MenuItem() { Text = "Download", Icon = "fa fa-fw fa-home"},
                new BootstrapBlazor.Components.MenuItem() { Text = "Pick", Icon = "fa fa-fw fa-home"}
            };
            var menus = new List<BootstrapBlazor.Components.MenuItem>
            {
                new BootstrapBlazor.Components.MenuItem() { Text = "Open", Icon = "fa fa-fw fa-home"},
                new BootstrapBlazor.Components.MenuItem() { Text = "Index", Icon = "fa fa-fw fa-fa", Url = "/" , Match = NavLinkMatch.All},
                new BootstrapBlazor.Components.MenuItem() { Text = "Counter", Icon = "fa fa-fw fa-check-square-o", Url = "/counter" },
                new BootstrapBlazor.Components.MenuItem() { Text = "FetchData", Icon = "fa fa-fw fa-database", Url = "fetchdata" },
                new BootstrapBlazor.Components.MenuItem() { Text = "Table", Icon = "fa fa-fw fa-table", Url = "table" },
                new BootstrapBlazor.Components.MenuItem() { Text = "花名册", Icon = "fa fa-fw fa-users", Url = "users" }
            };

            return menus;
        }
        public bool IsShowTree { get; set; }
        protected Func<BootstrapBlazor.Components.MenuItem, Task> ClickMenu() => async item =>
        {
            IsShowTree = !IsShowTree;
            StateHasChanged();
        };

        private List<TreeItem> Items { get; set; } = GetTreeItems();

        private Task OnTreeItemClick(TreeItem item)
        {
            return Task.CompletedTask;
        }
        public static List<TreeItem> GetTreeItems()
        {
            var items = new List<TreeItem>
        {
            new TreeItem() { Text = "导航一", Id = "1010" },
            new TreeItem() { Text = "导航二", Id = "1020" },
            new TreeItem() { Text = "导航三", Id = "1030" },

            new TreeItem() { Text = "子菜单一", Id = "1040", ParentId = "1020" },
            new TreeItem() { Text = "子菜单二", Id = "1050", ParentId = "1020" },
            new TreeItem() { Text = "子菜单三", Id = "1060", ParentId = "1020" },

            new TreeItem() { Text = "孙菜单一", Id = "1070", ParentId = "1050" },
            new TreeItem() { Text = "孙菜单二", Id = "1080", ParentId = "1050" },
            new TreeItem() { Text = "孙菜单三", Id = "1090", ParentId = "1050" },

            new TreeItem() { Text = "曾孙菜单一", Id = "1100", ParentId = "1080" },
            new TreeItem() { Text = "曾孙菜单二", Id = "1110", ParentId = "1080" },
            new TreeItem() { Text = "曾孙菜单三", Id = "1120", ParentId = "1080" },

            new TreeItem() { Text = "曾曾孙菜单一", Id = "1130", ParentId = "1100" },
            new TreeItem() { Text = "曾曾孙菜单二", Id = "1140", ParentId = "1100" },
            new TreeItem() { Text = "曾曾孙菜单三", Id = "1150", ParentId = "1100" }
        };

            // 算法获取属性结构数据
            return items.CascadingTree().ToList();
        }
    }
}
