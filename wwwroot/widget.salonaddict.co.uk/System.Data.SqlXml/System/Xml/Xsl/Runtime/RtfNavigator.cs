namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml;
    using System.Xml.XPath;

    internal abstract class RtfNavigator : XPathNavigator
    {
        protected RtfNavigator()
        {
        }

        public abstract void CopyToWriter(XmlWriter writer);
        public override bool IsSamePosition(XPathNavigator other)
        {
            throw new NotSupportedException();
        }

        public override bool MoveToFirstAttribute()
        {
            throw new NotSupportedException();
        }

        public override bool MoveToFirstChild()
        {
            throw new NotSupportedException();
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            throw new NotSupportedException();
        }

        public override bool MoveToId(string id)
        {
            throw new NotSupportedException();
        }

        public override bool MoveToNext()
        {
            throw new NotSupportedException();
        }

        public override bool MoveToNextAttribute()
        {
            throw new NotSupportedException();
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            throw new NotSupportedException();
        }

        public override bool MoveToParent()
        {
            throw new NotSupportedException();
        }

        public override bool MoveToPrevious()
        {
            throw new NotSupportedException();
        }

        public abstract XPathNavigator ToNavigator();

        public override bool IsEmptyElement =>
            false;

        public override string LocalName =>
            string.Empty;

        public override string Name =>
            string.Empty;

        public override string NamespaceURI =>
            string.Empty;

        public override XmlNameTable NameTable
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override XPathNodeType NodeType =>
            XPathNodeType.Root;

        public override string Prefix =>
            string.Empty;
    }
}

