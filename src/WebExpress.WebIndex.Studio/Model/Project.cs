using System.Collections.ObjectModel;

namespace WebExpress.WebIndex.Studio.Model
{
    public class Project
    {
        /// <summary>
        /// Returns or sets the name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Retuns a list of objects in the project.
        /// </summary>
        public ObservableCollection<Object> Objects { get; } = [];
    }
}
