namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public class NamedTypeBuildKey<T> : NamedTypeBuildKey
    {
        public NamedTypeBuildKey() : base(typeof(T), null)
        {
        }

        public NamedTypeBuildKey(string name) : base(typeof(T), name)
        {
        }
    }
}

