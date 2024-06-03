using System.Collections.ObjectModel;

namespace WebExpress.WebIndex.Studio.Controls;

/// <summary>
/// Represents a container for tab items within a user interface.
/// </summary>
public partial class TabControl : ContentView
{
    /// <summary>
    /// Returns or sets the collection of tab items.
    /// </summary>
    /// <value>A collection that holds the tab items.</value>
    public ObservableCollection<TabControlItem> TabItems { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TabControl"/> class.
    /// </summary>
    public TabControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the click event on a tab item.
    /// </summary>
    /// <param name="sender">The source of the event, typically a button.</param>
    /// <param name="e">The event arguments.</param>
    void OnTabClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var tabItem = (TabControlItem)button.BindingContext;
        var index = TabItems.IndexOf(tabItem);
        var underlinePosition = TabsGrid.ColumnDefinitions[index].Width.Value * index;
        ActiveTabUnderline.TranslateTo(underlinePosition, 0, 250, Easing.Linear);

        ContentArea.Content = tabItem.Content;
    }
}