namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Collections.Generic;

    internal class UniqueList<T>
    {
        private List<T> list;
        private Dictionary<T, int> lookup;

        public UniqueList()
        {
            this.lookup = new Dictionary<T, int>();
            this.list = new List<T>();
        }

        public int Add(T value)
        {
            if (!this.lookup.ContainsKey(value))
            {
                int count = this.list.Count;
                this.lookup.Add(value, count);
                this.list.Add(value);
                return count;
            }
            return this.lookup[value];
        }

        public T[] ToArray() => 
            this.list.ToArray();
    }
}

