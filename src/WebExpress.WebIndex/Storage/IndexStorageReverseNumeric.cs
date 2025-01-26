using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WebExpress.WebIndex.Term;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Implementation of the web reverse index, which stores numeric values on disk.
    /// </summary>
    /// <typeparam name="TIndexItem">The data type. This must have the IIndexItem interface.</typeparam>
    public class IndexStorageReverseNumeric<TIndexItem> : IndexStorageReverse<TIndexItem>, IIndexStorage
        where TIndexItem : IIndexItem
    {
        private readonly string _extentions = "wrn";
        private readonly int _version = 1;

        /// <summary>
        /// Returns or sets the numeric tree.
        /// </summary>
        public IndexStorageSegmentNumeric Numeric { get; private set; }

        /// <summary>
        /// Returns all items.
        /// </summary>
        public override IEnumerable<Guid> All => Numeric.All.Distinct();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="field">The field that makes up the index.</param>
        /// <param name="culture">The culture.</param>
        public IndexStorageReverseNumeric(IIndexDocumemntContext context, IndexFieldData field, CultureInfo culture)
            : base(context, field, culture)
        {
            FileName = Path.Combine(Context.IndexDirectory, $"{typeof(TIndexItem).Name}.{Field.Name}.{_extentions}");

            var exists = File.Exists(FileName);

            IndexFile = new IndexStorageFile(FileName);
            Header = new IndexStorageSegmentHeader(new IndexStorageContext(this)) { Identifier = _extentions, Version = (byte)_version };
            Allocator = new IndexStorageSegmentAllocatorReverseIndex(new IndexStorageContext(this));
            Statistic = new IndexStorageSegmentStatistic(new IndexStorageContext(this));
            Numeric = new IndexStorageSegmentNumeric(new IndexStorageContext(this));

            Header.Initialization(exists);
            Statistic.Initialization(exists);
            Numeric.Initialization(exists);
            Allocator.Initialization(exists);

            IndexFile.Flush();
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        public override void Add(TIndexItem item)
        {
            var value = GetPropertyValue(item, Field)?.ToString();
            var terms = Context.TokenAnalyzer.Analyze(value, Culture);

            Add(item, terms);
        }

        /// <summary>
        /// Adds token to to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public override void Add(TIndexItem item, IEnumerable<IndexTermToken> terms)
        {
            foreach (var term in terms)
            {
                var value = Convert.ToDecimal(term.Value);

                Numeric.Add(value)?
                    .AddPosting(item.Id);

                Statistic.Count++;
                IndexFile.Write(Statistic);
            }
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
        /// <param name="item">The data to be removed from the field.</param>
        public override void Delete(TIndexItem item)
        {
            var value = GetPropertyValue(item, Field);
            var terms = Context.TokenAnalyzer.Analyze(value?.ToString(), Culture);

            Delete(item, terms);
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the field.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public override void Delete(TIndexItem item, IEnumerable<IndexTermToken> terms)
        {
            foreach (var term in terms)
            {
                var node = Numeric[Convert.ToDecimal(term.Value)];

                if (node != null)
                {
                    if (node.RemovePosting(item.Id))
                    {
                        Statistic.Count--;
                        IndexFile.Write(Statistic);
                    }
                }
            }
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public override void Clear()
        {
            IndexFile.NextFreeAddr = 0;
            IndexFile.InvalidationAll();
            IndexFile.Flush();

            Header = new IndexStorageSegmentHeader(new IndexStorageContext(this)) { Identifier = _extentions, Version = (byte)_version };
            Allocator = new IndexStorageSegmentAllocatorReverseIndex(new IndexStorageContext(this));
            Statistic = new IndexStorageSegmentStatistic(new IndexStorageContext(this));
            Numeric = new IndexStorageSegmentNumeric(new IndexStorageContext(this));

            Header.Initialization(false);
            Statistic.Initialization(false);
            Numeric.Initialization(false);
            Allocator.Initialization(false);

            IndexFile.Flush();
        }

        /// <summary>
        /// Drop the reverse index.
        /// </summary>
        public override void Drop()
        {
            IndexFile.Delete();
        }

        /// <summary>
        /// Return all items for a given input.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="options">The retrieve options.</param>
        /// <returns>An enumeration of the data ids.</returns>
        public override IEnumerable<Guid> Retrieve(object input, IndexRetrieveOptions options)
        {
            if (decimal.TryParse(input?.ToString(), out decimal value))
            {
                return options.Method switch
                {
                    IndexRetrieveMethod.Phrase => Numeric.Retrieve(value, options),
                    IndexRetrieveMethod.GratherThan => Numeric.Retrieve(value, options),
                    IndexRetrieveMethod.GratherThanOrEqual => Numeric.Retrieve(value, options),
                    IndexRetrieveMethod.LessThan => Numeric.Retrieve(value, options),
                    IndexRetrieveMethod.LessThanOrEqual => Numeric.Retrieve(value, options),
                    _ => []
                };
            }

            return [];
        }
    }
}