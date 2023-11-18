using System.Diagnostics;
using WebExpress.WebIndex;
using WebExpress.WebIndex.Test;

namespace WebExpress.Test.Index
{
    public class UnitTestIndexFixture
    {
        public IndexManager IndexManager { get; } = new IndexManagerTest();

        public UnitTestIndexFixture()
        {

        }

        public virtual void Dispose()
        {

        }

        public long GetUsedMemory()
        {
            long lngSessMemory = Process.GetCurrentProcess().WorkingSet64;

            return lngSessMemory;
        }
    }
}
