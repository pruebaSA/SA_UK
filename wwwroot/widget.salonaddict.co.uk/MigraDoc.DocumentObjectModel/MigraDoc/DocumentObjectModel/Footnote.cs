namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public class Footnote : DocumentObject, IVisitable
    {
        [DV(ItemType=typeof(DocumentObject))]
        internal DocumentElements elements;
        [DV]
        internal ParagraphFormat format;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString reference;
        [DV]
        internal NString style;

        public Footnote()
        {
            this.reference = NString.NullValue;
            this.style = NString.NullValue;
        }

        internal Footnote(DocumentObject parent) : base(parent)
        {
            this.reference = NString.NullValue;
            this.style = NString.NullValue;
        }

        internal Footnote(string content) : this()
        {
            this.Elements.AddParagraph(content);
        }

        public void Add(Paragraph paragraph)
        {
            this.Elements.Add(paragraph);
        }

        public void Add(Image image)
        {
            this.Elements.Add(image);
        }

        public void Add(Table table)
        {
            this.Elements.Add(table);
        }

        public Image AddImage(string name) => 
            this.Elements.AddImage(name);

        public Paragraph AddParagraph() => 
            this.Elements.AddParagraph();

        public Paragraph AddParagraph(string text) => 
            this.Elements.AddParagraph(text);

        public Table AddTable() => 
            this.Elements.AddTable();

        public Footnote Clone() => 
            ((Footnote) this.DeepCopy());

        protected override object DeepCopy()
        {
            Footnote footnote = (Footnote) base.DeepCopy();
            if (footnote.elements != null)
            {
                footnote.elements = footnote.elements.Clone();
                footnote.elements.parent = footnote;
            }
            if (footnote.format != null)
            {
                footnote.format = footnote.format.Clone();
                footnote.format.parent = footnote;
            }
            return footnote;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitFootnote(this);
            if (visitChildren && (this.elements != null))
            {
                ((IVisitable) this.elements).AcceptVisitor(visitor, visitChildren);
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine(@"\footnote");
            int pos = serializer.BeginAttributes();
            if (this.reference.Value != string.Empty)
            {
                serializer.WriteSimpleAttribute("Reference", this.Reference);
            }
            if (this.style.Value != string.Empty)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.IsNull("Format"))
            {
                this.format.Serialize(serializer, "Format", null);
            }
            serializer.EndAttributes(pos);
            pos = serializer.BeginContent();
            if (!this.IsNull("Elements"))
            {
                this.elements.Serialize(serializer);
            }
            serializer.EndContent(pos);
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

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Footnote));
                }
                return meta;
            }
        }

        public string Reference
        {
            get => 
                this.reference.Value;
            set
            {
                this.reference.Value = value;
            }
        }

        public string Style
        {
            get => 
                this.style.Value;
            set
            {
                this.style.Value = value;
            }
        }
    }
}

