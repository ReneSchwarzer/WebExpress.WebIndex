using System.Collections.ObjectModel;

namespace WebExpress.WebIndex.Studio.Model
{
    /// <summary>
    /// Represents a project within the WebExpress.WebIndex.Studio.
    /// </summary>
    public class Project : BindableObject
    {
        private string _name;
        private string _description;

        /// <summary>
        /// Returns the id of the project.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Returns or sets the name of the project.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        /// <summary>
        /// Returns or sets the description of the project.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        /// <summary>
        /// Retuns a collection of objects in the project.
        /// </summary>
        public ObservableCollection<ObjectType> Objects { get; } = [];

        /// <summary>
        /// Removes the specified object from the project.
        /// </summary>
        /// <param name="obj">The object to remove from the project.</param>
        public void RemoveObject(ObjectType obj)
        {
            if (obj == null) return;

            Objects.Remove(obj);

            App.ViewModel.IndexManager.Drop(obj.BuildClass());
        }
    }
}
