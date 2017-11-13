namespace System.DirectoryServices
{
    using System;
    using System.Collections;
    using System.DirectoryServices.Interop;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class SchemaNameCollection : IList, ICollection, IEnumerable
    {
        private VariantPropGetter propGetter;
        private VariantPropSetter propSetter;

        internal SchemaNameCollection(VariantPropGetter propGetter, VariantPropSetter propSetter)
        {
            this.propGetter = propGetter;
            this.propSetter = propSetter;
        }

        public int Add(string value)
        {
            object[] objArray = this.GetValue();
            object[] objArray2 = new object[objArray.Length + 1];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray2[i] = objArray[i];
            }
            objArray2[objArray2.Length - 1] = value;
            this.propSetter(objArray2);
            return (objArray2.Length - 1);
        }

        public void AddRange(string[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            object[] objArray = this.GetValue();
            object[] objArray2 = new object[objArray.Length + value.Length];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray2[i] = objArray[i];
            }
            for (int j = objArray.Length; j < objArray2.Length; j++)
            {
                objArray2[j] = value[j - objArray.Length];
            }
            this.propSetter(objArray2);
        }

        public void AddRange(SchemaNameCollection value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            object[] objArray = this.GetValue();
            object[] objArray2 = new object[objArray.Length + value.Count];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray2[i] = objArray[i];
            }
            for (int j = objArray.Length; j < objArray2.Length; j++)
            {
                objArray2[j] = value[j - objArray.Length];
            }
            this.propSetter(objArray2);
        }

        public void Clear()
        {
            object[] objArray = new object[0];
            this.propSetter(objArray);
        }

        public bool Contains(string value) => 
            (this.IndexOf(value) != -1);

        public void CopyTo(string[] stringArray, int index)
        {
            this.GetValue().CopyTo(stringArray, index);
        }

        public IEnumerator GetEnumerator() => 
            this.GetValue().GetEnumerator();

        private object[] GetValue()
        {
            object obj2 = this.propGetter();
            if (obj2 == null)
            {
                return new object[0];
            }
            return (object[]) obj2;
        }

        public int IndexOf(string value)
        {
            object[] objArray = this.GetValue();
            for (int i = 0; i < objArray.Length; i++)
            {
                if (value == ((string) objArray[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, string value)
        {
            ArrayList list = new ArrayList(this.GetValue());
            list.Insert(index, value);
            this.propSetter(list.ToArray());
        }

        public void Remove(string value)
        {
            int index = this.IndexOf(value);
            this.RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            object[] objArray = this.GetValue();
            if ((index >= objArray.Length) || (index < 0))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            object[] objArray2 = new object[objArray.Length - 1];
            for (int i = 0; i < index; i++)
            {
                objArray2[i] = objArray[i];
            }
            for (int j = index + 1; j < objArray.Length; j++)
            {
                objArray2[j - 1] = objArray[j];
            }
            this.propSetter(objArray2);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.GetValue().CopyTo(array, index);
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
            this.GetValue().Length;

        public string this[int index]
        {
            get => 
                ((string) this.GetValue()[index]);
            set
            {
                object[] objArray = this.GetValue();
                objArray[index] = value;
                this.propSetter(objArray);
            }
        }

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
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

        internal class FilterDelegateWrapper
        {
            private UnsafeNativeMethods.IAdsContainer obj;

            internal FilterDelegateWrapper(UnsafeNativeMethods.IAdsContainer wrapped)
            {
                this.obj = wrapped;
            }

            private object GetFilter() => 
                this.obj.Filter;

            private void SetFilter(object value)
            {
                this.obj.Filter = value;
            }

            public SchemaNameCollection.VariantPropGetter Getter =>
                new SchemaNameCollection.VariantPropGetter(this.GetFilter);

            public SchemaNameCollection.VariantPropSetter Setter =>
                new SchemaNameCollection.VariantPropSetter(this.SetFilter);
        }

        internal delegate object VariantPropGetter();

        internal delegate void VariantPropSetter(object value);
    }
}

