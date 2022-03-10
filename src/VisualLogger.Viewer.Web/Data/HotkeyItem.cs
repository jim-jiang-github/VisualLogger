using Toolbelt.Blazor.HotKeys;
using VisualLogger.Viewer.Web.Components;

namespace VisualLogger.Viewer.Web.Data
{
    public class HotkeyItem
    {
        public string Name { get; }
        public Action Action { get; }

        public HotkeyItem(HotKeys hotKeys, string name, Action action)
        {
            Name = name;
            Action = action;
        }
    }
}
