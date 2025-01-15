using System;
using System.Globalization;
using System.Linq;

namespace WebExpress.WebIndex.Wql
{
    /// <summary>
    /// Represents a WQL (WebExpress Query Language) statement.
    /// </summary>
    public interface IWqlStatement
    {
        /// <summary>
        /// Returns the original wql statement.
        /// </summary>
        string Raw { get; }

        /// <summary>
        /// Returns true if there are any errors that occurred during parsing, false otherwise.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Returns the part in error of the original wql statement.
        /// </summary>
        WqlExpressionError Error { get; }

        /// <summary>
        /// Returns the culture in which to run the wql.
        /// </summary>
        CultureInfo Culture { get; }

        /// <summary>
        /// Applies the filter to the index.
        /// </summary>
        /// <param name="dataType">The data type. This must have the IIndexItem interface.</param>
        /// <returns>The data ids from the index.</returns>
        IQueryable Apply(Type dataType);

        /// <summary>
        /// Returns the sql query string.
        /// </summary>
        /// <returns>The sql part of the node.</returns>
        string GetSqlQueryString();
    }

    /// <summary>
    /// Represents a WQL (WebExpress Query Language) statement with a specific index item type.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public interface IWqlStatement<TIndexItem> : IWqlStatement
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Returns the filter expression.
        /// </summary>
        WqlExpressionNodeFilter<TIndexItem> Filter { get; }

        /// <summary>
        /// Returns the order expression.
        /// </summary>
        WqlExpressionNodeOrder<TIndexItem> Order { get; }

        /// <summary>
        /// Returns the partitioning expression.
        /// </summary>
        WqlExpressionNodePartitioning<TIndexItem> Partitioning { get; }

        /// <summary>
        /// Applies the filter to the index.
        /// </summary>
        /// <returns>The data ids from the index.</returns>
        IQueryable<TIndexItem> Apply();

        /// <summary>
        /// Applies the filter to the unfiltered data object.
        /// </summary>
        /// <param name="unfiltered">The unfiltered data.</param>
        /// <returns>The filtered data.</returns>
        IQueryable<TIndexItem> Apply(IQueryable<TIndexItem> unfiltered);
    }
}
