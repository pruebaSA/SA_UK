namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.XPath;

    internal class KeyMatchBuilder : XPathBuilder, XPathPatternParser.IPatternBuilder, IXPathBuilder<QilNode>
    {
        private PathConvertor convertor;
        private int depth;

        public KeyMatchBuilder(IXPathEnvironment env) : base(env)
        {
            this.convertor = new PathConvertor(env.Factory);
        }

        public override QilNode EndBuild(QilNode result)
        {
            this.depth--;
            if (result == null)
            {
                return base.EndBuild(result);
            }
            if (this.depth == 0)
            {
                result = this.convertor.ConvertReletive2Absolute(result, base.fixupCurrent);
                result = base.EndBuild(result);
            }
            return result;
        }

        public virtual IXPathBuilder<QilNode> GetPredicateBuilder(QilNode ctx) => 
            this;

        public override void StartBuild()
        {
            if (this.depth == 0)
            {
                base.StartBuild();
            }
            this.depth++;
        }

        internal class PathConvertor : QilReplaceVisitor
        {
            private XPathQilFactory f;
            private QilNode fixup;

            public PathConvertor(XPathQilFactory f) : base(f.BaseFactory)
            {
                this.f = f;
            }

            public QilNode ConvertReletive2Absolute(QilNode node, QilNode fixup)
            {
                QilDepthChecker.Check(node);
                this.fixup = fixup;
                return this.Visit(node);
            }

            protected override QilNode Visit(QilNode n)
            {
                if (((n.NodeType != QilNodeType.Union) && (n.NodeType != QilNodeType.DocOrderDistinct)) && ((n.NodeType != QilNodeType.Filter) && (n.NodeType != QilNodeType.Loop)))
                {
                    return n;
                }
                return base.Visit(n);
            }

            protected override QilNode VisitFilter(QilLoop n) => 
                this.VisitLoop(n);

            protected override QilNode VisitLoop(QilLoop n)
            {
                if ((n.Variable.Binding.NodeType != QilNodeType.Root) && (n.Variable.Binding.NodeType != QilNodeType.Deref))
                {
                    if (n.Variable.Binding.NodeType == QilNodeType.Content)
                    {
                        QilUnary binding = (QilUnary) n.Variable.Binding;
                        QilIterator variable = this.f.For(this.f.DescendantOrSelf(this.f.Root(this.fixup)));
                        binding.Child = variable;
                        n.Variable.Binding = this.f.Loop(variable, binding);
                        return n;
                    }
                    n.Variable.Binding = this.Visit(n.Variable.Binding);
                }
                return n;
            }
        }
    }
}

