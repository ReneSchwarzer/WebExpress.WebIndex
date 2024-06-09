using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WebExpress.WebIndex.Wi.Model
{
    internal class ViewModel
    {
        /// <summary>
        /// Returns or sets the name of the application.
        /// </summary>
        public string Name { get; private set; } = "wi";

        /// <summary>
        /// Returns the program version.
        /// </summary>
        public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Manages the indexing of project objects.
        /// </summary>
        public IndexManager IndexManager { get; private set; }

        /// <summary>
        /// Returns or set the current indexfile or the directory.
        /// </summary>
        public string CurrentDirectory { get; set; } = Environment.CurrentDirectory;

        /// <summary>
        /// Returns or set the current indexfile or the directory.
        /// </summary>
        public string CurrentIndexFile { get; set; }

        /// <summary>
        /// Return or sets the object type.
        /// </summary>
        public ObjectType ObjectType { get; set; }

        /// <summary>
        /// Opens the specified index file.
        /// </summary>
        /// <param name="indexFile">The full path to the index file.</param>
        /// <returns>True if successful, otherwise fasle.</returns>
        public bool OpenIndexFile(string indexFile)
        {
            CurrentDirectory = Path.GetDirectoryName(indexFile);
            CurrentIndexFile = indexFile;

            var indexName = Path.GetFileNameWithoutExtension(indexFile);
            var attributes = Directory.GetFiles(CurrentDirectory, $"{indexName}.*.wri")
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Select(x => Regex.Match(x, $"{indexName}\\.(.*)").Groups[1]?.Value)
                .Select(x => new Attribute() { Name = x, Type = AttributeType.Text });


            ObjectType = new ObjectType(indexName, attributes);
            var runtimeClass = ObjectType.BuildRuntimeClass();

            var context = new IndexContext { IndexDirectory = CurrentDirectory };
            IndexManager = new IndexManager();
            IndexManager.Initialization(context);

            IndexManager.Create(runtimeClass, CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            return true;
        }

        /// <summary>
        /// Close the current index file.
        /// </summary>
        /// <returns>True if successful, otherwise fasle.</returns>
        public bool CloseIndexFile()
        {
            CurrentIndexFile = null;

            ObjectType = null;

            return true;
        }

    }
}
