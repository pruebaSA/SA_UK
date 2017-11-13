namespace System.Xml.Schema
{
    using System;
    using System.Collections;

    public class XmlSchemaObjectEnumerator : IEnumerator
    {
        private IEnumerator enumerator;

        internal XmlSchemaObjectEnumerator(IEnumerator enumerator)
        {
            this.enumerator = enumerator;
        }

        public bool MoveNext() => 
            this.enumerator.MoveNext();

        public void Reset()
        {
            this.enumerator.Reset();
        }

        bool IEnumerator.MoveNext() => 
            this.enumerator.MoveNext();

        void IEnumerator.Reset()
        {
            this.enumerator.Reset();
        }

        public XmlSchemaObject Current =>
            ((XmlSchemaObject) this.enumerator.Current);

        object IEnumerator.Current =>
            this.enumerator.Current;
    }
}

