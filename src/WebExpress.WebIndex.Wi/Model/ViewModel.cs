﻿using System.Globalization;
using System.Reflection;
using System.Text.Json;
using WebExpress.WebIndex.Storage;
using WebExpress.WebIndex.Wi.Converter;

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
        /// Return or sets the current object type.
        /// </summary>
        public ObjectType CurrentObjectType { get; set; }

        /// <summary>
        /// Return or sets the current field.
        /// </summary>
        public Field CurrentIndexField { get; set; }

        /// <summary>
        /// Opens the specified index file.
        /// </summary>
        /// <param name="indexFile">The full path to the index file.</param>
        /// <returns>True if successful, otherwise fasle.</returns>
        public bool OpenIndexFile(string indexFile)
        {
            CurrentDirectory = Path.GetDirectoryName(indexFile);
            CurrentIndexFile = indexFile;

            var schema = File.ReadAllText(CurrentIndexFile);
            var options = new JsonSerializerOptions { Converters = { new FieldTypeConverter() } };
            CurrentObjectType = JsonSerializer.Deserialize<ObjectType>(schema, options);

            var runtimeClass = CurrentObjectType.BuildRuntimeClass();

            var context = new IndexContext { IndexDirectory = CurrentDirectory };
            IndexManager = new IndexManager();
            IndexManager.Initialization(context);

            IndexManager.Create(runtimeClass, CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            return true;
        }

        /// <summary>
        /// Opens the specified index field.
        /// </summary>
        /// <param name="indexField">The the index field.</param>
        /// <returns>True if successful, otherwise fasle.</returns>
        public bool OpenIndexField(Field indexField)
        {
            CurrentIndexField = indexField;

            return true;
        }

        /// <summary>
        /// Close the current index file.
        /// </summary>
        /// <returns>True if successful, otherwise fasle.</returns>
        public bool CloseIndexFile()
        {
            var runtimeClass = CurrentObjectType.BuildRuntimeClass();
            IndexManager.Close(runtimeClass);
            CurrentIndexFile = null;

            CurrentObjectType = null;

            return true;
        }

        /// <summary>
        /// Drop the current index file.
        /// </summary>
        /// <returns>True if successful, otherwise fasle.</returns>
        public bool DropIndexFile()
        {
            var runtimeClass = CurrentObjectType.BuildRuntimeClass();
            IndexManager.Drop(runtimeClass);
            CurrentIndexFile = null;

            CurrentObjectType = null;

            return true;
        }

        /// <summary>
        /// Returns the index terms.
        /// </summary>
        /// <returns>The index terms</returns>
        public IEnumerable<(string, uint, IEnumerable<Guid>)> GetIndexTerms()
        {
            var runtimeClass = CurrentObjectType.BuildRuntimeClass();
            var document = WiApp.ViewModel.IndexManager.GetIndexDocument(runtimeClass);
            var fieldProperty = runtimeClass.GetProperty(CurrentIndexField?.Name);
            var methodInfo = document.GetType().GetMethod("GetReverseIndex");
            var reverseIndex = methodInfo.Invoke(document, [fieldProperty]);
            var termProperty = reverseIndex.GetType().GetProperty("Term");
            var term = termProperty.GetValue(reverseIndex) as IndexStorageSegmentTerm;

            return term.Terms.Select(x => (x.Item1, x.Item2.Fequency, x.Item2.Postings.Select(y => y.DocumentID)));
        }
    }
}
