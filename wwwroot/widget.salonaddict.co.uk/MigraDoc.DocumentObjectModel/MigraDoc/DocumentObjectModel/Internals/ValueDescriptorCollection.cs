namespace MigraDoc.DocumentObjectModel.Internals
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class ValueDescriptorCollection : IEnumerable
    {
        private ArrayList arrayList = new ArrayList();
        private Hashtable hashTable = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

        public int Add(ValueDescriptor vd)
        {
            this.hashTable.Add(vd.ValueName, vd);
            return this.arrayList.Add(vd);
        }

        public IEnumerator GetEnumerator() => 
            this.arrayList.GetEnumerator();

        public int Count =>
            this.arrayList.Count;

        public ValueDescriptor this[int index] =>
            (this.arrayList[index] as ValueDescriptor);

        public ValueDescriptor this[string name] =>
            (this.hashTable[name] as ValueDescriptor);
    }
}

