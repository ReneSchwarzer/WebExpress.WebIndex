using CommunityToolkit.Maui.Views;
using WebExpress.WebIndex.Studio.Model;
using WebExpress.WebIndex.Studio.Popups;

namespace WebExpress.WebIndex.Studio.Pages;

public partial class ProjectPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectPage"/> class.
    /// </summary>
    public ProjectPage()
    {
        InitializeComponent();

        BindingContext = App.ViewModel;
    }

    private void OnAddClicked(object sender, EventArgs e)
    {

        var project = App.ViewModel.CreatetProject();
        App.ViewModel.OpenProjectConfigPageCommand.Execute(null);
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        var project = ProjectList.SelectedItem as Project;
        App.ViewModel.OpenProjectConfigPageCommand.Execute(project);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var popup = new ConfirmDeleteProjectPopup();
        var result = await this.ShowPopupAsync(popup, CancellationToken.None);

        if (result is bool boolResult)
        {
            if (boolResult)
            {
                var project = ProjectList.SelectedItem as Project;
                App.ViewModel.DeleteProject(project);
            }
        }
    }

    private void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        var project = ProjectList.SelectedItem as Project;

        App.ViewModel.OpenObjectTypePageCommand.Execute(project);
    }
}