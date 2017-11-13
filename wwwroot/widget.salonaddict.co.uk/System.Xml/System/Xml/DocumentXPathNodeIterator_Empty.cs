namespace System.Xml
{
    using System;
    using System.Xml.XPath;

    internal sealed class DocumentXPathNodeIterator_Empty : XPathNodeIterator
    {
        private XPathNavigator nav;

        internal DocumentXPathNodeIterator_Empty(DocumentXPathNavigator nav)
        {
            this.nav = nav.Clone();
        }

        internal DocumentXPathNodeIterator_Empty(DocumentXPathNodeIterator_Empty other)
        {
            this.nav = other.nav.Clone();
        }

        public override XPathNodeIterator Clone() => 
            new DocumentXPathNodeIterator_Empty(this);

        public override bool MoveNext() => 
            false;

        public override int Count =>
            0;

        public override XPathNavigator Current =>
            this.nav;

        public override int CurrentPosition =>
            0;
    }
}

