namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml.Schema;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.XPath;

    internal class XPathPatternBuilder : XPathPatternParser.IPatternBuilder, IXPathBuilder<QilNode>
    {
        private IXPathEnvironment environment;
        private XPathQilFactory f;
        private QilNode fixupNode;
        private bool inTheBuild;
        private XPathBuilder predicateBuilder;
        private XPathPredicateEnvironment predicateEnvironment;

        public XPathPatternBuilder(IXPathEnvironment environment)
        {
            this.environment = environment;
            this.f = environment.Factory;
            this.predicateEnvironment = new XPathPredicateEnvironment(environment);
            this.predicateBuilder = new XPathBuilder(this.predicateEnvironment);
            this.fixupNode = this.f.Unknown(XmlQueryTypeFactory.NodeNotRtfS);
        }

        [Conditional("DEBUG")]
        public void AssertFilter(QilLoop filter)
        {
        }

        public QilNode Axis(XPathAxis xpathAxis, XPathNodeType nodeType, string prefix, string name)
        {
            QilLoop loop;
            double num;
            XPathAxis axis = xpathAxis;
            if (axis == XPathAxis.DescendantOrSelf)
            {
                return this.f.Nop(this.fixupNode);
            }
            else if (axis == XPathAxis.Root)
            {
                QilIterator iterator;
                loop = this.f.BaseFactory.Filter(iterator = this.f.For(this.fixupNode), this.f.IsType(iterator, XmlQueryTypeFactory.Document));
                num = 0.5;
            }
            else
            {
                string nsUri = (prefix == null) ? null : this.environment.ResolvePrefix(prefix);
                loop = BuildAxisFilter(this.f, this.f.For(this.fixupNode), xpathAxis, nodeType, name, nsUri);
                switch (nodeType)
                {
                    case XPathNodeType.Element:
                    case XPathNodeType.Attribute:
                        if (name == null)
                        {
                            if (prefix != null)
                            {
                                num = -0.25;
                            }
                            else
                            {
                                num = -0.5;
                            }
                        }
                        else
                        {
                            num = 0.0;
                        }
                        goto Label_0106;

                    case XPathNodeType.ProcessingInstruction:
                        num = (name != null) ? 0.0 : -0.5;
                        goto Label_0106;
                }
                num = -0.5;
            }
        Label_0106:
            SetPriority(loop, num);
            SetLastParent(loop, loop);
            return loop;
        }

        private static QilLoop BuildAxisFilter(QilPatternFactory f, QilIterator itr, XPathAxis xpathAxis, XPathNodeType nodeType, string name, string nsUri)
        {
            QilNode right = ((name != null) && (nsUri != null)) ? f.Eq(f.NameOf(itr), f.QName(name, nsUri)) : ((nsUri != null) ? f.Eq(f.NamespaceUriOf(itr), f.String(nsUri)) : ((name != null) ? f.Eq(f.LocalNameOf(itr), f.String(name)) : f.True()));
            XmlNodeKindFlags kinds = XPathBuilder.AxisTypeMask(itr.XmlType.NodeKinds, nodeType, xpathAxis);
            QilNode left = (kinds == XmlNodeKindFlags.None) ? f.False() : ((kinds == itr.XmlType.NodeKinds) ? f.True() : f.IsType(itr, XmlQueryTypeFactory.NodeChoice(kinds)));
            QilLoop loop = f.BaseFactory.Filter(itr, f.And(left, right));
            loop.XmlType = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.NodeChoice(kinds), loop.XmlType.Cardinality);
            return loop;
        }

        public static void CleanAnnotation(QilNode node)
        {
            node.Annotation = null;
        }

        public virtual QilNode EndBuild(QilNode result)
        {
            this.inTheBuild = false;
            return result;
        }

        private void FixupFilterBinding(QilLoop filter, QilNode newBinding)
        {
            filter.Variable.Binding = newBinding;
        }

        public QilNode Function(string prefix, string name, IList<QilNode> args)
        {
            QilNode node;
            QilIterator iterator2;
            QilIterator context = this.f.For(this.fixupNode);
            if (name == "id")
            {
                node = this.f.Id(context, args[0]);
            }
            else
            {
                node = this.environment.ResolveFunction(prefix, name, args, new XsltFunctionFocus(context));
            }
            QilLoop loop = this.f.BaseFactory.Filter(context, this.f.Not(this.f.IsEmpty(this.f.Filter(iterator2 = this.f.For(node), this.f.Is(iterator2, context)))));
            SetPriority(loop, 0.5);
            SetLastParent(loop, loop);
            return loop;
        }

        private static QilLoop GetLastParent(QilNode node) => 
            ((Annotation) node.Annotation).Parent;

        public IXPathBuilder<QilNode> GetPredicateBuilder(QilNode ctx)
        {
            QilLoop filter = (QilLoop) ctx;
            this.predicateEnvironment.SetContext(filter);
            return this.predicateBuilder;
        }

        public static double GetPriority(QilNode node) => 
            ((Annotation) node.Annotation).Priority;

        public QilNode JoinStep(QilNode left, QilNode right)
        {
            if (left.NodeType == QilNodeType.Nop)
            {
                QilUnary unary = (QilUnary) left;
                unary.Child = right;
                return unary;
            }
            CleanAnnotation(left);
            QilLoop filter = (QilLoop) left;
            bool flag = false;
            if (right.NodeType == QilNodeType.Nop)
            {
                flag = true;
                QilUnary unary2 = (QilUnary) right;
                right = unary2.Child;
            }
            QilLoop lastParent = GetLastParent(right);
            this.FixupFilterBinding(filter, flag ? this.f.Ancestor(lastParent.Variable) : this.f.Parent(lastParent.Variable));
            lastParent.Body = this.f.And(lastParent.Body, this.f.Not(this.f.IsEmpty(filter)));
            SetPriority(right, 0.5);
            SetLastParent(right, filter);
            return right;
        }

        public QilNode Number(double value) => 
            this.UnexpectedToken("Literal number");

        public QilNode Operator(XPathOperator op, QilNode left, QilNode right)
        {
            if (left.NodeType == QilNodeType.Sequence)
            {
                ((QilList) left).Add(right);
                return left;
            }
            return this.f.Sequence(left, right);
        }

        public QilNode Predicate(QilNode node, QilNode condition, bool isReverseStep)
        {
            QilLoop filter = (QilLoop) node;
            if (condition.XmlType.TypeCode == XmlTypeCode.Double)
            {
                this.predicateEnvironment.SetContext(filter);
                condition = this.f.Eq(condition, this.predicateEnvironment.GetPosition());
            }
            else
            {
                condition = this.f.ConvertToBoolean(condition);
            }
            filter.Body = this.f.And(filter.Body, condition);
            SetPriority(node, 0.5);
            return node;
        }

        private static void SetLastParent(QilNode node, QilLoop parent)
        {
            Annotation annotation = ((Annotation) node.Annotation) ?? new Annotation();
            annotation.Parent = parent;
            node.Annotation = annotation;
        }

        public static void SetPriority(QilNode node, double priority)
        {
            Annotation annotation = ((Annotation) node.Annotation) ?? new Annotation();
            annotation.Priority = priority;
            node.Annotation = annotation;
        }

        public virtual void StartBuild()
        {
            this.inTheBuild = true;
        }

        public QilNode String(string value) => 
            this.f.String(value);

        private QilNode UnexpectedToken(string tokenName)
        {
            throw new Exception(string.Format(CultureInfo.InvariantCulture, "Internal Error: {0} is not allowed in XSLT pattern outside of predicate.", new object[] { tokenName }));
        }

        public QilNode Variable(string prefix, string name) => 
            this.UnexpectedToken("Variable");

        public QilNode FixupNode =>
            this.fixupNode;

        private class Annotation
        {
            public QilLoop Parent;
            public double Priority;
        }

        private class XPathPredicateEnvironment : IXPathEnvironment, IFocus
        {
            private QilLoop baseContext;
            private IXPathEnvironment baseEnvironment;
            private Cloner cloner;
            private XPathQilFactory f;

            public XPathPredicateEnvironment(IXPathEnvironment baseEnvironment)
            {
                this.baseEnvironment = baseEnvironment;
                this.f = baseEnvironment.Factory;
                this.cloner = new Cloner(this.f.BaseFactory);
            }

            public QilNode GetCurrent() => 
                this.baseContext.Variable;

            public QilNode GetLast()
            {
                QilLoop body = (QilLoop) this.cloner.Clone(this.baseContext);
                QilIterator context = this.f.For(this.f.Parent(this.GetCurrent()));
                body.Variable.Binding = this.f.Content(context);
                return this.f.XsltConvert(this.f.Length(this.f.Loop(context, body)), XmlQueryTypeFactory.DoubleX);
            }

            public QilNode GetPosition()
            {
                QilLoop body = (QilLoop) this.cloner.Clone(this.baseContext);
                if (this.baseContext.XmlType.NodeKinds == XmlNodeKindFlags.Attribute)
                {
                    QilIterator context = this.f.For(this.f.Parent(this.GetCurrent()));
                    body.Variable.Binding = this.f.Content(context);
                    body.Body = this.f.And(body.Body, this.f.Before(body.Variable, this.GetCurrent()));
                    body = this.f.BaseFactory.Loop(context, body);
                }
                else
                {
                    body.Variable.Binding = this.f.PrecedingSibling(this.GetCurrent());
                }
                return this.f.Add(this.f.Double(1.0), this.f.XsltConvert(this.f.Length(body), XmlQueryTypeFactory.DoubleX));
            }

            public QilNode ResolveFunction(string prefix, string name, IList<QilNode> args, IFocus env) => 
                this.baseEnvironment.ResolveFunction(prefix, name, args, env);

            public string ResolvePrefix(string prefix) => 
                this.baseEnvironment.ResolvePrefix(prefix);

            public QilNode ResolveVariable(string prefix, string name) => 
                this.baseEnvironment.ResolveVariable(prefix, name);

            public void SetContext(QilLoop filter)
            {
                this.baseContext = filter;
            }

            public XPathQilFactory Factory =>
                this.f;

            internal class Cloner : QilCloneVisitor
            {
                public Cloner(QilFactory f) : base(f)
                {
                }

                protected override QilNode VisitUnknown(QilNode n) => 
                    n;
            }
        }

        private class XsltFunctionFocus : IFocus
        {
            private QilIterator current;

            public XsltFunctionFocus(QilIterator current)
            {
                this.current = current;
            }

            public QilNode GetCurrent() => 
                this.current;

            public QilNode GetLast() => 
                null;

            public QilNode GetPosition() => 
                null;
        }
    }
}

