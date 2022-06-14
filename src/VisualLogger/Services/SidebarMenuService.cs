using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Data;

namespace VisualLogger.Services
{
    internal class SidebarMenuService
    {
        public IEnumerable<SidebarMenu> Menus { get; }

        public SidebarMenuService()
        {
            Menus = new SidebarMenu[]
            {
                new SidebarMenu()
                {
                    Name = "Overview",
                    Path = "/",
                    Icon = "&#xe88a"
                },
                new SidebarMenu()
                {
                    Name = "XXXX",
                    Path = "/counter",
                    Icon = "&#xe88a"
                }
            };
        }
    }
}
