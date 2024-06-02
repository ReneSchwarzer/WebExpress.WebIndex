using WebExpress.WebIndex.Studio.Model;

namespace WebExpress.WebIndex.Studio
{
    public partial class App : Application
    {
        public static ViewModel ViewModel { get; } = new ViewModel();

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
