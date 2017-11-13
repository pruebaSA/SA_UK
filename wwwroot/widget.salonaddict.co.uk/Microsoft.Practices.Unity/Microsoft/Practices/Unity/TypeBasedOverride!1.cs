namespace Microsoft.Practices.Unity
{
    using System;

    public class TypeBasedOverride<T> : TypeBasedOverride
    {
        public TypeBasedOverride(ResolverOverride innerOverride) : base(typeof(T), innerOverride)
        {
        }
    }
}

