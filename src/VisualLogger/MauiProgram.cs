using Microsoft.AspNetCore.Components.WebView.Maui;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components.Web;

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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddBlazorWebView();
            builder.Services.AddBootstrapBlazor();
            builder.Services.AddSingleton<IErrorBoundaryLogger>(new C());

            return builder.Build();
        }
    }

    public class C : IErrorBoundaryLogger
    {
        public ValueTask LogErrorAsync(Exception exception)
        {
            return new ValueTask();
        }
    }
}