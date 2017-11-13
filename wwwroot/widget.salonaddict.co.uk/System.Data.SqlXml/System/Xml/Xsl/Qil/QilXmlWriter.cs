namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Xml;

    internal class QilXmlWriter : QilScopedVisitor
    {
        private NameGenerator ngen;
        protected Options options;
        protected XmlWriter writer;

        public QilXmlWriter(XmlWriter writer) : this(writer, Options.NodeLocation | Options.NodeIdentity | Options.LineInfo | Options.TypeInfo | Options.Annotations)
        {
        }

        public QilXmlWriter(XmlWriter writer, Options options)
        {
            this.writer = writer;
            this.ngen = new NameGenerator();
            this.options = options;
        }

        protected override void AfterVisit(QilNode node)
        {
            this.writer.WriteEndElement();
            base.AfterVisit(node);
        }

        protected override void BeforeVisit(QilNode node)
        {
            base.BeforeVisit(node);
            if ((this.options & Options.Annotations) != Options.None)
            {
                this.WriteAnnotations(node.Annotation);
            }
            this.writer.WriteStartElement("", Enum.GetName(typeof(QilNodeType), node.NodeType), "");
            if ((this.options & (Options.RoundTripTypeInfo | Options.TypeInfo)) != Options.None)
            {
                this.WriteXmlType(node);
            }
            if (((this.options & Options.LineInfo) != Options.None) && (node.SourceLine != null))
            {
                this.WriteLineInfo(node);
            }
        }

        protected override void BeginScope(QilNode node)
        {
            this.ngen.NameOf(node);
        }

        protected override void EndScope(QilNode node)
        {
            this.ngen.ClearName(node);
        }

        public void ToXml(QilNode node)
        {
            this.VisitAssumeReference(node);
        }

        protected override QilNode VisitChildren(QilNode node)
        {
            if (node is QilLiteral)
            {
                this.writer.WriteValue(Convert.ToString(((QilLiteral) node).Value, CultureInfo.InvariantCulture));
                return node;
            }
            if (node is QilReference)
            {
                QilReference reference = (QilReference) node;
                this.writer.WriteAttributeString("id", this.ngen.NameOf(node));
                if (reference.DebugName != null)
                {
                    this.writer.WriteAttributeString("name", reference.DebugName.ToString());
                }
                if (node.NodeType == QilNodeType.Parameter)
                {
                    QilParameter parameter = (QilParameter) node;
                    if (parameter.DefaultValue != null)
                    {
                        this.Visit(parameter.DefaultValue);
                    }
                    return node;
                }
            }
            return base.VisitChildren(node);
        }

        protected override QilNode VisitLiteralQName(QilName value)
        {
            this.writer.WriteAttributeString("name", value.ToString());
            return value;
        }

        protected override QilNode VisitLiteralType(QilLiteral value)
        {
            this.writer.WriteString(value.ToString(((this.options & Options.TypeInfo) != Options.None) ? "G" : "S"));
            return value;
        }

        protected override QilNode VisitQilExpression(QilExpression qil)
        {
            IList<QilNode> list = new ForwardRefFinder().Find(qil);
            if ((list != null) && (list.Count > 0))
            {
                this.writer.WriteStartElement("ForwardDecls");
                foreach (QilNode node in list)
                {
                    this.writer.WriteStartElement(Enum.GetName(typeof(QilNodeType), node.NodeType));
                    this.writer.WriteAttributeString("id", this.ngen.NameOf(node));
                    this.WriteXmlType(node);
                    if (node.NodeType == QilNodeType.Function)
                    {
                        this.Visit(node[0]);
                        this.Visit(node[2]);
                    }
                    this.writer.WriteEndElement();
                }
                this.writer.WriteEndElement();
            }
            return this.VisitChildren(qil);
        }

        protected override QilNode VisitReference(QilNode node)
        {
            QilReference reference = (QilReference) node;
            string str = this.ngen.NameOf(node);
            if (str == null)
            {
                str = "OUT-OF-SCOPE REFERENCE";
            }
            this.writer.WriteStartElement("RefTo");
            this.writer.WriteAttributeString("id", str);
            if (reference.DebugName != null)
            {
                this.writer.WriteAttributeString("name", reference.DebugName.ToString());
            }
            this.writer.WriteEndElement();
            return node;
        }

        protected virtual void WriteAnnotations(object ann)
        {
            string str = null;
            string name = null;
            if (ann != null)
            {
                if (ann is string)
                {
                    str = ann as string;
                }
                else if (ann is IQilAnnotation)
                {
                    IQilAnnotation annotation = ann as IQilAnnotation;
                    name = annotation.Name;
                    str = ann.ToString();
                }
                else if (ann is IList<object>)
                {
                    IList<object> list = (IList<object>) ann;
                    foreach (object obj2 in list)
                    {
                        this.WriteAnnotations(obj2);
                    }
                    return;
                }
                if ((str != null) && (str.Length != 0))
                {
                    this.writer.WriteComment(((name != null) && (name.Length != 0)) ? (name + ": " + str) : str);
                }
            }
        }

        protected virtual void WriteLineInfo(QilNode node)
        {
            this.writer.WriteAttributeString("lineInfo", string.Format(CultureInfo.InvariantCulture, "[{0},{1} -- {2},{3}]", new object[] { node.SourceLine.StartLine, node.SourceLine.StartPos, node.SourceLine.EndLine, node.SourceLine.EndPos }));
        }

        protected virtual void WriteXmlType(QilNode node)
        {
            this.writer.WriteAttributeString("xmlType", node.XmlType.ToString(((this.options & Options.RoundTripTypeInfo) != Options.None) ? "S" : "G"));
        }

        internal class ForwardRefFinder : QilVisitor
        {
            private List<QilNode> backrefs = new List<QilNode>();
            private List<QilNode> fwdrefs = new List<QilNode>();

            public IList<QilNode> Find(QilExpression qil)
            {
                this.Visit(qil);
                return this.fwdrefs;
            }

            protected override QilNode Visit(QilNode node)
            {
                if ((node is QilIterator) || (node is QilFunction))
                {
                    this.backrefs.Add(node);
                }
                return base.Visit(node);
            }

            protected override QilNode VisitReference(QilNode node)
            {
                if (!this.backrefs.Contains(node) && !this.fwdrefs.Contains(node))
                {
                    this.fwdrefs.Add(node);
                }
                return node;
            }
        }

        private sealed class NameGenerator
        {
            private char end;
            private int len;
            private StringBuilder name;
            private char start;
            private int zero;

            public NameGenerator()
            {
                string str = "$";
                this.len = this.zero = str.Length;
                this.start = 'a';
                this.end = 'z';
                this.name = new StringBuilder(str, this.len + 2);
                this.name.Append(this.start);
            }

            public void ClearName(QilNode n)
            {
                if (n.Annotation is NameAnnotation)
                {
                    n.Annotation = ((NameAnnotation) n.Annotation).PriorAnnotation;
                }
            }

            public string NameOf(QilNode n)
            {
                string s = null;
                object a = n.Annotation;
                NameAnnotation annotation = a as NameAnnotation;
                if (annotation == null)
                {
                    s = this.NextName();
                    n.Annotation = new NameAnnotation(s, a);
                    return s;
                }
                return annotation.Name;
            }

            public string NextName()
            {
                string str = this.name.ToString();
                char ch = this.name[this.len];
                if (ch == this.end)
                {
                    StringBuilder builder;
                    int num2;
                    this.name[this.len] = this.start;
                    int len = this.len;
                    while ((len-- > this.zero) && (this.name[len] == this.end))
                    {
                        this.name[len] = this.start;
                    }
                    if (len < this.zero)
                    {
                        this.len++;
                        this.name.Append(this.start);
                        return str;
                    }
                    (builder = this.name)[num2 = len] = (char) (builder[num2] + '\x0001');
                    return str;
                }
                this.name[this.len] = ch = (char) (ch + '\x0001');
                return str;
            }

            private class NameAnnotation : ListBase<object>
            {
                public string Name;
                public object PriorAnnotation;

                public NameAnnotation(string s, object a)
                {
                    this.Name = s;
                    this.PriorAnnotation = a;
                }

                public override int Count =>
                    1;

                public override object this[int index]
                {
                    get
                    {
                        if (index != 0)
                        {
                            throw new IndexOutOfRangeException();
                        }
                        return this.PriorAnnotation;
                    }
                    set
                    {
                        throw new NotSupportedException();
                    }
                }
            }
        }

        [Flags]
        public enum Options
        {
            Annotations = 1,
            LineInfo = 8,
            NodeIdentity = 0x10,
            NodeLocation = 0x20,
            None = 0,
            RoundTripTypeInfo = 4,
            TypeInfo = 2
        }
    }
}

