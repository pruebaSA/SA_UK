namespace System.ServiceModel.Dispatcher
{
    using System;

    internal class XPathStringExpr : XPathLiteralExpr
    {
        private string literal;

        internal XPathStringExpr(string literal) : base(XPathExprType.String, ValueDataType.String)
        {
            this.literal = literal;
        }

        internal override object Literal =>
            this.literal;

        internal string String =>
            this.literal;
    }
}

