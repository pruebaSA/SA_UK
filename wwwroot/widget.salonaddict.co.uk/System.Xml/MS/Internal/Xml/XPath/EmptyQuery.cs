namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Xml.XPath;

    internal sealed class EmptyQuery : Query
    {
        public override XPathNavigator Advance() => 
            null;

        public override XPathNodeIterator Clone() => 
            this;

        public override object Evaluate(XPathNodeIterator context) => 
            this;

        public override void Reset()
        {
        }

        public override int Count =>
            0;

        public override XPathNavigator Current =>
            null;

        public override int CurrentPosition =>
            0;

        public override QueryProps Properties =>
            (QueryProps.Merge | QueryProps.Cached | QueryProps.Count | QueryProps.Position);

        public override XPathResultType StaticType =>
            XPathResultType.NodeSet;
    }
}

