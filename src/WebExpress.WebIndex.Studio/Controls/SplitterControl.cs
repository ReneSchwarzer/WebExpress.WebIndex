namespace WebExpress.WebIndex.Studio.Controls
{
    public class SplitterControl : ContentView
    {
        BoxView splitter;

        public SplitterControl()
        {
            //InitializeComponent();

            // Erstellen Sie die BoxView und fügen Sie sie zur Ansicht hinzu
            splitter = new BoxView
            {
                BackgroundColor = Color.FromRgb(125, 125, 125),
                WidthRequest = 2
            };
            Content = splitter;

            SetupGestureRecognizers();
        }

        private void SetupGestureRecognizers()
        {
            var dragGestureRecognizer = new DragGestureRecognizer();
            dragGestureRecognizer.DragStarting += OnDragStarting;
            splitter.GestureRecognizers.Add(dragGestureRecognizer);

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Parent" && this.Parent is Grid parentGrid)
                {
                    var dropGestureRecognizer = new DropGestureRecognizer();
                    dropGestureRecognizer.DragOver += OnDragOver;
                    parentGrid.GestureRecognizers.Add(dropGestureRecognizer);
                }
            };
        }

        private void OnDragStarting(object sender, DragStartingEventArgs e)
        {
            splitter.IsVisible = false;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            UpdateSplitterPosition(sender, e);
        }

        public void UpdateSplitterPosition(object sender, DragEventArgs e)
        {
            int columnIndex = Grid.GetColumn(this);
            columnIndex--;
            var newWidth = (int)e.GetPosition(null)!.Value.X;

#if WINDOWS
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                e.PlatformArgs.DragEventArgs.DragUIOverride!.IsCaptionVisible = false;
                e.PlatformArgs.DragEventArgs.DragUIOverride!.IsGlyphVisible = false;
            }
#endif
        }
    }
}
