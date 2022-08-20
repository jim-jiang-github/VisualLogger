using Toolbelt.Blazor.HotKeys;

namespace VisualLogger.Viewer.Data
{
    public interface IHotkeysable : IDisposable
    {
        string Page { get; }
        IEnumerable<HotkeyItem> Keys { get; }

        public void AddHotkeys(HotKeys hotKeys) 
        {
            //foreach (var keys in Keys)
            //{
            //    hotKeys.CreateContext().Add(ModKeys.Ctrl, Keys.S, () => this.AppBarNavIconClick());
            //}
        }
        public void Dispose()
        {
            foreach (var key in Keys)
            {
            }
        }
    }
}
