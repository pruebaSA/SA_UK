namespace System.ComponentModel.Design.Data
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class DataSourceGroupCollection : CollectionBase
    {
        public int Add(DataSourceGroup value) => 
            base.List.Add(value);

        public bool Contains(DataSourceGroup value) => 
            base.List.Contains(value);

        public void CopyTo(DataSourceGroup[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        public int IndexOf(DataSourceGroup value) => 
            base.List.IndexOf(value);

        public void Insert(int index, DataSourceGroup value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(DataSourceGroup value)
        {
            base.List.Remove(value);
        }

        public DataSourceGroup this[int index]
        {
            get => 
                ((DataSourceGroup) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }
    }
}

