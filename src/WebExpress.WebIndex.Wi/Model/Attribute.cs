using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebExpress.WebIndex.Wi.Model
{
    /// <summary>
    /// Represents an index attribute.
    /// </summary>
    internal class Attribute
    {
        /// <summary>
        /// Returns or sets the name of the attribute.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns or sets the description of the attribute.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns or sets the type of the attribute.
        /// </summary>
        public AttributeType Type { get; set; }
    }
}
