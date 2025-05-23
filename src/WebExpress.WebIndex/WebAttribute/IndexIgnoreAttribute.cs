﻿using System;

namespace WebExpress.WebIndex.WebAttribute
{
    /// <summary>
    /// Indicates that a property should not be indexed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class IndexIgnoreAttribute : Attribute, IIndexAttribute
    {
    }
}
