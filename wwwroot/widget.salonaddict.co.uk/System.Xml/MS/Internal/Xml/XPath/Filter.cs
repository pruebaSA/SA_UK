namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Xml.XPath;

    internal class Filter : AstNode
    {
        private AstNode condition;
        private AstNode input;

        public Filter(AstNode input, AstNode condition)
        {
            this.input = input;
            this.condition = condition;
        }

        public AstNode Condition =>
            this.condition;

        public AstNode Input =>
            this.input;

        public override XPathResultType ReturnType =>
            XPathResultType.NodeSet;

        public override AstNode.AstType Type =>
            AstNode.AstType.Filter;
    }
}

