using System;

namespace WebExpress.WebIndex.WebAttribute
{
    /// <summary>
    /// Indicates that a segment is permanently cached.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal class SegmentCachedAttribute : Attribute
    {
    }
}
