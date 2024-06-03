namespace WebExpress.WebIndex.Studio.Pages;

public partial class InfoPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InfoPage"/> class.
    /// </summary>
    public InfoPage()
    {
        InitializeComponent();

        BindingContext = App.ViewModel;
    }
}