using WebExpress.WebIndex.Studio.Model;

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

    private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var project = e.SelectedItem as Project;

        App.ViewModel.OpenObjectTypePageCommand.Execute(project);
    }
}