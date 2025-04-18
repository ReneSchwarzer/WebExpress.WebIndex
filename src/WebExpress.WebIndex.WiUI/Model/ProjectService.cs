using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace WebExpress.WebIndex.WiUI.Model
{
    /// <summary>
    /// Provides services for saving and loading projects.
    /// </summary>
    public class ProjectService
    {
        private static readonly string _appFolderName = "WebExpress.WebIndex.WiUI";
        private static readonly string _projectsFileName = "projects.xml";

        /// <summary>
        /// Returns the application data path for storing project files.
        /// </summary>
        /// <returns>The path to the application data directory.</returns>
        private static string GetAppDataPath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, _appFolderName);
            Directory.CreateDirectory(appFolder);

            return appFolder;
        }

        /// <summary>
        /// Saves the collection of projects to an XML file in the application data directory.
        /// </summary>
        /// <param name="projects">The collection of projects to save.</param>
        public static void SaveProjects(ObservableCollection<Project> projects)
        {
            var filePath = Path.Combine(GetAppDataPath(), _projectsFileName);

            var serializer = new XmlSerializer(typeof(ObservableCollection<Project>));
            using var writer = new StreamWriter(filePath);

            serializer.Serialize(writer, projects);
        }

        /// <summary>
        /// Loads the collection of projects from an XML file in the application data directory.
        /// </summary>
        /// <returns>The collection of projects loaded from the XML file.</returns>
        public static ObservableCollection<Project> LoadProjects()
        {
            var filePath = Path.Combine(GetAppDataPath(), _projectsFileName);
            if (!File.Exists(filePath))
            {
                return [];
            }

            var serializer = new XmlSerializer(typeof(ObservableCollection<Project>));
            using StreamReader reader = new StreamReader(filePath);
            var result = serializer.Deserialize(reader) as ObservableCollection<Project>;

            return result ?? [];
        }
    }
}
