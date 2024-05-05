namespace WebExpress.WebIndex.Test.Document
{
    public class UnitTestIndexTestDocumentC : UnitTestIndexTestDocument
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
            return $"{Id}: {Text}";
        }
    }
}
