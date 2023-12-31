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
    public abstract class WebIndexManager
    {
        /// <summary>
        /// Returns an enumeration of the existing index documents.
        /// </summary>
        private Dictionary<Type, IWebIndexDocument> Documents { get; } = [];

        /// <summary>
        /// Returns or sets the index context.
        /// </summary>
        public IWebIndexContext Context { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public WebIndexManager()
        {

        }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public void Initialization(IWebIndexContext context)
        {
            var assembly = typeof(WebIndexManager).Assembly;
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

            WebIndexTermConverterLowerCase.Initialization(context);
            WebIndexTermConverterMisspelled.Initialization(context);
            WebIndexTermConverterNormalizer.Initialization(context);
            WebIndexTermConverterSingular.Initialization(context);
            WebIndexTermConverterSynonym.Initialization(context);
            WebIndexTermFilterEmpty.Initialization(context);
            WebIndexTermFilterStopWord.Initialization(context);
        }

        /// <summary>
        /// Registers a data type in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="capacity">The predicted capacity (number of items to store) of the index.</param>
        /// <param name="type">The index type.</param>
        public void Register<T>(CultureInfo culture, uint capacity = ushort.MaxValue, WebIndexType type = WebIndexType.Memory) where T : IWebIndexItem
        {
            if (!Documents.ContainsKey(typeof(T)))
            {
                Documents.Add(typeof(T), new WebIndexDocument<T>(Context, type, culture, capacity));
            }
        }

        /// <summary>
        /// Rebuilds the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="items">The data to be added to the index.</param>
        public void ReIndex<T>(IEnumerable<T> items) where T : IWebIndexItem
        {
            Clear<T>();

            foreach (var item in items)
            {
                Add(item);
            };
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        public void Add<T>(T item) where T : IWebIndexItem
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
        public void Update<T>(T item) where T : IWebIndexItem
        {
            Remove(item);
            Add(item);
        }

        /// <summary>
        /// Clear all data from index document.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        public void Clear<T>() where T : IWebIndexItem
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
        public void Remove<T>() where T : IWebIndexItem
        {
            if (GetIndexDocument<T>() != null)
            {
                Documents.Remove(typeof(T), out IWebIndexDocument value);

                value.Dispose();
            }
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be removed from the index.</param>
        public void Remove<T>(T item) where T : IWebIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.Remove(item);
            }
        }

        /// <summary>
        /// Executes a wql statement.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="wql">Tje wql statement.</param>
        /// <returns>The WQL parser.</returns>
        public IWqlStatement<T> ExecuteWql<T>(string wql) where T : IWebIndexItem
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
        public IIndexDocument<T> GetIndexDocument<T>() where T : IWebIndexItem
        {
            return Documents.ContainsKey(typeof(T)) ? Documents[typeof(T)] as IIndexDocument<T> : null;
        }
    }
}