using System.Diagnostics;
using System.Text;
using WebExpress.WebIndex;
using WebExpress.WebIndex.Test;

namespace WebExpress.WebIndex.Test.Index
{
    public class UnitTestIndexFixture
    {
        public IndexManager IndexManager { get; } = new IndexManagerTest();

        public UnitTestIndexFixture()
        {
            lock (IndexManager)
            {
                IndexManager.Initialization(new IndexContext());
            }
        }

        public virtual void Dispose()
        {

        }

        public long GetUsedMemory()
        {
            long lngSessMemory = Process.GetCurrentProcess().WorkingSet64;

            return lngSessMemory;
        }

        public string GetRessource(string name)
        {
            var assembly = typeof(UnitTestIndexFixture).Assembly;
            var resources = assembly.GetManifestResourceNames();

            var resource = resources
                .Where(x => x.Contains(name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (resource == null)
            {
                return "";
            }

            using var stream = assembly.GetManifestResourceStream(resource);
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }
}
