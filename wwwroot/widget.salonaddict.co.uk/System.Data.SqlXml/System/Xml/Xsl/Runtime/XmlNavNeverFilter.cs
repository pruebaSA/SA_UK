namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml.XPath;

    internal class XmlNavNeverFilter : XmlNavigatorFilter
    {
        private static XmlNavigatorFilter Singleton = new XmlNavNeverFilter();

        private XmlNavNeverFilter()
        {
        }

        public static XmlNavigatorFilter Create() => 
            Singleton;

        public override bool IsFiltered(XPathNavigator navigator) => 
            false;

        public override bool MoveToContent(XPathNavigator navigator) => 
            MoveToFirstAttributeContent(navigator);

        public static bool MoveToFirstAttributeContent(XPathNavigator navigator)
        {
            if (!navigator.MoveToFirstAttribute())
            {
                return navigator.MoveToFirstChild();
            }
            return true;
        }

        public override bool MoveToFollowing(XPathNavigator navigator, XPathNavigator navEnd) => 
            navigator.MoveToFollowing(XPathNodeType.All, navEnd);

        public override bool MoveToFollowingSibling(XPathNavigator navigator) => 
            navigator.MoveToNext();

        public static bool MoveToNextAttributeContent(XPathNavigator navigator)
        {
            if (navigator.NodeType != XPathNodeType.Attribute)
            {
                return navigator.MoveToNext();
            }
            if (!navigator.MoveToNextAttribute())
            {
                navigator.MoveToParent();
                if (!navigator.MoveToFirstChild())
                {
                    navigator.MoveToFirstAttribute();
                    while (navigator.MoveToNextAttribute())
                    {
                    }
                    return false;
                }
            }
            return true;
        }

        public override bool MoveToNextContent(XPathNavigator navigator) => 
            MoveToNextAttributeContent(navigator);

        public override bool MoveToPreviousSibling(XPathNavigator navigator) => 
            navigator.MoveToPrevious();
    }
}

