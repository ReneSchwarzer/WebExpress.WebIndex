using System.Text;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex.Test.Document
{
    public abstract class UnitTestIndexTestDocument : IIndexItem
    {
        [IndexIgnore]
        public Guid Id { get; set; }
    }
}
