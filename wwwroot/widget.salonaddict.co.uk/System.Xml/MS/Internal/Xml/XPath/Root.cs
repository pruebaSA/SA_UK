namespace MS.Internal.Xml.XPath
{
    using System.Xml.XPath;

    internal class Root : AstNode
    {
        public override XPathResultType ReturnType =>
            XPathResultType.NodeSet;

        public override AstNode.AstType Type =>
            AstNode.AstType.Root;
    }
}

