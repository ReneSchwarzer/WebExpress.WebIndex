using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WebExpress.WebIndex.WiUI.Model
{
    /// <summary>
    /// Represents an project.
    /// </summary>
    public class Project : INotifyPropertyChanged
    {
        private string? _projectName;
        private bool? _isSelected;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Returns or sets the name of the project.
        /// </summary>
        public string? ProjectName
        {
            get => _projectName;
            set { _projectName = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Returns or sets the selected project.
        /// </summary>
        public bool? IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); OnPropertyChanged(nameof(TextColor)); }
        }

        /// <summary>
        /// Returns the color based on the selection status of the project.
        /// Blue if the project is selected, otherwise returns black.
        /// </summary>
        public Color? TextColor
        {
            get => _isSelected.HasValue && _isSelected.Value ? Color.FromRgb(20, 20, 255) : Color.FromRgb(0, 0, 0);
        }

        /// <summary>
        /// Returns or sets the index path.
        /// </summary>
        public string? IndexPath { get; set; }

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. This is optional and will be automatically provided by the compiler if not specified.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
