using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VisualLogger;
using VisualLogger.Viewer;
using VisualLogger.Viewer.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Main>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddVisualLoggerWeb();
var build = builder.Build();
Global.ServiceProvider = build.Services;
Global.Init();
await build.RunAsync();
