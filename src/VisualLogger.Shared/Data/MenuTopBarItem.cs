using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Shared.Data
{
    public class MenuTopBarItem
    {
        private Action? _clickAction;
        public string Title { get; }
        public IEnumerable<MenuTopBarItem>? MenuItems { get; }

        public MenuTopBarItem(string title, IEnumerable<MenuTopBarItem>? menuItems = null, Action? clickAction = null)
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
}
