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
            Scenario scenario,
            IScenarioOptions scenarioOptions)
        {
            IEnumerable<MenuTopBarItem> GetOpenMenuItems()
            {
                yield return new MenuTopBarItem(stringLocalizer["MenuBar.File.Open.FromFiles"], clickAction: async () =>
                {
                });
                yield return new MenuTopBarItem(stringLocalizer["MenuBar.File.Open.FromFolder"], clickAction: () =>
                {
                });
                yield return new MenuTopBarItem(stringLocalizer["MenuBar.File.Open.FromWebsite"], clickAction: () =>
                {
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
