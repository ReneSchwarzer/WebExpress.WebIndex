namespace WebExpress.WebIndex.Test.Document
{
    /// <summary>
    /// Data class for unit testing.
    /// </summary>
    public class UnitTestIndexTestDocumentD : UnitTestIndexTestDocument
    {
        /// <summary>
        /// Returns or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Returns or sets the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Returns or sets the phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Returns or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns or sets the birth day.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Returns or sets the salutation.
        /// </summary>
        public string Salutation { get; set; }

        /// <summary>
        /// Returns or sets the address.
        /// </summary>
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
