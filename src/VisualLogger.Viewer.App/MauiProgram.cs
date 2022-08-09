using Microsoft.AspNetCore.Components.WebView.Maui;
using VisualLogger.Viewer.Web.Extensions;
using VisualLogger.Viewer.Web;
using VisualLogger.Viewer.Web.Interfaces;
using VisualLogger.Viewer.App.InterfaceImpls;

namespace VisualLogger.Viewer.App
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
            builder.Services.AddSingleton<IFilesPicker, MauiFilesPicker>();
#if WINDOWS
            builder.Services.AddSingleton<IFolderPicker, Platforms.Windows.FolderPicker>();
#elif MACCATALYST
		    builder.Services.AddSingleton<IFolderPicker, Platforms.MacCatalyst.FolderPicker>();
#endif
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            builder.Services.AddVisualLoggerWeb();
            var build = builder.Build();
            Global.ServiceProvider = build.Services;
            return build;
        }
    }
}