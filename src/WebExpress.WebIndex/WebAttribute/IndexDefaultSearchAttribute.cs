using System;

namespace WebExpress.WebIndex.WebAttribute
{
    /// <summary>
    /// Indicates that a property is the default search attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class IndexDefaultSearchAttribute : Attribute, IIndexAttribute
    {
    }
}
