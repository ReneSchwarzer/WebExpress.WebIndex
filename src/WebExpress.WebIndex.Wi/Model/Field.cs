using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebExpress.WebIndex.Wi.Model
{
    /// <summary>
    /// Represents an index field.
    /// </summary>
    internal class Field
    {
        /// <summary>
        /// Returns or sets the name of the field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns or sets the type of the field.
        /// </summary>
        public FieldType Type { get; set; }

        /// <summary>
        /// Returns or sets the ignore attribute.
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Returns or sets if the field is abstract.
        /// </summary>
        public bool Abstract { get; set; }
    }
}
