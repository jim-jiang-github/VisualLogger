using Microsoft.AspNetCore.Components.WebView.Maui;
using BootstrapBlazor.Components;
using BootstrapBlazor.Localization;
using Microsoft.AspNetCore.Components.Web;
using VisualLogger.LogModules;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.JSInterop;
using System.Globalization;
using VisualLogger.InterfaceModules;
using VisualLogger.InterfaceImplModules.LogDownloaders;
using VisualLogger.LogPickers;
using Microsoft.Extensions.Logging;
using VisualLogger.Services;

namespace VisualLogger
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                                .ConfigureLifecycleEvents(events =>
                                {
#if WINDOWS
                                    events.AddWindows(windows => windows
                                           .OnWindowCreated(xamlWindow =>
                                           {
                                               var window = xamlWindow as MauiWinUIWindow;
                                               //var _hwndMain = window!.WindowHandle;
                                               window.ExtendsContentIntoTitleBar = true;
                                           }));
#endif
                                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            //builder.Services.AddLogging();
            builder.Services.AddLocalization();
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            builder.Services.AddBootstrapBlazor();
            builder.Services.AddSingleton<FileDownloadService>();
            builder.Services.AddSingleton<IErrorBoundaryLogger>(new ErrorBoundaryLoggerImpl());
            builder.Services.AddSingleton<DialogService>();
            builder.Services.AddSingleton<MenuBarService>();
            builder.Services.AddSingleton<ILogPicker, LogPickerLocalFiles>();
            builder.Services.AddSingleton<IWebsitePicker, LogDownloaderFromUrl>();
#if WINDOWS
            builder.Services.AddTransient<IFolderPicker, Platforms.Windows.FolderPicker>();
#elif MACCATALYST
		builder.Services.AddTransient<IFolderPicker, Platforms.MacCatalyst.FolderPicker>();
#endif

            return builder.Build();
        }
    }
}