using System.Diagnostics;
using System.Reflection;
using WebExpress.WebIndex;

namespace WebExpress.Test.Index
{
    public class UnitTestIndexFixture
    {
        public IndexManager IndexManager { get; private set; }

        public UnitTestIndexFixture()
        {
            var ctor = typeof(IndexManager)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c => !c.GetParameters().Any());

            IndexManager = (IndexManager)ctor.Invoke(Array.Empty<object>());
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
