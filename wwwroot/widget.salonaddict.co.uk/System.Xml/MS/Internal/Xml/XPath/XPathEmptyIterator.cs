namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Xml.XPath;

    internal sealed class XPathEmptyIterator : ResetableIterator
    {
        public static XPathEmptyIterator Instance = new XPathEmptyIterator();

        private XPathEmptyIterator()
        {
        }

        public override XPathNodeIterator Clone() => 
            this;

        public override bool MoveNext() => 
            false;

        public override void Reset()
        {
        }

        public override int Count =>
            0;

        public override XPathNavigator Current =>
            null;

        public override int CurrentPosition =>
            0;
    }
}

