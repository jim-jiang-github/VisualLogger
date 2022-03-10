using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Viewer.Web.Interfaces
{
    public interface IScenarioOptions
    {
        Task OpenScenarioDialog();
    }
    public class ScenarioOptions : IScenarioOptions
    {
        public Task OpenScenarioDialog()
        {
            return Task.CompletedTask;
        }
    }
}
