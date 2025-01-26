using System;
using System.Reflection;

namespace WebExpress.WebIndex
{
    /// <summary>
    /// Represents the data for an index field.
    /// </summary>
    public class IndexFieldData
    {
        /// <summary>
        /// Returns the name of the index field.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Returns the type of the index field.
        /// </summary>
        public Type Type { get; internal set; }

        /// <summary>
        /// returns the PropertyInfo of the index field.
        /// </summary>
        public PropertyInfo PropertyInfo { get; internal set; }
    }
}
