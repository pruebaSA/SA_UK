namespace System.Data.Linq
{
    using System;
    using System.Collections.Generic;

    internal static class SourceState<T>
    {
        internal static readonly IEnumerable<T> Assigned;
        internal static readonly IEnumerable<T> Loaded;

        static SourceState()
        {
            SourceState<T>.Loaded = (IEnumerable<T>) new T[0];
            SourceState<T>.Assigned = (IEnumerable<T>) new T[0];
        }
    }
}

