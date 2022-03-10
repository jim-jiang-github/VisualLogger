using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Shared.Data;

namespace VisualLogger.Shared.Services
{
    internal class MenuSideBarService
    {
        public IEnumerable<MenuSideBarItem> MenuItems { get; }

        public MenuSideBarService()
        {
            MenuItems = new MenuSideBarItem[]
            {
                new MenuSideBarItem()
                {
                    Name = "Overview",
                    Path = "/",
                    Icon = "&#xe88a"
                },
                new MenuSideBarItem()
                {
                    Name = "XXXX",
                    Path = "/counter",
                    Icon = "&#xe88a"
                },
                new MenuSideBarItem()
                {
                    Name = "XXXX",
                    Path = "/counter",
                    Icon = "&#xe88a"
                },
                new MenuSideBarItem()
                {
                    Name = "XXXX",
                    Path = "/counter",
                    Icon = "&#xe88a"
                },
                new MenuSideBarItem()
                {
                    Name = "XXXX",
                    Path = "/counter",
                    Icon = "&#xe88a"
                }
            };
        }
    }
}
