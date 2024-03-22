using System.Linq;
using System.Reflection;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// A read buffer item for buffering of segments.
    /// </summary>
    public class IndexStorageReadBufferItem
    {
        /// <summary>
        /// Returns or sets the lifetime.
        /// </summary>
        public static uint Lifetime { get; set; } = 100;

        /// <summary>
        /// The lifetime counter is for deletion from the buffer.
        /// </summary>
        private uint _counter;

        /// <summary>
        /// The lifetime counter is for deletion from the buffer.
        /// </summary>
        public uint Counter 
        { 
            get => _counter; 
            set 
            { 
                if (_counter < uint.MaxValue)
                {
                    _counter = value;
                } 
            }
        }

        /// <summary>
        /// The segment to be cached.
        /// </summary>
        public IIndexStorageSegment Segment;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="segment">The segment to be cached.</param>
        public IndexStorageReadBufferItem(IIndexStorageSegment segment)
        {
            Segment = segment;

            if (segment.GetType().GetCustomAttribute<SegmentCachedAttribute>() != null)
            {
                _counter = uint.MaxValue;
            }

            Refresh();
        }

        /// <summary>
        /// Refresh the lifetime
        /// </summary>
        public void Refresh()
        {
            Counter = Lifetime;
        }
    }
}