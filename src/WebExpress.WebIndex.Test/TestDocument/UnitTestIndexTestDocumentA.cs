namespace WebExpress.WebIndex.Test.Document
{
    /// <summary>
    /// Represents a test document for a person.
    /// </summary>
    public class UnitTestIndexTestDocumentA : UnitTestIndexTestDocument
    {
        public string Text { get; set; }

        /// <summary>
        /// Convert the object into a string representation. 
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{Id} - {Text}";
        }
    }
}
