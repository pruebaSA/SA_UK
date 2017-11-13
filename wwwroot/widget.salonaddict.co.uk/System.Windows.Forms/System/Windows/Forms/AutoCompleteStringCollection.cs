namespace System.Windows.Forms
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Reflection;
    using System.Security.Permissions;

    public class AutoCompleteStringCollection : IList, ICollection, IEnumerable
    {
        private ArrayList data = new ArrayList();

        public event CollectionChangeEventHandler CollectionChanged;

        public int Add(string value)
        {
            int num = this.data.Add(value);
            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
            return num;
        }

        public void AddRange(string[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.data.AddRange(value);
            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public void Clear()
        {
            this.data.Clear();
            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public bool Contains(string value) => 
            this.data.Contains(value);

        public void CopyTo(string[] array, int index)
        {
            this.data.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator() => 
            this.data.GetEnumerator();

        public int IndexOf(string value) => 
            this.data.IndexOf(value);

        public void Insert(int index, string value)
        {
            this.data.Insert(index, value);
            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
        }

        protected void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            if (this.onCollectionChanged != null)
            {
                this.onCollectionChanged(this, e);
            }
        }

        public void Remove(string value)
        {
            this.data.Remove(value);
            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
        }

        public void RemoveAt(int index)
        {
            string element = (string) this.data[index];
            this.data.RemoveAt(index);
            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, element));
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.data.CopyTo(array, index);
        }

        int IList.Add(object value) => 
            this.Add((string) value);

        bool IList.Contains(object value) => 
            this.Contains((string) value);

        int IList.IndexOf(object value) => 
            this.IndexOf((string) value);

        void IList.Insert(int index, object value)
        {
            this.Insert(index, (string) value);
        }

        void IList.Remove(object value)
        {
            this.Remove((string) value);
        }

        public int Count =>
            this.data.Count;

        public bool IsReadOnly =>
            false;

        public bool IsSynchronized =>
            false;

        public string this[int index]
        {
            get => 
                ((string) this.data[index]);
            set
            {
                this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, this.data[index]));
                this.data[index] = value;
                this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
            }
        }

        public object SyncRoot =>
            this;

        bool IList.IsFixedSize =>
            false;

        bool IList.IsReadOnly =>
            false;

        object IList.this[int index]
        {
            get => 
                this[index];
            set
            {
                this[index] = (string) value;
            }
        }
    }
}

