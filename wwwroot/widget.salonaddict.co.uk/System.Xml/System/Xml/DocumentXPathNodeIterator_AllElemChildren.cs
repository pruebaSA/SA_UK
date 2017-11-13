﻿namespace System.Xml
{
    using System;
    using System.Xml.XPath;

    internal class DocumentXPathNodeIterator_AllElemChildren : DocumentXPathNodeIterator_ElemDescendants
    {
        internal DocumentXPathNodeIterator_AllElemChildren(DocumentXPathNavigator nav) : base(nav)
        {
        }

        internal DocumentXPathNodeIterator_AllElemChildren(DocumentXPathNodeIterator_AllElemChildren other) : base(other)
        {
        }

        public override XPathNodeIterator Clone() => 
            new DocumentXPathNodeIterator_AllElemChildren(this);

        protected override bool Match(XmlNode node) => 
            (node.NodeType == XmlNodeType.Element);
    }
}

