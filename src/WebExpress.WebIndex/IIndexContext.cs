﻿namespace WebExpress.WebIndex
{
    /// <summary>
    /// Provides the context for accessing the index data.
    /// </summary>
    public interface IIndexContext
    {
        /// <summary>
        /// Returns the data directory where the index data is located.
        /// </summary>
        public string IndexDirectory { get; }
    }
}
