namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public class TextArea : ChartObject, IVisitable
    {
        [DV]
        internal Unit bottomPadding;
        [DV(ItemType=typeof(DocumentObject))]
        internal DocumentElements elements;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.FillFormat fillFormat;
        [DV]
        internal ParagraphFormat format;
        [DV]
        internal Unit height;
        [DV]
        internal Unit leftPadding;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.LineFormat lineFormat;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal Unit rightPadding;
        [DV]
        internal NString style;
        [DV]
        internal Unit topPadding;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment))]
        internal NEnum verticalAlignment;
        [DV]
        internal Unit width;

        internal TextArea()
        {
            this.height = Unit.NullValue;
            this.width = Unit.NullValue;
            this.style = NString.NullValue;
            this.leftPadding = Unit.NullValue;
            this.rightPadding = Unit.NullValue;
            this.topPadding = Unit.NullValue;
            this.bottomPadding = Unit.NullValue;
            this.verticalAlignment = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment));
        }

        internal TextArea(DocumentObject parent) : base(parent)
        {
            this.height = Unit.NullValue;
            this.width = Unit.NullValue;
            this.style = NString.NullValue;
            this.leftPadding = Unit.NullValue;
            this.rightPadding = Unit.NullValue;
            this.topPadding = Unit.NullValue;
            this.bottomPadding = Unit.NullValue;
            this.verticalAlignment = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment));
        }

        public void Add(Paragraph paragraph)
        {
            this.Elements.Add(paragraph);
        }

        public void Add(Legend legend)
        {
            this.Elements.Add(legend);
        }

        public void Add(Image image)
        {
            this.Elements.Add(image);
        }

        public void Add(Table table)
        {
            this.Elements.Add(table);
        }

        public Image AddImage(string fileName) => 
            this.Elements.AddImage(fileName);

        public Legend AddLegend() => 
            this.Elements.AddLegend();

        public Paragraph AddParagraph() => 
            this.Elements.AddParagraph();

        public Paragraph AddParagraph(string paragraphText) => 
            this.Elements.AddParagraph(paragraphText);

        public Table AddTable() => 
            this.Elements.AddTable();

        public TextArea Clone() => 
            ((TextArea) this.DeepCopy());

        protected override object DeepCopy()
        {
            TextArea area = (TextArea) base.DeepCopy();
            if (area.format != null)
            {
                area.format = area.format.Clone();
                area.format.parent = area;
            }
            if (area.lineFormat != null)
            {
                area.lineFormat = area.lineFormat.Clone();
                area.lineFormat.parent = area;
            }
            if (area.fillFormat != null)
            {
                area.fillFormat = area.fillFormat.Clone();
                area.fillFormat.parent = area;
            }
            if (area.elements != null)
            {
                area.elements = area.elements.Clone();
                area.elements.parent = area;
            }
            return area;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitTextArea(this);
            if ((this.elements != null) && visitChildren)
            {
                ((IVisitable) this.elements).AcceptVisitor(visitor, visitChildren);
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            Chart parent = base.parent as Chart;
            serializer.WriteLine(@"\" + parent.CheckTextArea(this));
            int pos = serializer.BeginAttributes();
            if (!this.style.IsNull)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.IsNull("Format"))
            {
                this.format.Serialize(serializer, "Format", null);
            }
            if (!this.topPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("TopPadding", this.TopPadding);
            }
            if (!this.leftPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("LeftPadding", this.LeftPadding);
            }
            if (!this.rightPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("RightPadding", this.RightPadding);
            }
            if (!this.bottomPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("BottomPadding", this.BottomPadding);
            }
            if (!this.width.IsNull)
            {
                serializer.WriteSimpleAttribute("Width", this.Width);
            }
            if (!this.height.IsNull)
            {
                serializer.WriteSimpleAttribute("Height", this.Height);
            }
            if (!this.verticalAlignment.IsNull)
            {
                serializer.WriteSimpleAttribute("VerticalAlignment", this.VerticalAlignment);
            }
            if (!this.IsNull("LineFormat"))
            {
                this.lineFormat.Serialize(serializer);
            }
            if (!this.IsNull("FillFormat"))
            {
                this.fillFormat.Serialize(serializer);
            }
            serializer.EndAttributes(pos);
            serializer.BeginContent();
            if (this.elements != null)
            {
                this.elements.Serialize(serializer);
            }
            serializer.EndContent();
        }

        public Unit BottomPadding
        {
            get => 
                this.bottomPadding;
            set
            {
                this.bottomPadding = value;
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

        public MigraDoc.DocumentObjectModel.Shapes.FillFormat FillFormat
        {
            get
            {
                if (this.fillFormat == null)
                {
                    this.fillFormat = new MigraDoc.DocumentObjectModel.Shapes.FillFormat(this);
                }
                return this.fillFormat;
            }
            set
            {
                base.SetParent(value);
                this.fillFormat = value;
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

        public Unit Height
        {
            get => 
                this.height;
            set
            {
                this.height = value;
            }
        }

        public Unit LeftPadding
        {
            get => 
                this.leftPadding;
            set
            {
                this.leftPadding = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.LineFormat LineFormat
        {
            get
            {
                if (this.lineFormat == null)
                {
                    this.lineFormat = new MigraDoc.DocumentObjectModel.Shapes.LineFormat(this);
                }
                return this.lineFormat;
            }
            set
            {
                base.SetParent(value);
                this.lineFormat = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(TextArea));
                }
                return meta;
            }
        }

        public Unit RightPadding
        {
            get => 
                this.rightPadding;
            set
            {
                this.rightPadding = value;
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

        public Unit TopPadding
        {
            get => 
                this.topPadding;
            set
            {
                this.topPadding = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Tables.VerticalAlignment VerticalAlignment
        {
            get => 
                ((MigraDoc.DocumentObjectModel.Tables.VerticalAlignment) this.verticalAlignment.Value);
            set
            {
                this.verticalAlignment.Value = (int) value;
            }
        }

        public Unit Width
        {
            get => 
                this.width;
            set
            {
                this.width = value;
            }
        }
    }
}

