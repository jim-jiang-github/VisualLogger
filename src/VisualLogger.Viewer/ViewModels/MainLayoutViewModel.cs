namespace VisualLogger.Viewer.ViewModels
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
