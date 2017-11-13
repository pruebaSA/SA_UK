namespace AjaxControlToolkit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Web.UI;

    public sealed class AccordionPaneCollection : IList, ICollection, IEnumerable<AccordionPane>, IEnumerable
    {
        private Accordion _parent;
        private int _version;

        internal AccordionPaneCollection(Accordion parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent", "Parent Accordion cannot be null.");
            }
            this._parent = parent;
        }

        public void Add(AccordionPane item)
        {
            this._parent.Controls.Add(item);
            this._version++;
        }

        public void Clear()
        {
            this._parent.ClearPanes();
            this._version++;
        }

        public bool Contains(AccordionPane item) => 
            this._parent.Controls.Contains(item);

        public void CopyTo(Array array, int index)
        {
            AccordionPane[] paneArray = array as AccordionPane[];
            if (paneArray == null)
            {
                throw new ArgumentException("Expected an array of AccordionPanes.");
            }
            this.CopyTo(paneArray, index);
        }

        public void CopyTo(AccordionPane[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array", "Cannot copy into a null array.");
            }
            int num = 0;
            for (int i = 0; i < this._parent.Controls.Count; i++)
            {
                AccordionPane pane = this._parent.Controls[i] as AccordionPane;
                if (pane != null)
                {
                    if ((num + index) == array.Length)
                    {
                        throw new ArgumentException("Array is not large enough for the AccordionPanes");
                    }
                    array[num++ + index] = pane;
                }
            }
        }

        private int FromRawIndex(int index)
        {
            if (index < 0)
            {
                return -1;
            }
            int num = -1;
            for (int i = 0; i < this._parent.Controls.Count; i++)
            {
                if (this._parent.Controls[i] is AccordionPane)
                {
                    num++;
                }
                if (index == i)
                {
                    return num;
                }
            }
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "No AccordionPane at position {0}", new object[] { index }));
        }

        public IEnumerator<AccordionPane> GetEnumerator() => 
            new AccordionPaneEnumerator(this);

        public int IndexOf(AccordionPane item) => 
            this.FromRawIndex(this._parent.Controls.IndexOf(item));

        public void Insert(int index, AccordionPane item)
        {
            this._parent.Controls.AddAt(this.ToRawIndex(index), item);
            this._version++;
        }

        public void Remove(AccordionPane item)
        {
            this._parent.Controls.Remove(item);
            this._version++;
        }

        public void RemoveAt(int index)
        {
            this._parent.Controls.RemoveAt(this.ToRawIndex(index));
            this._version++;
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new AccordionPaneEnumerator(this);

        int IList.Add(object value)
        {
            this.Add(value as AccordionPane);
            return 0;
        }

        bool IList.Contains(object value) => 
            this.Contains(value as AccordionPane);

        int IList.IndexOf(object value) => 
            this.IndexOf(value as AccordionPane);

        void IList.Insert(int index, object value)
        {
            this.Insert(index, value as AccordionPane);
        }

        void IList.Remove(object value)
        {
            this.Remove(value as AccordionPane);
        }

        private int ToRawIndex(int paneIndex)
        {
            if (paneIndex < 0)
            {
                return -1;
            }
            int num = -1;
            for (int i = 0; i < this._parent.Controls.Count; i++)
            {
                if ((this._parent.Controls[i] is AccordionPane) && (++num == paneIndex))
                {
                    return i;
                }
            }
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "No AccordionPane at position {0}", new object[] { paneIndex }));
        }

        public int Count
        {
            get
            {
                int num = 0;
                foreach (Control control in this._parent.Controls)
                {
                    if (control is AccordionPane)
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        public bool IsReadOnly =>
            false;

        public AccordionPane this[int index] =>
            (this._parent.Controls[this.ToRawIndex(index)] as AccordionPane);

        public AccordionPane this[string id]
        {
            get
            {
                for (int i = 0; i < this._parent.Controls.Count; i++)
                {
                    AccordionPane pane = this._parent.Controls[i] as AccordionPane;
                    if ((pane != null) && (pane.ID == id))
                    {
                        return pane;
                    }
                }
                return null;
            }
        }

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IList.IsFixedSize =>
            false;

        object IList.this[int index]
        {
            get => 
                this[index];
            set
            {
            }
        }

        private class AccordionPaneEnumerator : IEnumerator<AccordionPane>, IDisposable, IEnumerator
        {
            private AccordionPaneCollection _collection;
            private IEnumerator _parentEnumerator;
            private int _version;

            public AccordionPaneEnumerator(AccordionPaneCollection parent)
            {
                this._collection = parent;
                this._parentEnumerator = parent._parent.Controls.GetEnumerator();
                this._version = parent._version;
            }

            private void CheckVersion()
            {
                if (this._version != this._collection._version)
                {
                    throw new InvalidOperationException("Enumeration can't continue because the collection has been modified.");
                }
            }

            public void Dispose()
            {
                this._parentEnumerator = null;
                this._collection = null;
                GC.SuppressFinalize(this);
            }

            public bool MoveNext()
            {
                this.CheckVersion();
                bool flag = this._parentEnumerator.MoveNext();
                if (flag && !(this._parentEnumerator.Current is AccordionPane))
                {
                    flag = this.MoveNext();
                }
                return flag;
            }

            public void Reset()
            {
                this.CheckVersion();
                this._parentEnumerator.Reset();
            }

            public AccordionPane Current
            {
                get
                {
                    this.CheckVersion();
                    return (this._parentEnumerator.Current as AccordionPane);
                }
            }

            object IEnumerator.Current =>
                this.Current;
        }
    }
}

