namespace System.Collections.Specialized
{
    using System;
    using System.Collections;
    using System.Reflection;

    [Serializable]
    public class StringCollection : IList, ICollection, IEnumerable
    {
        private ArrayList data = new ArrayList();

        public int Add(string value) => 
            this.data.Add(value);

        public void AddRange(string[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.data.AddRange(value);
        }

        public void Clear()
        {
            this.data.Clear();
        }

        public bool Contains(string value) => 
            this.data.Contains(value);

        public void CopyTo(string[] array, int index)
        {
            this.data.CopyTo(array, index);
        }

        public StringEnumerator GetEnumerator() => 
            new StringEnumerator(this);

        public int IndexOf(string value) => 
            this.data.IndexOf(value);

        public void Insert(int index, string value)
        {
            this.data.Insert(index, value);
        }

        public void Remove(string value)
        {
            this.data.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this.data.RemoveAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.data.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.data.GetEnumerator();

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
                this.data[index] = value;
            }
        }

        public object SyncRoot =>
            this.data.SyncRoot;

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

