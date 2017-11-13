namespace AjaxControlToolkit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web.UI;

    public class ReorderListItemCollection : IList<ReorderListItem>, ICollection<ReorderListItem>, IEnumerable<ReorderListItem>, IEnumerable
    {
        private ReorderList _parent;

        public ReorderListItemCollection(ReorderList parent)
        {
            this._parent = parent;
        }

        public void Add(ReorderListItem item)
        {
            this.ChildList.Add(item);
        }

        public void Clear()
        {
            this.ChildList.Clear();
        }

        public bool Contains(ReorderListItem item) => 
            this.ChildList.Contains(item);

        public void CopyTo(ReorderListItem[] array, int arrayIndex)
        {
            this.ChildList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ReorderListItem> GetEnumerator() => 
            new ReorderListItemEnumerator(this.ChildList.GetEnumerator());

        public int IndexOf(ReorderListItem item) => 
            this.ChildList.IndexOf(item);

        public void Insert(int index, ReorderListItem item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(ReorderListItem item)
        {
            this.ChildList.Remove(item);
            return true;
        }

        public void RemoveAt(int index)
        {
            this.ChildList.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.ChildList.GetEnumerator();

        private ControlCollection ChildList =>
            this._parent.ChildList.Controls;

        public int Count =>
            this.ChildList.Count;

        public bool IsReadOnly =>
            this.ChildList.IsReadOnly;

        public ReorderListItem this[int index]
        {
            get => 
                ((ReorderListItem) this.ChildList[index]);
            set
            {
                throw new NotSupportedException();
            }
        }

        private class ReorderListItemEnumerator : IEnumerator<ReorderListItem>, IDisposable, IEnumerator
        {
            private IEnumerator _controlEnum;

            public ReorderListItemEnumerator(IEnumerator baseEnum)
            {
                this._controlEnum = baseEnum;
            }

            public void Dispose()
            {
                this._controlEnum = null;
                GC.SuppressFinalize(this);
            }

            public bool MoveNext() => 
                this._controlEnum.MoveNext();

            public void Reset()
            {
                this._controlEnum.Reset();
            }

            public ReorderListItem Current =>
                ((ReorderListItem) this._controlEnum.Current);

            object IEnumerator.Current =>
                ((ReorderListItem) this._controlEnum.Current);
        }
    }
}

