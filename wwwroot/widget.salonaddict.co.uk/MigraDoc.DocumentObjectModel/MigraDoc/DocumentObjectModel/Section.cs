namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public class Section : DocumentObject, IVisitable
    {
        [DV]
        internal NString comment;
        [DV]
        internal DocumentElements elements;
        [DV]
        internal HeadersFooters footers;
        [DV]
        internal HeadersFooters headers;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal MigraDoc.DocumentObjectModel.PageSetup pageSetup;

        public Section()
        {
            this.comment = NString.NullValue;
        }

        internal Section(DocumentObject parent) : base(parent)
        {
            this.comment = NString.NullValue;
        }

        public void Add(Paragraph paragraph)
        {
            this.Elements.Add(paragraph);
        }

        public void Add(Chart chart)
        {
            this.Elements.Add(chart);
        }

        public void Add(Image image)
        {
            this.Elements.Add(image);
        }

        public void Add(TextFrame textFrame)
        {
            this.Elements.Add(textFrame);
        }

        public void Add(Table table)
        {
            this.Elements.Add(table);
        }

        public Chart AddChart() => 
            this.Elements.AddChart();

        public Chart AddChart(ChartType type) => 
            this.Elements.AddChart(type);

        public Image AddImage(string fileName) => 
            this.Elements.AddImage(fileName);

        public void AddPageBreak()
        {
            this.Elements.AddPageBreak();
        }

        public Paragraph AddParagraph() => 
            this.Elements.AddParagraph();

        public Paragraph AddParagraph(string paragraphText) => 
            this.Elements.AddParagraph(paragraphText);

        public Paragraph AddParagraph(string paragraphText, string style) => 
            this.Elements.AddParagraph(paragraphText, style);

        public Table AddTable() => 
            this.Elements.AddTable();

        public TextFrame AddTextFrame() => 
            this.Elements.AddTextFrame();

        public Section Clone() => 
            ((Section) this.DeepCopy());

        protected override object DeepCopy()
        {
            Section section = (Section) base.DeepCopy();
            if (section.pageSetup != null)
            {
                section.pageSetup = section.pageSetup.Clone();
                section.pageSetup.parent = section;
            }
            if (section.headers != null)
            {
                section.headers = section.headers.Clone();
                section.headers.parent = section;
            }
            if (section.footers != null)
            {
                section.footers = section.footers.Clone();
                section.footers.parent = section;
            }
            if (section.elements != null)
            {
                section.elements = section.elements.Clone();
                section.elements.parent = section;
            }
            return section;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitSection(this);
            if (visitChildren && (this.headers != null))
            {
                ((IVisitable) this.headers).AcceptVisitor(visitor, visitChildren);
            }
            if (visitChildren && (this.footers != null))
            {
                ((IVisitable) this.footers).AcceptVisitor(visitor, visitChildren);
            }
            if (visitChildren && (this.elements != null))
            {
                ((IVisitable) this.elements).AcceptVisitor(visitor, visitChildren);
            }
        }

        public Section PreviousSection()
        {
            Sections parent = base.Parent as Sections;
            int index = parent.IndexOf(this);
            if (index > 0)
            {
                return parent[index - 1];
            }
            return null;
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine(@"\section");
            int pos = serializer.BeginAttributes();
            if (!this.IsNull("PageSetup"))
            {
                this.PageSetup.Serialize(serializer);
            }
            serializer.EndAttributes(pos);
            serializer.BeginContent();
            if (!this.IsNull("headers"))
            {
                this.headers.Serialize(serializer);
            }
            if (!this.IsNull("footers"))
            {
                this.footers.Serialize(serializer);
            }
            if (!this.IsNull("elements"))
            {
                this.elements.Serialize(serializer);
            }
            serializer.EndContent();
        }

        public string Comment
        {
            get => 
                this.comment.Value;
            set
            {
                this.comment.Value = value;
            }
        }

        public DocumentElements Elements
        {
            get
            {
                if (this.elements == null)
                {
                    this.elements = new DocumentElements(this);
                }
                return this.elements;
            }
            set
            {
                base.SetParent(value);
                this.elements = value;
            }
        }

        public HeadersFooters Footers
        {
            get
            {
                if (this.footers == null)
                {
                    this.footers = new HeadersFooters(this);
                }
                return this.footers;
            }
            set
            {
                base.SetParent(value);
                this.footers = value;
            }
        }

        public HeadersFooters Headers
        {
            get
            {
                if (this.headers == null)
                {
                    this.headers = new HeadersFooters(this);
                }
                return this.headers;
            }
            set
            {
                base.SetParent(value);
                this.headers = value;
            }
        }

        public Paragraph LastParagraph
        {
            get
            {
                for (int i = this.elements.Count - 1; i >= 0; i--)
                {
                    if (this.elements[i] is Paragraph)
                    {
                        return (Paragraph) this.elements[i];
                    }
                }
                return null;
            }
        }

        public Table LastTable
        {
            get
            {
                for (int i = this.elements.Count - 1; i >= 0; i--)
                {
                    if (this.elements[i] is Table)
                    {
                        return (Table) this.elements[i];
                    }
                }
                return null;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Section));
                }
                return meta;
            }
        }

        public MigraDoc.DocumentObjectModel.PageSetup PageSetup
        {
            get
            {
                if (this.pageSetup == null)
                {
                    this.pageSetup = new MigraDoc.DocumentObjectModel.PageSetup(this);
                }
                return this.pageSetup;
            }
            set
            {
                base.SetParent(value);
                this.pageSetup = value;
            }
        }
    }
}

