﻿namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class ReadOnlyCollectionExtensions
    {
        internal static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                return DefaultReadOnlyCollection<T>.Empty;
            }
            ReadOnlyCollection<T> onlys = sequence as ReadOnlyCollection<T>;
            if (onlys != null)
            {
                return onlys;
            }
            return new ReadOnlyCollection<T>(sequence.ToArray<T>());
        }

        private static class DefaultReadOnlyCollection<T>
        {
            private static ReadOnlyCollection<T> _defaultCollection;

            internal static ReadOnlyCollection<T> Empty
            {
                get
                {
                    if (ReadOnlyCollectionExtensions.DefaultReadOnlyCollection<T>._defaultCollection == null)
                    {
                        ReadOnlyCollectionExtensions.DefaultReadOnlyCollection<T>._defaultCollection = new ReadOnlyCollection<T>(new T[0]);
                    }
                    return ReadOnlyCollectionExtensions.DefaultReadOnlyCollection<T>._defaultCollection;
                }
            }
        }
    }
}

