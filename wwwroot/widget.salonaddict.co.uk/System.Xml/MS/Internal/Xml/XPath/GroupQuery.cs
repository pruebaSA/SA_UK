namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Xml.XPath;

    internal sealed class GroupQuery : BaseAxisQuery
    {
        private GroupQuery(GroupQuery other) : base((BaseAxisQuery) other)
        {
        }

        public GroupQuery(Query qy) : base(qy)
        {
        }

        public override XPathNavigator Advance()
        {
            base.currentNode = base.qyInput.Advance();
            if (base.currentNode != null)
            {
                base.position++;
            }
            return base.currentNode;
        }

        public override XPathNodeIterator Clone() => 
            new GroupQuery(this);

        public override object Evaluate(XPathNodeIterator nodeIterator) => 
            base.qyInput.Evaluate(nodeIterator);

        public override QueryProps Properties =>
            QueryProps.Position;

        public override XPathResultType StaticType =>
            base.qyInput.StaticType;
    }
}

