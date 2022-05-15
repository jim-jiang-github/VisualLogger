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
using VisualLogger.InterfaceImplModules.SceneOptions;

namespace VisualLogger
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            //CommandRunnerService commandRunnerService = new CommandRunnerService();
            //string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scenes");
            //commandRunnerService.Init("git", directory)
            //    //.Run("clone --depth 1 git@git.ringcentral.com:jim.jiang/visuallogger.options.git .")
            //    .Command("init")
            //    .Command("remote add origin git@git.ringcentral.com:jim.jiang/visuallogger.options.git")
            //    .Command("fetch")
            //    .Command("branch -a")
            //    .Run()
            //    ;

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
            builder.Services.AddSingleton<CommandRunnerService>();
            builder.Services.AddSingleton<DialogService>();
            builder.Services.AddSingleton<MenuBarService>();
            builder.Services.AddSingleton<ILogPicker, LogPickerLocalFiles>();
            builder.Services.AddSingleton<IWebsitePicker, LogDownloaderFromUrl>();
            builder.Services.AddSingleton<ISceneOptions, SceneOptionsFromGit>();
#if WINDOWS
            builder.Services.AddTransient<IFolderPicker, Platforms.Windows.FolderPicker>();
#elif MACCATALYST
		builder.Services.AddTransient<IFolderPicker, Platforms.MacCatalyst.FolderPicker>();
#endif

            return builder.Build();
        }
    }
}