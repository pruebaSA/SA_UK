namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml.XPath;

    internal class XmlNavNameFilter : XmlNavigatorFilter
    {
        private string localName;
        private string namespaceUri;

        private XmlNavNameFilter(string localName, string namespaceUri)
        {
            this.localName = localName;
            this.namespaceUri = namespaceUri;
        }

        public static XmlNavigatorFilter Create(string localName, string namespaceUri) => 
            new XmlNavNameFilter(localName, namespaceUri);

        public override bool IsFiltered(XPathNavigator navigator)
        {
            if (navigator.LocalName == this.localName)
            {
                return (navigator.NamespaceURI != this.namespaceUri);
            }
            return true;
        }

        public override bool MoveToContent(XPathNavigator navigator) => 
            navigator.MoveToChild(this.localName, this.namespaceUri);

        public override bool MoveToFollowing(XPathNavigator navigator, XPathNavigator navEnd) => 
            navigator.MoveToFollowing(this.localName, this.namespaceUri, navEnd);

        public override bool MoveToFollowingSibling(XPathNavigator navigator) => 
            navigator.MoveToNext(this.localName, this.namespaceUri);

        public override bool MoveToNextContent(XPathNavigator navigator) => 
            navigator.MoveToNext(this.localName, this.namespaceUri);

        public override bool MoveToPreviousSibling(XPathNavigator navigator) => 
            navigator.MoveToPrevious(this.localName, this.namespaceUri);
    }
}

