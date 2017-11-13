namespace System.Xml
{
    using System;
    using System.Collections;

    internal class XmlEmptyElementListEnumerator : IEnumerator
    {
        public XmlEmptyElementListEnumerator(XmlElementList list)
        {
        }

        public bool MoveNext() => 
            false;

        public void Reset()
        {
        }

        public object Current =>
            null;
    }
}

