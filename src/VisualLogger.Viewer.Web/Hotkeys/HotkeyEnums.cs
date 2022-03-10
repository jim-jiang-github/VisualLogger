using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using VisualLogger.Viewer.Web.Localization;

namespace VisualLogger.Viewer.Web.Hotkeys
{
    public static class HotkeyEnums
    {
        public abstract class HotkeyItem
        {
            public abstract string Name { get; }
        }
        private class HotkeyItemHighlight : HotkeyItem
        {
            public override string Name => I18nKeys.Hotkeys.Highlight;
        }
        private class HotkeyItemFindNextHighlight : HotkeyItem
        {
            public override string Name => I18nKeys.Hotkeys.NextHighlight;
        }
        public static HotkeyItem Highlight { get; } = new HotkeyItemHighlight();
        public static HotkeyItem FindNextHighlight { get; } = new HotkeyItemHighlight();
    }
}