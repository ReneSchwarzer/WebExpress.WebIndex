using System.Diagnostics;
using System.Text;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureToken : IDisposable
    {
        /// <summary>
        /// Returns the index manager.
        /// </summary>
        public IndexManager IndexManager { get; } = new IndexManagerTest();

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureToken()
        {
            var context = new IndexContext();
            IndexManager.Initialization(context);
        }

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public virtual void Dispose()
        {
            IndexManager.Dispose();
        }

        public long GetUsedMemory()
        {
            long lngSessMemory = Process.GetCurrentProcess().WorkingSet64;

            return lngSessMemory;
        }

        public string GetRessource(string name)
        {
            var assembly = typeof(UnitTestIndexFixtureToken).Assembly;
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
