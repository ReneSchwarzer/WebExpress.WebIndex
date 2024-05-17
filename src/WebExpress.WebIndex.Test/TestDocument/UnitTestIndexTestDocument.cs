using System.Text;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex.Test.Document
{
    public abstract class UnitTestIndexTestDocument : IIndexItem
    {
        /// <summary>
        /// Returns or sets the id.
        /// </summary>
        [IndexIgnore]
        public Guid Id { get; set; }
    }
}
