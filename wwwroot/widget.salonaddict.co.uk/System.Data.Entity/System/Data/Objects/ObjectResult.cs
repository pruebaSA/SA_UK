namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.ComponentModel;

    public abstract class ObjectResult : IEnumerable, IDisposable, IListSource
    {
        internal ObjectResult()
        {
        }

        public abstract void Dispose();
        internal abstract IEnumerator GetEnumeratorInternal();
        internal abstract IList GetIListSourceListInternal();
        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumeratorInternal();

        IList IListSource.GetList() => 
            this.GetIListSourceListInternal();

        public abstract Type ElementType { get; }

        bool IListSource.ContainsListCollection =>
            false;
    }
}

