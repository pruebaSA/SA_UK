namespace System.Windows.Forms
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Reflection;

    [ListBindable(false)]
    public class DataGridViewSelectedRowCollection : BaseCollection, IList, ICollection, IEnumerable
    {
        private ArrayList items = new ArrayList();

        internal DataGridViewSelectedRowCollection()
        {
        }

        internal int Add(DataGridViewRow dataGridViewRow) => 
            this.items.Add(dataGridViewRow);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Clear()
        {
            throw new NotSupportedException(System.Windows.Forms.SR.GetString("DataGridView_ReadOnlyCollection"));
        }

        public bool Contains(DataGridViewRow dataGridViewRow) => 
            (this.items.IndexOf(dataGridViewRow) != -1);

        public void CopyTo(DataGridViewRow[] array, int index)
        {
            this.items.CopyTo(array, index);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Insert(int index, DataGridViewRow dataGridViewRow)
        {
            throw new NotSupportedException(System.Windows.Forms.SR.GetString("DataGridView_ReadOnlyCollection"));
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.items.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.items.GetEnumerator();

        int IList.Add(object value)
        {
            throw new NotSupportedException(System.Windows.Forms.SR.GetString("DataGridView_ReadOnlyCollection"));
        }

        void IList.Clear()
        {
            throw new NotSupportedException(System.Windows.Forms.SR.GetString("DataGridView_ReadOnlyCollection"));
        }

        bool IList.Contains(object value) => 
            this.items.Contains(value);

        int IList.IndexOf(object value) => 
            this.items.IndexOf(value);

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException(System.Windows.Forms.SR.GetString("DataGridView_ReadOnlyCollection"));
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException(System.Windows.Forms.SR.GetString("DataGridView_ReadOnlyCollection"));
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException(System.Windows.Forms.SR.GetString("DataGridView_ReadOnlyCollection"));
        }

        public DataGridViewRow this[int index] =>
            ((DataGridViewRow) this.items[index]);

        protected override ArrayList List =>
            this.items;

        int ICollection.Count =>
            this.items.Count;

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            this;

        bool IList.IsFixedSize =>
            true;

        bool IList.IsReadOnly =>
            true;

        object IList.this[int index]
        {
            get => 
                this.items[index];
            set
            {
                throw new NotSupportedException(System.Windows.Forms.SR.GetString("DataGridView_ReadOnlyCollection"));
            }
        }
    }
}

