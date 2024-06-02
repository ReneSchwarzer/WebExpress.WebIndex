using WebExpress.WebIndex.Studio.Views;

namespace WebExpress.WebIndex.Studio.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        BindingContext = App.ViewModel;

        ObjectList.SelectedItem = App.ViewModel.Objects.FirstOrDefault();


    }

    private void OnAddObjectClicked(object sender, EventArgs e)
    {
        var newObject = new Model.Object() { Name = $"Object_{App.ViewModel.Project.Objects.Count + 1}" };
        App.ViewModel.Project.Objects.Add(newObject);

        Frame.Content = new ObjectPropertyView(newObject);
    }

    private void OnAddClicked(object sender, EventArgs e)
    {
        var newObject = new Model.Object() { Name = $"Object_{App.ViewModel.Project.Objects.Count + 1}" };
        App.ViewModel.Project.Objects.Add(newObject);

        ObjectList.SelectedItem = newObject;
        Frame.Content = new ObjectDetailsView(newObject, "property");
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        Frame.Content = new ObjectDetailsView(ObjectList.SelectedItem as Model.Object, "property");
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        var obj = ObjectList.SelectedItem as Model.Object;

        App.ViewModel.Project.RemoveObject(obj);

        ObjectList.SelectedItem = App.ViewModel.Objects.FirstOrDefault();
    }

    private void OnSelectObjectClicked(object sender, SelectedItemChangedEventArgs e)
    {
        Frame.Content = new ObjectDetailsView(e.SelectedItem as Model.Object);
    }
}