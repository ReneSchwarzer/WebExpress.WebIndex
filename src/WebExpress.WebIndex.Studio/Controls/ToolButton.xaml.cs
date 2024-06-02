using System.ComponentModel;

namespace WebExpress.WebIndex.Studio.Controls;

public partial class ToolButton : ContentView
{
    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(ToolButton), default(string));
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ToolButton), default(string));

    public event EventHandler Clicked;

    public string Icon
    {
        get { return (string)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

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

    private void OnTapped(object sender, EventArgs e)
    {
        if (IsEnabled)
        {
            Clicked?.Invoke(sender, e);
        }

    }

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (IsEnabled)
        {
            if (App.Current.Resources.TryGetValue("Secondary", out var colorValue))
            {
                Button.BackgroundColor = (Color)colorValue;
            }
        }
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        if (IsEnabled)
        {
            if (App.Current.Resources.TryGetValue("Gray50", out var colorValue))
            {
                Button.BackgroundColor = (Color)colorValue;
            }
        }
    }
}