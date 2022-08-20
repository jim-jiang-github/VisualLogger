using Toolbelt.Blazor.HotKeys;
using VisualLogger.Viewer.Components;

namespace VisualLogger.Viewer.Data
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
