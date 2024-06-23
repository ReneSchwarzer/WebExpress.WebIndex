using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex.Test.Document
{
    /// <summary>
    /// Data class for unit testing.
    /// </summary>
    public class UnitTestIndexTestDocumentC : UnitTestIndexTestDocument
    {
        /// <summary>
        /// Returns or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Returns or sets the numer of the item.
        /// </summary>
        [IndexIgnore]
        public int Number { get; set; }

        /// <summary>
        /// Convert the object into a string representation. 
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"#{Number} - {Id}: {Text}";
        }
    }
}
