namespace VisualLogger.Viewer.Web.ViewModels
{
    public class MainLayoutViewModel
    {
        public ScenarioOptionsViewModel ScenarioOptions { get; }

        public MainLayoutViewModel(ScenarioOptionsViewModel scenarioOptions)
        {
            ScenarioOptions = scenarioOptions;
        }
    }
}
