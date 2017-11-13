namespace MigraDoc.DocumentObjectModel.Shapes
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public class TextFrame : Shape, IVisitable
    {
        [DV(ItemType=typeof(DocumentObject))]
        protected DocumentElements elements;
        [DV]
        internal Unit marginBottom;
        [DV]
        internal Unit marginLeft;
        [DV]
        internal Unit marginRight;
        [DV]
        internal Unit marginTop;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV(Type=typeof(TextOrientation))]
        internal NEnum orientation;

        public TextFrame()
        {
            this.marginLeft = Unit.NullValue;
            this.marginRight = Unit.NullValue;
            this.marginTop = Unit.NullValue;
            this.marginBottom = Unit.NullValue;
            this.orientation = NEnum.NullValue(typeof(TextOrientation));
        }

        internal TextFrame(DocumentObject parent) : base(parent)
        {
            this.marginLeft = Unit.NullValue;
            this.marginRight = Unit.NullValue;
            this.marginTop = Unit.NullValue;
            this.marginBottom = Unit.NullValue;
            this.orientation = NEnum.NullValue(typeof(TextOrientation));
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

        public void Add(Table table)
        {
            this.Elements.Add(table);
        }

        public Chart AddChart() => 
            this.Elements.AddChart();

        public Chart AddChart(ChartType _type) => 
            this.Elements.AddChart(_type);

        public Image AddImage(string _fileName) => 
            this.Elements.AddImage(_fileName);

        public Paragraph AddParagraph() => 
            this.Elements.AddParagraph();

        public Paragraph AddParagraph(string _paragraphText) => 
            this.Elements.AddParagraph(_paragraphText);

        public Table AddTable() => 
            this.Elements.AddTable();

        public TextFrame Clone() => 
            ((TextFrame) this.DeepCopy());

        protected override object DeepCopy()
        {
            TextFrame frame = (TextFrame) base.DeepCopy();
            if (frame.elements != null)
            {
                frame.elements = frame.elements.Clone();
                frame.elements.parent = frame;
            }
            return frame;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitTextFrame(this);
            if (visitChildren && (this.elements != null))
            {
                ((IVisitable) this.elements).AcceptVisitor(visitor, visitChildren);
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine(@"\textframe");
            int pos = serializer.BeginAttributes();
            base.Serialize(serializer);
            if (!this.marginLeft.IsNull)
            {
                serializer.WriteSimpleAttribute("MarginLeft", this.MarginLeft);
            }
            if (!this.marginRight.IsNull)
            {
                serializer.WriteSimpleAttribute("MarginRight", this.MarginRight);
            }
            if (!this.marginTop.IsNull)
            {
                serializer.WriteSimpleAttribute("MarginTop", this.MarginTop);
            }
            if (!this.marginBottom.IsNull)
            {
                serializer.WriteSimpleAttribute("MarginBottom", this.MarginBottom);
            }
            if (!this.orientation.IsNull)
            {
                serializer.WriteSimpleAttribute("Orientation", this.Orientation);
            }
            serializer.EndAttributes(pos);
            serializer.BeginContent();
            if (this.elements != null)
            {
                this.elements.Serialize(serializer);
            }
            serializer.EndContent();
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

        public Unit MarginBottom
        {
            get => 
                this.marginBottom;
            set
            {
                this.marginBottom = value;
            }
        }

        public Unit MarginLeft
        {
            get => 
                this.marginLeft;
            set
            {
                this.marginLeft = value;
            }
        }

        public Unit MarginRight
        {
            get => 
                this.marginRight;
            set
            {
                this.marginRight = value;
            }
        }

        public Unit MarginTop
        {
            get => 
                this.marginTop;
            set
            {
                this.marginTop = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(TextFrame));
                }
                return meta;
            }
        }

        public TextOrientation Orientation
        {
            get => 
                ((TextOrientation) this.orientation.Value);
            set
            {
                this.orientation.Value = (int) value;
            }
        }
    }
}

