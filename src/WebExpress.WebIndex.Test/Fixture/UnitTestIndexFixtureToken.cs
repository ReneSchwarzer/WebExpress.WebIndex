using System.Diagnostics;
using System.Text;
using WebExpress.WebIndex.Term;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureToken : IDisposable
    {
        /// <summary>
        /// Returns the context.
        /// </summary>
        public IndexContext Context { get; private set; }

        /// <summary>
        /// Returns the token analyzer.
        /// </summary>
        public IndexTokenAnalyzer TokenAnalyzer { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureToken()
        {
            var context = new IndexContext();
            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            Context = context;

            TokenAnalyzer = new IndexTokenAnalyzer(Context);
        }

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public virtual void Dispose()
        {
            TokenAnalyzer.Dispose();
            Directory.Delete(Context.IndexDirectory, true);
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
