using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.LogDownloaders
{
    public partial class LogDownloaderPage : ComponentBase, IResultDialog
    {
        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public int Value { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public EventCallback<int> ValueChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public async Task OnClose(DialogResult result)
        {
            if (result == DialogResult.Yes)
            {
                if (ValueChanged.HasDelegate)
                {
                    await ValueChanged.InvokeAsync(Value);
                }
            }
        }
    }

}
