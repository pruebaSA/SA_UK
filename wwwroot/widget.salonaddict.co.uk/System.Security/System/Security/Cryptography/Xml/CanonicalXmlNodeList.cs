namespace System.Security.Cryptography.Xml
{
    using System;
    using System.Collections;
    using System.Security;
    using System.Xml;

    internal class CanonicalXmlNodeList : XmlNodeList, IList, ICollection, IEnumerable
    {
        private ArrayList m_nodeArray = new ArrayList();

        internal CanonicalXmlNodeList()
        {
        }

        public int Add(object value)
        {
            if (!(value is XmlNode))
            {
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_IncorrectObjectType"), "node");
            }
            return this.m_nodeArray.Add(value);
        }

        public void Clear()
        {
            this.m_nodeArray.Clear();
        }

        public bool Contains(object value) => 
            this.m_nodeArray.Contains(value);

        public void CopyTo(Array array, int index)
        {
            this.m_nodeArray.CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator() => 
            this.m_nodeArray.GetEnumerator();

        public int IndexOf(object value) => 
            this.m_nodeArray.IndexOf(value);

        public void Insert(int index, object value)
        {
            if (!(value is XmlNode))
            {
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_IncorrectObjectType"), "value");
            }
            this.m_nodeArray.Insert(index, value);
        }

        public override XmlNode Item(int index) => 
            ((XmlNode) this.m_nodeArray[index]);

        public void Remove(object value)
        {
            this.m_nodeArray.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this.m_nodeArray.RemoveAt(index);
        }

        public override int Count =>
            this.m_nodeArray.Count;

        public bool IsFixedSize =>
            this.m_nodeArray.IsFixedSize;

        public bool IsReadOnly =>
            this.m_nodeArray.IsReadOnly;

        public bool IsSynchronized =>
            this.m_nodeArray.IsSynchronized;

        public object SyncRoot =>
            this.m_nodeArray.SyncRoot;

        object IList.this[int index]
        {
            get => 
                this.m_nodeArray[index];
            set
            {
                if (!(value is XmlNode))
                {
                    throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_IncorrectObjectType"), "value");
                }
                this.m_nodeArray[index] = value;
            }
        }
    }
}

