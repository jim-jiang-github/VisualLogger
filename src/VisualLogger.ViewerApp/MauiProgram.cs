using Microsoft.AspNetCore.Components.WebView.Maui;
using VisualLogger.Storage;
using VisualLogger.Viewer.Extensions;
using VisualLogger.ViewerApp.Data;

namespace VisualLogger.ViewerApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            builder.Services.AddVisualLoggerWeb();
            //builder.Services.AddScoped<IFileStorage, MauiFileStorage>();
            var build = builder.Build();
            Global.ServiceProvider = build.Services;
            Global.Init();
            return build;
        }
    }
}