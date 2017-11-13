namespace MigraDoc.DocumentObjectModel.Tables
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public class Cell : DocumentObject, IVisitable
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Borders borders;
        private MigraDoc.DocumentObjectModel.Tables.Column clm;
        [DV]
        internal NString comment;
        [DV(ItemType=typeof(DocumentObject))]
        internal DocumentElements elements;
        [DV]
        internal ParagraphFormat format;
        [DV]
        internal NInt mergeDown;
        [DV]
        internal NInt mergeRight;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        private MigraDoc.DocumentObjectModel.Tables.Row row;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shading shading;
        [DV]
        internal NString style;
        private MigraDoc.DocumentObjectModel.Tables.Table table;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment))]
        internal NEnum verticalAlignment;

        public Cell()
        {
            this.style = NString.NullValue;
            this.verticalAlignment = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment));
            this.mergeRight = NInt.NullValue;
            this.mergeDown = NInt.NullValue;
            this.comment = NString.NullValue;
        }

        internal Cell(DocumentObject parent) : base(parent)
        {
            this.style = NString.NullValue;
            this.verticalAlignment = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment));
            this.mergeRight = NInt.NullValue;
            this.mergeDown = NInt.NullValue;
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

        public TextFrame AddTextFrame() => 
            this.Elements.AddTextFrame();

        public Cell Clone() => 
            ((Cell) this.DeepCopy());

        protected override object DeepCopy()
        {
            Cell cell = (Cell) base.DeepCopy();
            if (cell.format != null)
            {
                cell.format = cell.format.Clone();
                cell.format.parent = cell;
            }
            if (cell.borders != null)
            {
                cell.borders = cell.borders.Clone();
                cell.borders.parent = cell;
            }
            if (cell.shading != null)
            {
                cell.shading = cell.shading.Clone();
                cell.shading.parent = cell;
            }
            if (cell.elements != null)
            {
                cell.elements = cell.elements.Clone();
                cell.elements.parent = cell;
            }
            return cell;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitCell(this);
            if (visitChildren && (this.elements != null))
            {
                ((IVisitable) this.elements).AcceptVisitor(visitor, visitChildren);
            }
        }

        internal override void ResetCachedValues()
        {
            this.row = null;
            this.clm = null;
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine(@"\cell");
            int pos = serializer.BeginAttributes();
            if (this.style.Value != string.Empty)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.IsNull("Format"))
            {
                this.format.Serialize(serializer, "Format", null);
            }
            if (!this.mergeDown.IsNull)
            {
                serializer.WriteSimpleAttribute("MergeDown", this.MergeDown);
            }
            if (!this.mergeRight.IsNull)
            {
                serializer.WriteSimpleAttribute("MergeRight", this.MergeRight);
            }
            if (!this.verticalAlignment.IsNull)
            {
                serializer.WriteSimpleAttribute("VerticalAlignment", this.VerticalAlignment);
            }
            if (!this.IsNull("Borders"))
            {
                this.borders.Serialize(serializer, null);
            }
            if (!this.IsNull("Shading"))
            {
                this.shading.Serialize(serializer);
            }
            serializer.EndAttributes(pos);
            pos = serializer.BeginContent();
            if (!this.IsNull("Elements"))
            {
                this.elements.Serialize(serializer);
            }
            serializer.EndContent(pos);
        }

        public MigraDoc.DocumentObjectModel.Borders Borders
        {
            get
            {
                if (this.borders == null)
                {
                    if (base.Document == null)
                    {
                        base.GetType();
                    }
                    this.borders = new MigraDoc.DocumentObjectModel.Borders(this);
                }
                return this.borders;
            }
            set
            {
                base.SetParent(value);
                this.borders = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Tables.Column Column
        {
            get
            {
                if (this.clm == null)
                {
                    Cells parent = base.Parent as Cells;
                    for (int i = 0; i < parent.Count; i++)
                    {
                        if (parent[i] == this)
                        {
                            this.clm = this.Table.Columns[i];
                        }
                    }
                }
                return this.clm;
            }
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

        public int MergeDown
        {
            get => 
                this.mergeDown.Value;
            set
            {
                this.mergeDown.Value = value;
            }
        }

        public int MergeRight
        {
            get => 
                this.mergeRight.Value;
            set
            {
                this.mergeRight.Value = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Cell));
                }
                return meta;
            }
        }

        public MigraDoc.DocumentObjectModel.Tables.Row Row
        {
            get
            {
                if (this.row == null)
                {
                    Cells parent = base.Parent as Cells;
                    this.row = parent.Row;
                }
                return this.row;
            }
        }

        public MigraDoc.DocumentObjectModel.Shading Shading
        {
            get
            {
                if (this.shading == null)
                {
                    this.shading = new MigraDoc.DocumentObjectModel.Shading(this);
                }
                return this.shading;
            }
            set
            {
                base.SetParent(value);
                this.shading = value;
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

        public MigraDoc.DocumentObjectModel.Tables.Table Table
        {
            get
            {
                if (this.table == null)
                {
                    Cells parent = base.Parent as Cells;
                    if (parent != null)
                    {
                        this.table = parent.Table;
                    }
                }
                return this.table;
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
    }
}

