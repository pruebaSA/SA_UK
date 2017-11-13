namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml.XPath;

    internal class XmlNavTypeFilter : XmlNavigatorFilter
    {
        private int mask;
        private XPathNodeType nodeType;
        private static XmlNavigatorFilter[] TypeFilters = new XmlNavigatorFilter[9];

        static XmlNavTypeFilter()
        {
            TypeFilters[1] = new XmlNavTypeFilter(XPathNodeType.Element);
            TypeFilters[4] = new XmlNavTypeFilter(XPathNodeType.Text);
            TypeFilters[7] = new XmlNavTypeFilter(XPathNodeType.ProcessingInstruction);
            TypeFilters[8] = new XmlNavTypeFilter(XPathNodeType.Comment);
        }

        private XmlNavTypeFilter(XPathNodeType nodeType)
        {
            this.nodeType = nodeType;
            this.mask = XPathNavigator.GetContentKindMask(nodeType);
        }

        public static XmlNavigatorFilter Create(XPathNodeType nodeType) => 
            TypeFilters[(int) nodeType];

        public override bool IsFiltered(XPathNavigator navigator) => 
            (((((int) 1) << navigator.NodeType) & this.mask) == 0);

        public override bool MoveToContent(XPathNavigator navigator) => 
            navigator.MoveToChild(this.nodeType);

        public override bool MoveToFollowing(XPathNavigator navigator, XPathNavigator navEnd) => 
            navigator.MoveToFollowing(this.nodeType, navEnd);

        public override bool MoveToFollowingSibling(XPathNavigator navigator) => 
            navigator.MoveToNext(this.nodeType);

        public override bool MoveToNextContent(XPathNavigator navigator) => 
            navigator.MoveToNext(this.nodeType);

        public override bool MoveToPreviousSibling(XPathNavigator navigator) => 
            navigator.MoveToPrevious(this.nodeType);
    }
}

