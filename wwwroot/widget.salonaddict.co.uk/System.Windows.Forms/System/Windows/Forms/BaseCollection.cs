namespace System.Windows.Forms
{
    using System;
    using System.Collections;
    using System.ComponentModel;

    public class BaseCollection : MarshalByRefObject, ICollection, IEnumerable
    {
        public void CopyTo(Array ar, int index)
        {
            this.List.CopyTo(ar, index);
        }

        public IEnumerator GetEnumerator() => 
            this.List.GetEnumerator();

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual int Count =>
            this.List.Count;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsReadOnly =>
            false;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsSynchronized =>
            false;

        protected virtual ArrayList List =>
            null;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public object SyncRoot =>
            this;
    }
}

