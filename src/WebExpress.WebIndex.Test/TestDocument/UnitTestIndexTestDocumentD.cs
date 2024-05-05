namespace WebExpress.WebIndex.Test.Document
{
    public class UnitTestIndexTestDocumentD : UnitTestIndexTestDocument
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Salutation { get; set; }
        public string Address { get; set; }

        /// <summary>
        /// Convert the object into a string representation. 
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{Id} - {FirstName} {LastName}";
        }
    }
}
