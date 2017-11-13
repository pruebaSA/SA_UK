namespace System.CodeDom
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class CodeNamespaceImportCollection : IList, ICollection, IEnumerable
    {
        private ArrayList data = new ArrayList();
        private Hashtable keys = new Hashtable(StringComparer.OrdinalIgnoreCase);

        public void Add(CodeNamespaceImport value)
        {
            if (!this.keys.ContainsKey(value.Namespace))
            {
                this.keys[value.Namespace] = value;
                this.data.Add(value);
            }
        }

        public void AddRange(CodeNamespaceImport[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            foreach (CodeNamespaceImport import in value)
            {
                this.Add(import);
            }
        }

        public void Clear()
        {
            this.data.Clear();
            this.keys.Clear();
        }

        public IEnumerator GetEnumerator() => 
            this.data.GetEnumerator();

        private void SyncKeys()
        {
            this.keys = new Hashtable(StringComparer.OrdinalIgnoreCase);
            foreach (CodeNamespaceImport import in this)
            {
                this.keys[import.Namespace] = import;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.data.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        int IList.Add(object value) => 
            this.data.Add((CodeNamespaceImport) value);

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object value) => 
            this.data.Contains(value);

        int IList.IndexOf(object value) => 
            this.data.IndexOf((CodeNamespaceImport) value);

        void IList.Insert(int index, object value)
        {
            this.data.Insert(index, (CodeNamespaceImport) value);
            this.SyncKeys();
        }

        void IList.Remove(object value)
        {
            this.data.Remove((CodeNamespaceImport) value);
            this.SyncKeys();
        }

        void IList.RemoveAt(int index)
        {
            this.data.RemoveAt(index);
            this.SyncKeys();
        }

        public int Count =>
            this.data.Count;

        public CodeNamespaceImport this[int index]
        {
            get => 
                ((CodeNamespaceImport) this.data[index]);
            set
            {
                this.data[index] = value;
                this.SyncKeys();
            }
        }

        int ICollection.Count =>
            this.Count;

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            null;

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
                this[index] = (CodeNamespaceImport) value;
                this.SyncKeys();
            }
        }
    }
}

