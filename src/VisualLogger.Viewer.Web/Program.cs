using BlazorComponent.I18n;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Reflection;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Toolbelt.Blazor.HotKeys;
using VisualLogger.Viewer.Web;
using VisualLogger.Viewer.Web.Data;
using VisualLogger.Viewer.Web.Extensions;
using VisualLogger.Viewer.Web.Hotkeys;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddVisualLoggerWeb();
var build = builder.Build();
Global.ServiceProvider = build.Services;
await build.RunAsync();