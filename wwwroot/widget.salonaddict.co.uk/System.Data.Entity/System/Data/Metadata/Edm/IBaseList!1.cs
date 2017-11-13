namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal interface IBaseList<T> : IList, ICollection, IEnumerable
    {
        int IndexOf(T item);

        T this[string identity] { get; }

        T this[int index] { get; }
    }
}

