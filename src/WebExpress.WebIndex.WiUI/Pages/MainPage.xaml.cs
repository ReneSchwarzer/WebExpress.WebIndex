using WebExpress.WebIndex.WiUI.Model;

namespace WebExpress.WebIndex.WiUI.Pages
{
    /// <summary>
    /// Represents the main page of the application.
    /// Inherits from <see cref="ContentPage"/>.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// Sets the BindingContext to a new instance of <see cref="MainViewModel"/>.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

        /// <summary>
        /// Handles the event when an index is selected.
        /// Displays an alert with the selected index.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the selected item.</param>
        private void OnIndexSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            var selectedIndex = e.SelectedItem as Model.Index;
            var context = BindingContext as MainViewModel;

            var fileNameWithPath = selectedIndex?.FileNameWithPath;

            if (File.Exists(fileNameWithPath))
            {
                context?.OpenIndexFile(fileNameWithPath);
            }
        }

        /// <summary>
        /// Handles the event when a field is selected.
        /// Displays an alert with the selected field.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the selected item.</param>
        private void OnFieldSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            var selectedIndex = e.SelectedItem as Model.Field;
            var context = BindingContext as MainViewModel;

            context?.OpenIndexField(selectedIndex);
        }


        /// <summary>
        /// Handles the event when a term is selected.
        /// Displays an alert with the selected term.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the selected item.</param>
        private void OnTermSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            var selectedTerm = e.SelectedItem as Model.Term;
            var context = BindingContext as MainViewModel;
        }

        /// <summary>
        /// Handles the event when a term is added to the clipboard.
        /// Copies the selected term's value to the clipboard and displays an alert.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnAddClipboard(object sender, EventArgs e)
        {
            var button = sender as ImageButton;

            if (button?.BindingContext is Model.Term selectedTerm)
            {
                await Clipboard.Default.SetTextAsync(selectedTerm.Value);
                await DisplayAlert("Copied", $"'{selectedTerm.Value}' copied to clipboard", "OK");
            }
        }
    }
}
