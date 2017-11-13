namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface ILifetimeContainer : IEnumerable<object>, IEnumerable, IDisposable
    {
        void Add(object item);
        bool Contains(object item);
        void Remove(object item);

        int Count { get; }
    }
}

