namespace WebExpress.WebIndex.Studio.Pages;

[QueryProperty(nameof(ProjectId), "id")]
public partial class ProjectConfigPage : ContentPage
{
    /// <summary>
    /// Define a property to store the value of the query parameter
    /// </summary>
    public string ProjectId { set; get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectConfigPage"/> class.
    /// </summary>
    public ProjectConfigPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Called when the page is appearing.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        BindingContext = App.ViewModel.GetProject(ProjectId);
    }
}