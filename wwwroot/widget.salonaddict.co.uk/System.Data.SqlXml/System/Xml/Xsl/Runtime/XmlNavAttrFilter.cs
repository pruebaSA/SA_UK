namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml.XPath;

    internal class XmlNavAttrFilter : XmlNavigatorFilter
    {
        private static XmlNavigatorFilter Singleton = new XmlNavAttrFilter();

        private XmlNavAttrFilter()
        {
        }

        public static XmlNavigatorFilter Create() => 
            Singleton;

        public override bool IsFiltered(XPathNavigator navigator) => 
            (navigator.NodeType == XPathNodeType.Attribute);

        public override bool MoveToContent(XPathNavigator navigator) => 
            navigator.MoveToFirstChild();

        public override bool MoveToFollowing(XPathNavigator navigator, XPathNavigator navEnd) => 
            navigator.MoveToFollowing(XPathNodeType.All, navEnd);

        public override bool MoveToFollowingSibling(XPathNavigator navigator) => 
            navigator.MoveToNext();

        public override bool MoveToNextContent(XPathNavigator navigator) => 
            navigator.MoveToNext();

        public override bool MoveToPreviousSibling(XPathNavigator navigator) => 
            navigator.MoveToPrevious();
    }
}

