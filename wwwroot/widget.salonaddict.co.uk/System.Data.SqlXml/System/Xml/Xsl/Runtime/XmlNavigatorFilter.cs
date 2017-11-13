namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Xml.XPath;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class XmlNavigatorFilter
    {
        protected XmlNavigatorFilter()
        {
        }

        public abstract bool IsFiltered(XPathNavigator navigator);
        public abstract bool MoveToContent(XPathNavigator navigator);
        public abstract bool MoveToFollowing(XPathNavigator navigator, XPathNavigator navigatorEnd);
        public abstract bool MoveToFollowingSibling(XPathNavigator navigator);
        public abstract bool MoveToNextContent(XPathNavigator navigator);
        public abstract bool MoveToPreviousSibling(XPathNavigator navigator);
    }
}

