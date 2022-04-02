using Microsoft.AspNetCore.Components.WebView.Maui;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components.Web;
using VisualLogger.LogModules;
using InterfaceModules;
using Microsoft.Maui.LifecycleEvents;

namespace VisualLogger
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .RegisterBlazorMauiWebView()
                .UseMauiApp<App>()
//                .ConfigureLifecycleEvents(events =>
//                {
//#if WINDOWS
//                  events.AddWindows(windows => windows
//                         .OnWindowCreated(window =>
//                         {
//                         }));
//#endif
//                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            builder.Services.AddBlazorWebView();
            builder.Services.AddBootstrapBlazor();
            builder.Services.AddSingleton<IErrorBoundaryLogger>(new ErrorBoundaryLoggerImpl());
            builder.Services.AddSingleton<ILogPicker>(new LogPickers.LogPickerLocalFiles());
            builder.Services.AddSingleton<ILogDownloader>(new LogDownloaders.LogDownloaderUrl());

            return builder.Build();
        }
    }
}