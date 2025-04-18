using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WebExpress.WebIndex.WiUI.Model
{
    /// <summary>
    /// ViewModel for loading projects.
    /// </summary>
    public class ProjectViewModel : INotifyPropertyChanged
    {
        private Project? _selectedProject = null;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Returns the command to add a new project.
        /// </summary>
        public ICommand AddProjectCommand { get; }

        /// <summary>
        /// Returns the command to delete an existing project.
        /// </summary>
        public ICommand DeleteProjectCommand { get; }

        /// <summary>
        /// Returns the command to save the current project.
        /// </summary>
        public ICommand SaveProjectCommand { get; }

        /// <summary>
        /// Returns or sets the collection of projects.
        /// </summary>
        public ObservableCollection<Project> Projects { get; set; }

        /// <summary>
        /// Returns or sets the selected project.
        /// </summary>
        public Project? SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;

                Projects.ToList().ForEach(p => p.IsSelected = false);

                if (_selectedProject != null)
                {
                    _selectedProject.IsSelected = true;
                }

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectViewModel"/> class.
        /// Loads the projects and initializes the load project command.
        /// </summary>
        public ProjectViewModel()
        {
            Projects = ProjectService.LoadProjects();
            AddProjectCommand = new Command(OnAddProject);
            DeleteProjectCommand = new Command(OnDeleteProject, () => SelectedProject != null);
            SaveProjectCommand = new Command(OnSaveProject, () => SelectedProject != null);
        }

        /// <summary>
        /// Adds a new project to the collection and saves the updated collection.
        /// </summary>
        private void OnAddProject()
        {
            var newProject = new Project { ProjectName = "New Project" };
            Projects.Add(newProject);
            SelectedProject = newProject;
            ProjectService.SaveProjects(Projects);
        }

        /// <summary>
        /// Deletes the selected project from the collection and saves the updated collection.
        /// </summary>
        private void OnDeleteProject()
        {
            if (SelectedProject != null)
            {
                Projects.Remove(SelectedProject);
                ProjectService.SaveProjects(Projects);
            }
        }

        /// <summary>
        /// Saves the current project to the project collection.
        /// </summary>
        private void OnSaveProject()
        {
            if (SelectedProject != null)
            {
                ProjectService.SaveProjects(Projects);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}