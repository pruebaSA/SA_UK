namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Xml.XPath;

    internal class ContextQuery : Query
    {
        protected XPathNavigator contextNode;

        public ContextQuery()
        {
            base.count = 0;
        }

        protected ContextQuery(ContextQuery other) : base(other)
        {
            this.contextNode = other.contextNode;
        }

        public override XPathNavigator Advance()
        {
            if (base.count == 0)
            {
                base.count = 1;
                return this.contextNode;
            }
            return null;
        }

        public override XPathNodeIterator Clone() => 
            new ContextQuery(this);

        public override object Evaluate(XPathNodeIterator context)
        {
            this.contextNode = context.Current;
            base.count = 0;
            return this;
        }

        public override XPathNavigator MatchNode(XPathNavigator current) => 
            current;

        public override void Reset()
        {
            base.count = 0;
        }

        public override int Count =>
            1;

        public override XPathNavigator Current =>
            this.contextNode;

        public override int CurrentPosition =>
            base.count;

        public override QueryProps Properties =>
            (QueryProps.Merge | QueryProps.Cached | QueryProps.Count | QueryProps.Position);

        public override XPathResultType StaticType =>
            XPathResultType.NodeSet;
    }
}

