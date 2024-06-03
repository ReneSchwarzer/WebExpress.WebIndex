using WebExpress.WebIndex.Studio.Pages;

namespace WebExpress.WebIndex.Studio
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ProjectPage), typeof(ProjectPage));
            Routing.RegisterRoute(nameof(ProjectConfigPage), typeof(ProjectConfigPage));
            Routing.RegisterRoute(nameof(ObjectTypePage), typeof(ObjectTypePage));
        }
    }
}
