namespace VisualLogger.Viewer.Web
{
    public class Global
    {
        public static event EventHandler? StateHasChanged;

        public static IServiceProvider? ServiceProvider { get; set; }

        public static void RefreshUI()
        {
            StateHasChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
