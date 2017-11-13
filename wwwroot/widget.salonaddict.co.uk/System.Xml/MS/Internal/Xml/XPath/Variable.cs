namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Xml.XPath;

    internal class Variable : AstNode
    {
        private string localname;
        private string prefix;

        public Variable(string name, string prefix)
        {
            this.localname = name;
            this.prefix = prefix;
        }

        public string Localname =>
            this.localname;

        public string Prefix =>
            this.prefix;

        public override XPathResultType ReturnType =>
            XPathResultType.Any;

        public override AstNode.AstType Type =>
            AstNode.AstType.Variable;
    }
}

