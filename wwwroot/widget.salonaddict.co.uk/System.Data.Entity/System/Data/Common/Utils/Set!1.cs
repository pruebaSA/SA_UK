namespace System.Data.Common.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class Set<TElement> : InternalBase, IEnumerable<TElement>, IEnumerable
    {
        private bool _isReadOnly;
        private readonly HashSet<TElement> _values;
        internal static readonly System.Data.Common.Utils.Set<TElement> Empty;
        internal static readonly IEqualityComparer<System.Data.Common.Utils.Set<TElement>> ValueComparer;

        static Set()
        {
            System.Data.Common.Utils.Set<TElement>.ValueComparer = new SetValueComparer<TElement>();
            System.Data.Common.Utils.Set<TElement>.Empty = new System.Data.Common.Utils.Set<TElement>().MakeReadOnly();
        }

        internal Set() : this(null, null)
        {
        }

        internal Set(IEnumerable<TElement> elements) : this(elements, null)
        {
        }

        internal Set(IEqualityComparer<TElement> comparer) : this(null, comparer)
        {
        }

        internal Set(System.Data.Common.Utils.Set<TElement> other) : this(other._values, other.Comparer)
        {
        }

        internal Set(IEnumerable<TElement> elements, IEqualityComparer<TElement> comparer)
        {
            this._values = new HashSet<TElement>(elements ?? Enumerable.Empty<TElement>(), comparer ?? EqualityComparer<TElement>.Default);
        }

        internal void Add(TElement element)
        {
            this._values.Add(element);
        }

        internal void AddRange(IEnumerable<TElement> elements)
        {
            foreach (TElement local in elements)
            {
                this.Add(local);
            }
        }

        internal System.Data.Common.Utils.Set<TElement> AsReadOnly()
        {
            if (this._isReadOnly)
            {
                return (System.Data.Common.Utils.Set<TElement>) this;
            }
            return new System.Data.Common.Utils.Set<TElement>((System.Data.Common.Utils.Set<TElement>) this) { _isReadOnly = true };
        }

        [Conditional("DEBUG")]
        private void AssertReadWrite()
        {
        }

        [Conditional("DEBUG")]
        private void AssertSetCompatible(System.Data.Common.Utils.Set<TElement> other)
        {
        }

        internal void Clear()
        {
            this._values.Clear();
        }

        internal bool Contains(TElement element) => 
            this._values.Contains(element);

        internal System.Data.Common.Utils.Set<TElement> Difference(IEnumerable<TElement> other)
        {
            System.Data.Common.Utils.Set<TElement> set = new System.Data.Common.Utils.Set<TElement>((System.Data.Common.Utils.Set<TElement>) this);
            set.Subtract(other);
            return set;
        }

        internal int GetElementsHashCode()
        {
            int num = 0;
            foreach (TElement local in this)
            {
                num ^= this.Comparer.GetHashCode(local);
            }
            return num;
        }

        public HashSet<TElement>.Enumerator GetEnumerator() => 
            this._values.GetEnumerator();

        internal void Intersect(System.Data.Common.Utils.Set<TElement> other)
        {
            this._values.IntersectWith(other._values);
        }

        internal bool IsSubsetOf(System.Data.Common.Utils.Set<TElement> other) => 
            this._values.IsSubsetOf(other._values);

        internal System.Data.Common.Utils.Set<TElement> MakeReadOnly()
        {
            this._isReadOnly = true;
            return (System.Data.Common.Utils.Set<TElement>) this;
        }

        internal bool Overlaps(System.Data.Common.Utils.Set<TElement> other) => 
            this._values.Overlaps(other._values);

        internal void Remove(TElement element)
        {
            this._values.Remove(element);
        }

        internal bool SetEquals(System.Data.Common.Utils.Set<TElement> other) => 
            ((this._values.Count == other._values.Count) && this._values.IsSubsetOf(other._values));

        internal void Subtract(IEnumerable<TElement> other)
        {
            this._values.ExceptWith(other);
        }

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() => 
            this.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        internal TElement[] ToArray() => 
            this._values.ToArray<TElement>();

        internal override void ToCompactString(StringBuilder builder)
        {
            StringUtil.ToCommaSeparatedStringSorted(builder, this);
        }

        internal System.Data.Common.Utils.Set<TElement> Union(IEnumerable<TElement> other)
        {
            System.Data.Common.Utils.Set<TElement> set = new System.Data.Common.Utils.Set<TElement>((System.Data.Common.Utils.Set<TElement>) this);
            set.Unite(other);
            return set;
        }

        internal void Unite(IEnumerable<TElement> other)
        {
            this._values.UnionWith(other);
        }

        internal IEqualityComparer<TElement> Comparer =>
            this._values.Comparer;

        internal int Count =>
            this._values.Count;

        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<TElement>, IDisposable, IEnumerator
        {
            private Dictionary<TElement, bool>.KeyCollection.Enumerator keys;
            internal Enumerator(Dictionary<TElement, bool>.KeyCollection.Enumerator keys)
            {
                this.keys = keys;
            }

            public TElement Current =>
                this.keys.Current;
            public void Dispose()
            {
                this.keys.Dispose();
            }

            object IEnumerator.Current =>
                ((IEnumerator) this.keys).Current;
            public bool MoveNext() => 
                this.keys.MoveNext();

            void IEnumerator.Reset()
            {
                this.keys.Reset();
            }
        }

        private class SetValueComparer : IEqualityComparer<System.Data.Common.Utils.Set<TElement>>
        {
            bool IEqualityComparer<System.Data.Common.Utils.Set<TElement>>.Equals(System.Data.Common.Utils.Set<TElement> x, System.Data.Common.Utils.Set<TElement> y) => 
                x.SetEquals(y);

            int IEqualityComparer<System.Data.Common.Utils.Set<TElement>>.GetHashCode(System.Data.Common.Utils.Set<TElement> obj) => 
                obj.GetElementsHashCode();
        }
    }
}

