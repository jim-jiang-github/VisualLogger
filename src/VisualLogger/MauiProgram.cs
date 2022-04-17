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
using Serilog.Extensions.Logging;
using Serilog;
using Serilog.Events;
using VisualLogger.Services;

namespace VisualLogger
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "log.txt"),
                restrictedToMinimumLevel: LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss zzz}[{Level:u3}]{Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                                .ConfigureLifecycleEvents(events =>
                                {
#if WINDOWS
                                    events.AddWindows(windows => windows
                                           .OnWindowCreated(xamlWindow =>
                                           {
                                               //var window = xamlWindow as MauiWinUIWindow;
                                               //var _hwndMain = window!.WindowHandle;
                                               //window.ExtendsContentIntoTitleBar = false;
                                           }));
#endif
                                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            builder.Logging.AddSerilog(Log.Logger, dispose: true);
            builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, dispose: true));
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
            builder.Services.AddSingleton<ILogPicker>(new LogPickerLocalFiles());
            builder.Services.AddSingleton<ILogDownloader, LogDownloaderFromUrl>();

            return builder.Build();
        }
    }
}