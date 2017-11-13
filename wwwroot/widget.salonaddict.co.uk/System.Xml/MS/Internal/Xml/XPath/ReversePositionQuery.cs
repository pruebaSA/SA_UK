namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Xml.XPath;

    internal sealed class ReversePositionQuery : ForwardPositionQuery
    {
        public ReversePositionQuery(Query input) : base(input)
        {
        }

        private ReversePositionQuery(ReversePositionQuery other) : base((ForwardPositionQuery) other)
        {
        }

        public override XPathNodeIterator Clone() => 
            new ReversePositionQuery(this);

        public override int CurrentPosition =>
            ((base.outputBuffer.Count - base.count) + 1);

        public override QueryProps Properties =>
            (base.Properties | QueryProps.Reverse);
    }
}

