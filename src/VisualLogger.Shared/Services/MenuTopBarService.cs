using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Scenarios;
using VisualLogger.Shared.Resources.Languages;
using VisualLogger.Shared.Data;

namespace VisualLogger.Shared.Services
{
    public class MenuTopBarService
    {
        private readonly List<MenuTopBarItem> _menuItems = new List<MenuTopBarItem>();
        public MenuTopBarService(IStringLocalizer<Strings> stringLocalizer,
            IServiceProvider serviceProvider,
            Scenario scenario,
            IScenarioOptions scenarioOptions)
        {
            IEnumerable<MenuTopBarItem> GetOpenMenuItems()
            {
                IFilesPicker? filesPicker = serviceProvider.GetService<IFilesPicker>();
                if (filesPicker != null)
                {
                    yield return new MenuTopBarItem(stringLocalizer["MenuBar.File.Open.FromFiles"], clickAction: async () =>
                    {
                        var files = await filesPicker.PickFiles();
                        scenario.LoadLogFiles(files.ToArray());
                    });
                }
                IFolderPicker? folderPicker = serviceProvider.GetService<IFolderPicker>();
                if (folderPicker != null)
                {
                    yield return new MenuTopBarItem(stringLocalizer["MenuBar.File.Open.FromFolder"], clickAction: () =>
                    {
                        folderPicker.PickFolder();
                    });
                }
                yield return new MenuTopBarItem(stringLocalizer["MenuBar.File.Open.FromWebsite"], clickAction: () =>
                {
                    GC.Collect();
                });
            }
            IEnumerable<MenuTopBarItem> GetFileMenuItems()
            {
                yield return new MenuTopBarItem(stringLocalizer["MenuBar.File.Open"], GetOpenMenuItems());
                yield return new MenuTopBarItem(stringLocalizer["MenuBar.File.Scenario"], clickAction: async () =>
                {
                    await scenarioOptions.OpenScenarioDialog();
                });
                yield return new MenuTopBarItem(stringLocalizer["MenuBar.Exit"], clickAction: () =>
                {
                });
            }
            _menuItems.Add(new MenuTopBarItem(stringLocalizer["MenuBar.File"], GetFileMenuItems().ToArray()));
            IEnumerable<MenuTopBarItem> GetToolsMenuItems()
            {
                yield return new MenuTopBarItem(stringLocalizer["MenuBar.Tools.Options"]);
            }
            _menuItems.Add(new MenuTopBarItem(stringLocalizer["MenuBar.Tools"], GetToolsMenuItems()));
            _menuItems.Add(new MenuTopBarItem(stringLocalizer["MenuBar.Help"]));
        }
        public IEnumerable<MenuTopBarItem> GetMenuItems()
        {
            return _menuItems;
        }
    }
}
