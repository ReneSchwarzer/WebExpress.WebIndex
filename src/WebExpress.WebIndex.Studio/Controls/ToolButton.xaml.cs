using System.ComponentModel;

namespace WebExpress.WebIndex.Studio.Controls;

/// <summary>
/// Represents a button control with customizable icon and text properties.
/// </summary>
public partial class ToolButton : ContentView
{
    /// <summary>
    /// Identifies the <see cref="Icon"/> bindable property.
    /// </summary>
    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(ToolButton), default(string));
    
    /// <summary>
    /// Identifies the <see cref="Text"/> bindable property.
    /// </summary>
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ToolButton), default(string));

    /// <summary>
    /// Occurs when the ToolButton is clicked.
    /// </summary>
    public event EventHandler Clicked;

    /// <summary>
    /// Returns or sets the icon glyph.
    /// </summary>
    /// <value>The icon displayed on the button.</value>
    public string Icon
    {
        get { return (string)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    /// <summary>
    /// Returns or sets the text label.
    /// </summary>
    /// <value>The text displayed on the button.</value>
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolButton"/> class.
    /// </summary>
    public ToolButton()
    {
        InitializeComponent();

        BindingContext = this;
        if (App.Current.Resources.TryGetValue("Gray50", out var colorValue))
        {
            Button.BackgroundColor = (Color)colorValue;
        }

        PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Called when a property value changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data that provides information about the changed property.</param>
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsEnabled")
        {
            if (App.Current.Resources.TryGetValue("Gray50", out var colorValue))
            {
                Button.BackgroundColor = (Color)colorValue;
            }
        }
    }

    /// <summary>
    /// Called when the ToolButton is tapped.
    /// </summary>
    /// <param name="sender">The source of the tap event.</param>
    /// <param name="e">Event data.</param>
    private void OnTapped(object sender, EventArgs e)
    {
        // invoke the Clicked event if the button is enabled
        if (IsEnabled)
        {
            Clicked?.Invoke(sender, e);
        }

    }

    /// <summary>
    /// Called when a pointer enters the ToolButton area.
    /// </summary>
    /// <param name="sender">The source of the pointer event.</param>
    /// <param name="e">Event data.</param>
    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        // change the button background color when a pointer is over it
        if (IsEnabled)
        {
            if (App.Current.Resources.TryGetValue("Secondary", out var colorValue))
            {
                Button.BackgroundColor = (Color)colorValue;
            }
        }
    }

    // <summary>
    /// Called when a pointer exits the ToolButton area.
    /// </summary>
    /// <param name="sender">The source of the pointer event.</param>
    /// <param name="e">Event data.</param>
    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        // revert the button background color when a pointer exits
        if (IsEnabled)
        {
            if (App.Current.Resources.TryGetValue("Gray50", out var colorValue))
            {
                Button.BackgroundColor = (Color)colorValue;
            }
        }
    }
}