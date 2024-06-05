namespace WebExpress.WebIndex.Studio.Popups;
using CommunityToolkit.Maui.Views;

public partial class ConfirmDeleteProjectPopup : Popup
{
    public ConfirmDeleteProjectPopup()
    {
        InitializeComponent();
    }

    private async void OnYesButtonClicked(object sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(true, cts.Token);
    }

}