using Microsoft.AspNetCore.Components.WebView.Maui;
using VisualLogger.Data;
using VisualLogger.Extensions;
using VisualLogger.InterfaceModules;
using VisualLogger.Maui.InterfaceImplModules;
using VisualLogger.Maui.Services;

namespace VisualLogger.Maui
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
            builder.Services.AddVisualLogger();
            builder.Services.AddSingleton<ILogPicker, MauiLogPicker>();
            builder.Services.AddSingleton<MenuBarService>();
#if WINDOWS
            builder.Services.AddTransient<IFolderPicker, Platforms.Windows.FolderPicker>();
#elif MACCATALYST
		builder.Services.AddTransient<IFolderPicker, Platforms.MacCatalyst.FolderPicker>();
#endif
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            builder.Services.AddSingleton<WeatherForecastService>();

            return builder.Build();
        }
    }
}