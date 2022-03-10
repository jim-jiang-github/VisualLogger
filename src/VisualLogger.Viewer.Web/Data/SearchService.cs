using Microsoft.AspNetCore.Components;

namespace VisualLogger.Viewer.Web.Data
{
    public class SearchService
    {
        public event EventHandler<bool>? ShowFilterDialog;

        public bool IsShow { get; set; }

        public void Show()
        {
            IsShow = true;
            ShowFilterDialog?.Invoke(this, IsShow);
        }
    }
}
