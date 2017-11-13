namespace System.ComponentModel
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class ListSortDescriptionCollection : IList, ICollection, IEnumerable
    {
        private ArrayList sorts;

        public ListSortDescriptionCollection()
        {
            this.sorts = new ArrayList();
        }

        public ListSortDescriptionCollection(ListSortDescription[] sorts)
        {
            this.sorts = new ArrayList();
            if (sorts != null)
            {
                for (int i = 0; i < sorts.Length; i++)
                {
                    this.sorts.Add(sorts[i]);
                }
            }
        }

        public bool Contains(object value) => 
            this.sorts.Contains(value);

        public void CopyTo(Array array, int index)
        {
            this.sorts.CopyTo(array, index);
        }

        public int IndexOf(object value) => 
            this.sorts.IndexOf(value);

        IEnumerator IEnumerable.GetEnumerator() => 
            this.sorts.GetEnumerator();

        int IList.Add(object value)
        {
            throw new InvalidOperationException(SR.GetString("CantModifyListSortDescriptionCollection"));
        }

        void IList.Clear()
        {
            throw new InvalidOperationException(SR.GetString("CantModifyListSortDescriptionCollection"));
        }

        void IList.Insert(int index, object value)
        {
            throw new InvalidOperationException(SR.GetString("CantModifyListSortDescriptionCollection"));
        }

        void IList.Remove(object value)
        {
            throw new InvalidOperationException(SR.GetString("CantModifyListSortDescriptionCollection"));
        }

        void IList.RemoveAt(int index)
        {
            throw new InvalidOperationException(SR.GetString("CantModifyListSortDescriptionCollection"));
        }

        public int Count =>
            this.sorts.Count;

        public ListSortDescription this[int index]
        {
            get => 
                ((ListSortDescription) this.sorts[index]);
            set
            {
                throw new InvalidOperationException(SR.GetString("CantModifyListSortDescriptionCollection"));
            }
        }

        bool ICollection.IsSynchronized =>
            true;

        object ICollection.SyncRoot =>
            this;

        bool IList.IsFixedSize =>
            true;

        bool IList.IsReadOnly =>
            true;

        object IList.this[int index]
        {
            get => 
                this[index];
            set
            {
                throw new InvalidOperationException(SR.GetString("CantModifyListSortDescriptionCollection"));
            }
        }
    }
}

