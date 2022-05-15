using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.InterfaceModules;
using VisualLogger.Resources.Languages;

namespace VisualLogger.InterfaceImplModules.SceneOptions
{
    internal class SceneOptionsFromGit : ISceneOptions
    {
        private DialogService _dialogService;
        private IStringLocalizer<Strings> _localizer;

        public SceneOptionsFromGit(DialogService dialogService, IStringLocalizer<Strings> localizer)
        {
            _dialogService = dialogService;
            _localizer = localizer;
        }

        public async void AddScene()
        {
            var op = new DialogOption()
            {
                Title = _localizer["AddScenePage.Title"].Value,
                ShowFooter = false,
                BodyContext = 1
            };
            op.BodyTemplate = BootstrapDynamicComponent.CreateComponent<AddSceneFromGitPage>(new Dictionary<string, object?>
            {
                //[nameof(AddSceneFromGitPage.OnClose)] = new Action(async () => await op.Dialog.Close())
            }).Render();
            await _dialogService.Show(op);
        }

        public void RemoveCurrentScene()
        {
        }
    }
}
