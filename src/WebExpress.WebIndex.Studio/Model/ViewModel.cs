using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using WebExpress.WebIndex.Studio.Pages;

namespace WebExpress.WebIndex.Studio.Model
{
    /// <summary>
    /// ViewModel class for managing the UI logic and data binding for the application.
    /// </summary>
    public class ViewModel
    {
        /// <summary>
        /// Command that triggers the opening of the GitHub project page.
        /// </summary>
        public ICommand OpenGitHubProjectCommand { get; }

        /// <summary>
        /// Command that triggers the opening the project config page.
        /// </summary>
        public ICommand OpenProjectConfigPageCommand { get; }

        /// <summary>
        /// Command that triggers the opening the object type page.
        /// </summary>
        public ICommand OpenObjectTypePageCommand { get; }


        /// <summary>
        /// Manages the indexing of project objects.
        /// </summary>
        public IndexManager IndexManager { get; } = new IndexManager();

        /// <summary>
        /// Represents the current project with its associated objects and attributes.
        /// </summary>
        public ObservableCollection<Project> Projects { get; } = [];

        /// <summary>
        /// Constructor for the ViewModel. Initializes the project and index manager.
        /// </summary>
        public ViewModel()
        {
            var project = new Project() { Name = "Project 1", Description = "Project 1 is the best." };

            Projects.Add(project);
            Projects.Add(new Project() { Name = "Project 2" });

            OpenGitHubProjectCommand = new Command(OpenGitHubProject);
            OpenProjectConfigPageCommand = new Command<Project>(OpenProjectConfigPage);
            OpenObjectTypePageCommand = new Command<Project>(OpenObjectTypePage);

            var obj1 = new ObjectType() { Name = "Object_1" };

            obj1.Attributes.Add(new Attribute() { Name = "Name", Type = AttributeType.Text });
            obj1.Attributes.Add(new Attribute() { Name = "Attribute_2", Type = AttributeType.Double });
            obj1.Attributes.Add(new Attribute() { Name = "Attribute_3", Type = AttributeType.Bool });
            obj1.Attributes.Add(new Attribute() { Name = "Attribute_4", Type = AttributeType.Int });

            project.Objects.Add(obj1);
            project.Objects.Add(new ObjectType() { Name = "Object_2" });
            project.Objects.Add(new ObjectType() { Name = "Object_3" });

            var context = new IndexContext { IndexDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WebIndex Studio") };
            IndexManager.Initialization(context);

            var runtimeClass = obj1.BuildClass();
            IndexManager.Create(runtimeClass, CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            //IndexManager.Clear(runtimeClass);

            if (!IndexManager.All(runtimeClass).Any())
            {
                var id = runtimeClass.GetProperty("Id");
                var name = runtimeClass.GetProperty("Name");
                var attribute2 = runtimeClass.GetProperty("Attribute_2");
                var attribute3 = runtimeClass.GetProperty("Attribute_3");
                var attribute4 = runtimeClass.GetProperty("Attribute_4");

                var instance = Activator.CreateInstance(runtimeClass);
                id.SetValue(instance, Guid.NewGuid(), null);
                name.SetValue(instance, "Hello, World!", null);
                attribute2.SetValue(instance, 1293.76, null);
                attribute3.SetValue(instance, true, null);
                attribute4.SetValue(instance, 10, null);

                IndexManager.Insert(runtimeClass, instance);

                instance = Activator.CreateInstance(runtimeClass);
                id.SetValue(instance, Guid.NewGuid(), null);
                name.SetValue(instance, "Hello, Welt!", null);
                attribute2.SetValue(instance, 545.876, null);
                attribute3.SetValue(instance, false, null);
                attribute4.SetValue(instance, 5, null);

                IndexManager.Insert(runtimeClass, instance);
            }
        }

        /// <summary>
        /// Create a project.
        /// </summary>
        /// <returns>The created project.</returns>
        public Project CreatetProject()
        {
            var project = new Project() { Name = $"Project {Projects.Count + 1}" };
            Projects.Add(project);

            return project;
        }

        /// <summary>
        /// Delete a project.
        /// </summary>
        /// <param name="project">The project to be deleted.</param>
        public void DeleteProject(Project project)
        {
            Projects.Remove(project);
        }

        /// <summary>
        /// Returns the project based on the id.
        /// </summary>
        /// <param name="id">The project id.</param>
        /// <returns>The project, if it is found, false otherwise.</returns>
        public Project GetProject(string id)
        {
            return Projects.Where(x => x.Id.ToString().Equals(id)).FirstOrDefault();
        }

        /// <summary>
        /// Opens the GitHub project page in the default web browser.
        /// </summary>
        private void OpenGitHubProject()
        {
            var url = "https://github.com/ReneSchwarzer/WebExpress.WebIndex";
            try
            {
                Launcher.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Opens the project config page.
        /// </summary>
        /// <param name="project">The project.</param>
        private async void OpenProjectConfigPage(Project project)
        {
            await Shell.Current.GoToAsync($"{nameof(ProjectConfigPage)}?id={project?.Id}");
        }

        /// <summary>
        /// Opens the object type page.
        /// </summary>
        /// <param name="project">The project.</param>
        private async void OpenObjectTypePage(Project project)
        {
            await Shell.Current.GoToAsync($"{nameof(ObjectTypePage)}?id={project.Id}");
        }

    }
}
