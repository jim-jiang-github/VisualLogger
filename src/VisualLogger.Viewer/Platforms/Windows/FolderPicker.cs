using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Viewer.Web.Interfaces;
using WindowsFolderPicker = Windows.Storage.Pickers.FolderPicker;

namespace VisualLogger.Viewer.Platforms.Windows
{
    internal class FolderPicker : IFolderPicker
    {
        public async Task<string?> PickFolder()
        {
            var folderPicker = new WindowsFolderPicker();
            // Might be needed to make it work on Windows 10
            folderPicker.FileTypeFilter.Add("*");

            if (App.Current == null || App.Current.Windows.Count <= 0)
            {
                return null;
            }
            var mauiWinUIWindow = App.Current.Windows[0].Handler.PlatformView as MauiWinUIWindow;
            if (mauiWinUIWindow == null)
            {
                return null;
            }
            // Get the current window's HWND by passing in the Window object
            var hwnd = mauiWinUIWindow.WindowHandle;

            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            var result = await folderPicker.PickSingleFolderAsync();

            return result?.Path;
        }
    }
}
