using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Utils;
using VisualLogger.Viewer.Components;

namespace VisualLogger.Viewer
{
    public partial class Main : IDisposable
    {
        private bool _isShowReport = false;
        private VisualLoggerErrorBoundary? _errorBoundary;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _errorBoundary = new VisualLoggerErrorBoundary();
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
    }
}
