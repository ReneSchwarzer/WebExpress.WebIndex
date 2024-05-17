﻿namespace WebExpress.WebIndex.Test.Document
{

    /// <summary>
    /// Data class for unit testing.
    /// </summary>
    public class UnitTestIndexTestDocumentB : UnitTestIndexTestDocument
    {
        /// <summary>
        /// Returns or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns or sets the summary.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Returns or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Returns or sets the price.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Returns or sets the new attribute.
        /// </summary>
        public bool New { get; set; }

        /// <summary>
        /// Convert the object into a string representation. 
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("({0}:{1},{2},{3},{4})", Id, Name, Date.ToShortDateString(), Price, New).Trim();
        }
    }
}
