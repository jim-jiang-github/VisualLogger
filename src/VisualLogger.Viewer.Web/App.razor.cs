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
            Global.UIChanged += Global_UIChanged;
        }

        private void Global_UIChanged(object? sender, EventArgs e)
        {
            StateHasChanged();
        }

        public void Dispose()
        {
            Global.UIChanged -= Global_UIChanged;
        }

        private async Task CustomErrorHandleAsync(Exception exception)
        {
            Log.Error(exception, "Gloabl error!");
            await Task.CompletedTask;
        }
    }
}
