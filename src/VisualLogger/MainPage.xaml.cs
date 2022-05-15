namespace VisualLogger
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new MainPageViewModel();
        }

        public class MainPageViewModel
        {
            public string MenuText { get; } = "mmmmmmmm";
        }
    }
}