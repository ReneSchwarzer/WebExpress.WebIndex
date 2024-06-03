namespace WebExpress.WebIndex.Studio.Pages;

/// <summary>
/// The main page of the application that hosts the object list and content area.
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainPage"/> class.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();

        BindingContext = App.ViewModel;
    }
}