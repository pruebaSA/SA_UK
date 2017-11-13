namespace System.Xml.Schema
{
    using System;
    using System.Collections;

    public sealed class XmlSchemaCollectionEnumerator : IEnumerator
    {
        private IDictionaryEnumerator enumerator;

        internal XmlSchemaCollectionEnumerator(Hashtable collection)
        {
            this.enumerator = collection.GetEnumerator();
        }

        public bool MoveNext() => 
            this.enumerator.MoveNext();

        bool IEnumerator.MoveNext() => 
            this.enumerator.MoveNext();

        void IEnumerator.Reset()
        {
            this.enumerator.Reset();
        }

        public XmlSchema Current
        {
            get
            {
                XmlSchemaCollectionNode node = (XmlSchemaCollectionNode) this.enumerator.Value;
                if (node != null)
                {
                    return node.Schema;
                }
                return null;
            }
        }

        internal XmlSchemaCollectionNode CurrentNode =>
            ((XmlSchemaCollectionNode) this.enumerator.Value);

        object IEnumerator.Current =>
            this.Current;
    }
}

