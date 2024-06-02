using WebExpress.WebIndex.Studio.Views;

namespace WebExpress.WebIndex.Studio.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        BindingContext = App.ViewModel;

        List.SelectedItem = App.ViewModel.Objects.FirstOrDefault();
    }

    private void OnAddObjectClicked(object sender, EventArgs e)
    {
        var newObject = new Model.Object() { Name = $"Object_{App.ViewModel.Project.Objects.Count + 1}" };
        App.ViewModel.Project.Objects.Add(newObject);

        Frame.Content = new ObjectPropertyView(newObject);
    }

    private void OnSelectObjectClicked(object sender, SelectedItemChangedEventArgs e)
    {
        Frame.Content = new ObjectDetailsView(e.SelectedItem as Model.Object);
    }
}