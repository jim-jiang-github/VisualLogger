using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Viewer
{
    public partial class Main : IDisposable
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
