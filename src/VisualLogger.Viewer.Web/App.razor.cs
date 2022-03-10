using BlazorComponent.I18n;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using VisualLogger.Viewer.Web.Data;

namespace VisualLogger.Viewer.Web
{
    public partial class App : IDisposable
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Global.StateHasChanged += Global_StateHasChanged;
        }

        private void Global_StateHasChanged(object? sender, EventArgs e)
        {
            StateHasChanged();
        }

        public void Dispose()
        {
            Global.StateHasChanged -= Global_StateHasChanged;
        }

        private async Task CustomErrorHandleAsync(Exception exception)
        {
            Log.Error(exception, "Gloabl error!");
            await Task.CompletedTask;
        }
    }
}
