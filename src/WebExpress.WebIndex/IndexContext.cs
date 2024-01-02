using System;
using System.IO;

namespace WebExpress.WebIndex
{
    public class IndexContext : IIndexContext
    {
        /// <summary>
        /// Returns or sets the data directory where the index data is located.
        /// </summary>
        public string IndexDirectory { get; set; } = Path.Combine(Environment.CurrentDirectory, "index");
    }
}
