namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public class HeaderFooter : DocumentObject, IVisitable
    {
        [DV]
        internal NString comment;
        [DV(ItemType=typeof(DocumentObject))]
        internal DocumentElements elements;
        [DV]
        internal ParagraphFormat format;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString style;

        public HeaderFooter()
        {
            this.style = NString.NullValue;
            this.comment = NString.NullValue;
        }

        internal HeaderFooter(DocumentObject parent) : base(parent)
        {
            this.style = NString.NullValue;
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

        public Paragraph AddParagraph() => 
            this.Elements.AddParagraph();

        public Paragraph AddParagraph(string paragraphText) => 
            this.Elements.AddParagraph(paragraphText);

        public Table AddTable() => 
            this.Elements.AddTable();

        public TextFrame AddTextFrame() => 
            this.Elements.AddTextFrame();

        public HeaderFooter Clone() => 
            ((HeaderFooter) this.DeepCopy());

        protected override object DeepCopy()
        {
            HeaderFooter footer = (HeaderFooter) base.DeepCopy();
            if (footer.format != null)
            {
                footer.format = footer.format.Clone();
                footer.format.parent = footer;
            }
            if (footer.elements != null)
            {
                footer.elements = footer.elements.Clone();
                footer.elements.parent = footer;
            }
            return footer;
        }

        public override bool IsNull() => 
            false;

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitHeaderFooter(this);
            if (visitChildren && (this.elements != null))
            {
                ((IVisitable) this.elements).AcceptVisitor(visitor, visitChildren);
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            HeadersFooters parent = base.parent as HeadersFooters;
            if (parent.Primary == this)
            {
                this.Serialize(serializer, "primary");
            }
            else if (parent.EvenPage == this)
            {
                this.Serialize(serializer, "evenpage");
            }
            else if (parent.FirstPage == this)
            {
                this.Serialize(serializer, "firstpage");
            }
        }

        internal void Serialize(Serializer serializer, string prefix)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine(@"\" + prefix + (this.IsHeader ? "header" : "footer"));
            int pos = serializer.BeginAttributes();
            if (!this.IsNull("Format"))
            {
                this.format.Serialize(serializer, "Format", null);
            }
            serializer.EndAttributes(pos);
            serializer.BeginContent();
            if (!this.IsNull("Elements"))
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

        public ParagraphFormat Format
        {
            get
            {
                if (this.format == null)
                {
                    this.format = new ParagraphFormat(this);
                }
                return this.format;
            }
            set
            {
                base.SetParent(value);
                this.format = value;
            }
        }

        public bool IsEvenPage =>
            (((HeadersFooters) base.parent).evenPage == this);

        public bool IsFirstPage =>
            (((HeadersFooters) base.parent).firstPage == this);

        public bool IsFooter =>
            ((HeadersFooters) base.parent).IsFooter;

        public bool IsHeader =>
            ((HeadersFooters) base.parent).IsHeader;

        public bool IsPrimary =>
            (((HeadersFooters) base.parent).primary == this);

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(HeaderFooter));
                }
                return meta;
            }
        }

        public string Style
        {
            get => 
                this.style.Value;
            set
            {
                if (base.Document.Styles[value] == null)
                {
                    throw new ArgumentException("Invalid style name '" + value + "'.");
                }
                this.style.Value = value;
            }
        }
    }
}

