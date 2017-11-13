namespace System.Xml.XPath
{
    using System;
    using System.Xml;

    internal class XmlEmptyNavigator : XPathNavigator
    {
        private static XmlEmptyNavigator singleton;

        private XmlEmptyNavigator()
        {
        }

        public override XPathNavigator Clone() => 
            this;

        public override XmlNodeOrder ComparePosition(XPathNavigator other)
        {
            if (this != other)
            {
                return XmlNodeOrder.Unknown;
            }
            return XmlNodeOrder.Same;
        }

        public override string GetAttribute(string localName, string namespaceName) => 
            null;

        public override string GetNamespace(string name) => 
            null;

        public override bool IsSamePosition(XPathNavigator other) => 
            (this == other);

        public override bool MoveTo(XPathNavigator other) => 
            (this == other);

        public override bool MoveToAttribute(string localName, string namespaceName) => 
            false;

        public override bool MoveToFirst() => 
            false;

        public override bool MoveToFirstAttribute() => 
            false;

        public override bool MoveToFirstChild() => 
            false;

        public override bool MoveToFirstNamespace(XPathNamespaceScope scope) => 
            false;

        public override bool MoveToId(string id) => 
            false;

        public override bool MoveToNamespace(string prefix) => 
            false;

        public override bool MoveToNext() => 
            false;

        public override bool MoveToNextAttribute() => 
            false;

        public override bool MoveToNextNamespace(XPathNamespaceScope scope) => 
            false;

        public override bool MoveToParent() => 
            false;

        public override bool MoveToPrevious() => 
            false;

        public override void MoveToRoot()
        {
        }

        public override string BaseURI =>
            string.Empty;

        public override bool HasAttributes =>
            false;

        public override bool HasChildren =>
            false;

        public override bool IsEmptyElement =>
            false;

        public override string LocalName =>
            string.Empty;

        public override string Name =>
            string.Empty;

        public override string NamespaceURI =>
            string.Empty;

        public override XmlNameTable NameTable =>
            new System.Xml.NameTable();

        public override XPathNodeType NodeType =>
            XPathNodeType.All;

        public override string Prefix =>
            string.Empty;

        public static XmlEmptyNavigator Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new XmlEmptyNavigator();
                }
                return singleton;
            }
        }

        public override string Value =>
            string.Empty;

        public override string XmlLang =>
            string.Empty;
    }
}

