namespace Microsoft.VisualC
{
    using System;

    [AttributeUsage(AttributeTargets.All), Obsolete("Microsoft.VisualC.dll is an obsolete assembly and exists only for backwards compatibility.")]
    public sealed class DecoratedNameAttribute : Attribute
    {
        public DecoratedNameAttribute()
        {
        }

        public DecoratedNameAttribute(string decoratedName)
        {
        }
    }
}

