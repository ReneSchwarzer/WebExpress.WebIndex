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
    /// <typeparam name="T">The type of the index item.</typeparam>
    public interface IWqlStatement<T> : IWqlStatement where T : IIndexItem
    {
        /// <summary>
        /// Returns the filter expression.
        /// </summary>
        WqlExpressionNodeFilter<T> Filter { get; }

        /// <summary>
        /// Returns the order expression.
        /// </summary>
        WqlExpressionNodeOrder<T> Order { get; }

        /// <summary>
        /// Returns the partitioning expression.
        /// </summary>
        WqlExpressionNodePartitioning<T> Partitioning { get; }

        /// <summary>
        /// Applies the filter to the index.
        /// </summary>
        /// <returns>The data ids from the index.</returns>
        IQueryable<T> Apply();

        /// <summary>
        /// Applies the filter to the unfiltered data object.
        /// </summary>
        /// <param name="unfiltered">The unfiltered data.</param>
        /// <returns>The filtered data.</returns>
        IQueryable<T> Apply(IQueryable<T> unfiltered);
    }
}
