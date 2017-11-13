namespace System.Web.UI.Design
{
    using System;
    using System.Collections;
    using System.Design;
    using System.Globalization;
    using System.Reflection;

    public class DesignerRegionCollection : IList, ICollection, IEnumerable
    {
        private ArrayList _list;
        private ControlDesigner _owner;

        public DesignerRegionCollection()
        {
        }

        public DesignerRegionCollection(ControlDesigner owner)
        {
            this._owner = owner;
        }

        public int Add(DesignerRegion region) => 
            this.InternalList.Add(region);

        public void Clear()
        {
            this.InternalList.Clear();
        }

        public bool Contains(DesignerRegion region) => 
            this.InternalList.Contains(region);

        public void CopyTo(Array array, int index)
        {
            this.InternalList.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator() => 
            this.InternalList.GetEnumerator();

        public int IndexOf(DesignerRegion region) => 
            this.InternalList.IndexOf(region);

        public void Insert(int index, DesignerRegion region)
        {
            this.InternalList.Insert(index, region);
        }

        public void Remove(DesignerRegion region)
        {
            this.InternalList.Remove(region);
        }

        public void RemoveAt(int index)
        {
            this.InternalList.RemoveAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        int IList.Add(object o)
        {
            if (!(o is DesignerRegion))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, System.Design.SR.GetString("WrongType"), new object[] { "DesignerRegion" }), "o");
            }
            return this.Add((DesignerRegion) o);
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object o)
        {
            if (!(o is DesignerRegion))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, System.Design.SR.GetString("WrongType"), new object[] { "DesignerRegion" }), "o");
            }
            return this.Contains((DesignerRegion) o);
        }

        int IList.IndexOf(object o)
        {
            if (!(o is DesignerRegion))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, System.Design.SR.GetString("WrongType"), new object[] { "DesignerRegion" }), "o");
            }
            return this.IndexOf((DesignerRegion) o);
        }

        void IList.Insert(int index, object o)
        {
            if (!(o is DesignerRegion))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, System.Design.SR.GetString("WrongType"), new object[] { "DesignerRegion" }), "o");
            }
            this.Insert(index, (DesignerRegion) o);
        }

        void IList.Remove(object o)
        {
            if (!(o is DesignerRegion))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, System.Design.SR.GetString("WrongType"), new object[] { "DesignerRegion" }), "o");
            }
            this.Remove((DesignerRegion) o);
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        public int Count =>
            this.InternalList.Count;

        private ArrayList InternalList
        {
            get
            {
                if (this._list == null)
                {
                    this._list = new ArrayList();
                }
                return this._list;
            }
        }

        public bool IsFixedSize =>
            this.InternalList.IsFixedSize;

        public bool IsReadOnly =>
            this.InternalList.IsReadOnly;

        public bool IsSynchronized =>
            this.InternalList.IsSynchronized;

        public DesignerRegion this[int index]
        {
            get => 
                ((DesignerRegion) this.InternalList[index]);
            set
            {
                this.InternalList[index] = value;
            }
        }

        public ControlDesigner Owner =>
            this._owner;

        public object SyncRoot =>
            this.InternalList.SyncRoot;

        int ICollection.Count =>
            this.Count;

        bool ICollection.IsSynchronized =>
            this.IsSynchronized;

        object ICollection.SyncRoot =>
            this.SyncRoot;

        bool IList.IsFixedSize =>
            this.IsFixedSize;

        bool IList.IsReadOnly =>
            this.IsReadOnly;

        object IList.this[int index]
        {
            get => 
                this[index];
            set
            {
                if (!(value is DesignerRegion))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, System.Design.SR.GetString("WrongType"), new object[] { "DesignerRegion" }), "value");
                }
                this[index] = (DesignerRegion) value;
            }
        }
    }
}

