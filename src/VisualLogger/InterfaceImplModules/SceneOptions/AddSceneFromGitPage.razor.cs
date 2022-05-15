using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.InterfaceModules;
using VisualLogger.Services;

namespace VisualLogger.InterfaceImplModules.SceneOptions
{
    public partial class AddSceneFromGitPage : ComponentBase
    {
        private const string SCENE_ROOT = "scenes";
        private string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SCENE_ROOT);
        [Inject]
        public CommandRunnerService CommandRunner { get; set; }
        [Parameter]
        public string WaterMark { get; set; }
        [Parameter]
        public string GitRepo { get; set; }
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
        public async Task OnClose()
        {
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        public async Task Click()
        {
            var result = await CommandRunner.Init("git", directory)
                .Command("init")
                .Command($"remote add origin {GitRepo}")
                .Command("fetch")
                .Command("branch -a", true)
                .Run();
        }
    }

}
