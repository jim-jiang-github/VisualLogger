using Microsoft.Extensions.DependencyInjection;
using VisualLogger.Extensions;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.AspNetCore.Components.Web;
using VisualLogger.Shared.Extensions;
using VisualLogger.Shared;

namespace VisualLogger.WinForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddWindowsFormsBlazorWebView();
            serviceCollection.AddVisualLoggerShared();
#if DEBUG
            serviceCollection.AddBlazorWebViewDeveloperTools();
#endif

            mainBlazorWebView.HostPage = "wwwroot\\index.html";
            mainBlazorWebView.Services = serviceCollection.BuildServiceProvider();
            mainBlazorWebView.RootComponents.Add<App>("#app");

        }
    }
}