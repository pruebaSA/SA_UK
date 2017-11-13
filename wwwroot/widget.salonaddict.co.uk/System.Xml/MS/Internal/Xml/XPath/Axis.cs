namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Xml.XPath;

    internal class Axis : AstNode
    {
        protected bool abbrAxis;
        private AxisType axisType;
        private AstNode input;
        private string name;
        private XPathNodeType nodeType;
        private string prefix;
        private string urn;

        public Axis(AxisType axisType, AstNode input) : this(axisType, input, string.Empty, string.Empty, XPathNodeType.All)
        {
            this.abbrAxis = true;
        }

        public Axis(AxisType axisType, AstNode input, string prefix, string name, XPathNodeType nodetype)
        {
            this.urn = string.Empty;
            this.axisType = axisType;
            this.input = input;
            this.prefix = prefix;
            this.name = name;
            this.nodeType = nodetype;
        }

        public bool AbbrAxis =>
            this.abbrAxis;

        public AstNode Input
        {
            get => 
                this.input;
            set
            {
                this.input = value;
            }
        }

        public string Name =>
            this.name;

        public XPathNodeType NodeType =>
            this.nodeType;

        public string Prefix =>
            this.prefix;

        public override XPathResultType ReturnType =>
            XPathResultType.NodeSet;

        public override AstNode.AstType Type =>
            AstNode.AstType.Axis;

        public AxisType TypeOfAxis =>
            this.axisType;

        public string Urn
        {
            get => 
                this.urn;
            set
            {
                this.urn = value;
            }
        }

        public enum AxisType
        {
            Ancestor,
            AncestorOrSelf,
            Attribute,
            Child,
            Descendant,
            DescendantOrSelf,
            Following,
            FollowingSibling,
            Namespace,
            Parent,
            Preceding,
            PrecedingSibling,
            Self,
            None
        }
    }
}

