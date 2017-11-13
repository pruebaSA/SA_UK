namespace System.ComponentModel.Design
{
    using System;
    using System.Collections;
    using System.Reflection;

    public sealed class DesignSurfaceCollection : ICollection, IEnumerable
    {
        private DesignerCollection _designers;

        internal DesignSurfaceCollection(DesignerCollection designers)
        {
            this._designers = designers;
            if (this._designers == null)
            {
                this._designers = new DesignerCollection(null);
            }
        }

        public void CopyTo(DesignSurface[] array, int index)
        {
            ((ICollection) this).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator() => 
            new DesignSurfaceEnumerator(this._designers.GetEnumerator());

        void ICollection.CopyTo(Array array, int index)
        {
            foreach (DesignSurface surface in this)
            {
                array.SetValue(surface, index++);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public int Count =>
            this._designers.Count;

        public DesignSurface this[int index]
        {
            get
            {
                IDesignerHost host = this._designers[index];
                DesignSurface service = host.GetService(typeof(DesignSurface)) as DesignSurface;
                if (service == null)
                {
                    throw new NotSupportedException();
                }
                return service;
            }
        }

        int ICollection.Count =>
            this.Count;

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            null;

        private class DesignSurfaceEnumerator : IEnumerator
        {
            private IEnumerator _designerEnumerator;

            internal DesignSurfaceEnumerator(IEnumerator designerEnumerator)
            {
                this._designerEnumerator = designerEnumerator;
            }

            public bool MoveNext() => 
                this._designerEnumerator.MoveNext();

            public void Reset()
            {
                this._designerEnumerator.Reset();
            }

            public object Current
            {
                get
                {
                    IDesignerHost current = (IDesignerHost) this._designerEnumerator.Current;
                    DesignSurface service = current.GetService(typeof(DesignSurface)) as DesignSurface;
                    if (service == null)
                    {
                        throw new NotSupportedException();
                    }
                    return service;
                }
            }
        }
    }
}

