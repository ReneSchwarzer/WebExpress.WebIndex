﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using WebExpress.WebIndex.Term.Converter;
using WebExpress.WebIndex.Term.Filter;
using WebExpress.WebIndex.Wql;

namespace WebExpress.WebIndex
{
    public abstract class IndexManager : IDisposable
    {
        /// <summary>
        /// Returns an enumeration of the existing index documents.
        /// </summary>
        private Dictionary<Type, IIndexDocument> Documents { get; } = [];

        /// <summary>
        /// Returns or sets the index context.
        /// </summary>
        public IIndexContext Context { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public IndexManager()
        {

        }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public void Initialization(IIndexContext context)
        {
            var assembly = typeof(IndexManager).Assembly;
            string[] fileNames = ["IrregularWords.en", "IrregularWords.de", "MisspelledWords.en", "MisspelledWords.de", "RegularWords.en", "RegularWords.de", "StopWords.en", "StopWords.de"];

            Context = context;

            Directory.CreateDirectory(Context.IndexDirectory);

            foreach (var fileName in fileNames)
            {
                var path = Path.Combine(Context.IndexDirectory, fileName.ToLower());
                var resources = assembly.GetManifestResourceNames();
                var resource = resources
                    .Where(x => x.EndsWith($".{fileName}", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (resource == null)
                {
                    continue;
                }

                try
                {

                    if (!File.Exists(path))
                    {
                        using var sw = new StreamWriter(path, false, Encoding.UTF8);
                        using var contentStream = assembly.GetManifestResourceStream(resource);
                        using var sr = new StreamReader(contentStream, Encoding.UTF8);

                        sw.Write(sr.ReadToEnd());
                    }
                }
                catch
                {
                }
            }

            IndexTermConverterLowerCase.Initialization(context);
            IndexTermConverterMisspelled.Initialization(context);
            IndexTermConverterNormalizer.Initialization(context);
            IndexTermConverterSingular.Initialization(context);
            IndexTermConverterSynonym.Initialization(context);
            IndexTermFilterEmpty.Initialization(context);
            IndexTermFilterStopWord.Initialization(context);
        }

        /// <summary>
        /// Registers a data type in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="type">The index type.</param>
        public void Register<T>(CultureInfo culture, IndexType type = IndexType.Memory) where T : IIndexItem
        {
            if (!Documents.ContainsKey(typeof(T)))
            {
                Documents.Add(typeof(T), new IndexDocument<T>(Context, type, culture));
            }
        }

        /// <summary>
        /// Rebuilds the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="items">The data to be added to the index.</param>
        public void ReIndex<T>(IEnumerable<T> items) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.ReBuild((uint)items.Count());

                foreach (var item in items)
                {
                    document.Add(item);
                };
            }
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        public void Add<T>(T item) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.Add(item);
            }
        }

        /// <summary>
        /// Updates a item in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be updated to the index.</param>
        public void Update<T>(T item) where T : IIndexItem
        {
            Remove(item);
            Add(item);
        }

        /// <summary>
        /// Clear all data from index document.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        public void Clear<T>() where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.Clear();
            }
        }

        /// <summary>
        /// Removes a index document.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        public void Remove<T>() where T : IIndexItem
        {
            if (GetIndexDocument<T>() != null)
            {
                Documents.Remove(typeof(T), out IIndexDocument value);

                value.Dispose();
            }
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be removed from the index.</param>
        public void Remove<T>(T item) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.Remove(item);
            }
        }

        /// <summary>
        /// Returns all documents from the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <returns>An enumeration of the documents</returns>
        public IEnumerable<T> All<T>() where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                return document.All;
            }

            return [];
        }

        /// <summary>
        /// Executes a wql statement.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="wql">Tje wql statement.</param>
        /// <returns>The WQL parser.</returns>
        public IWqlStatement<T> ExecuteWql<T>(string wql) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                var parser = new WqlParser<T>(document);
                return parser.Parse(wql);
            }

            return null;
        }

        /// <summary>
        /// Returns an index type based on its type.
        /// </summary>
        /// <returns>The index type or null.</returns>
        public IIndexDocument<T> GetIndexDocument<T>() where T : IIndexItem
        {
            return Documents.ContainsKey(typeof(T)) ? Documents[typeof(T)] as IIndexDocument<T> : null;
        }

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public void Dispose()
        {
            foreach (var document in Documents)
            {
                document.Value.Dispose();
            }

            Documents.Clear();
        }
    }
}
