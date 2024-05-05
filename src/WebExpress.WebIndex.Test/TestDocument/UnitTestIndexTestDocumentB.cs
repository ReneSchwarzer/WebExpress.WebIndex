namespace WebExpress.WebIndex.Test.Document
{
    public class UnitTestIndexTestDocumentB : UnitTestIndexTestDocument
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
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
