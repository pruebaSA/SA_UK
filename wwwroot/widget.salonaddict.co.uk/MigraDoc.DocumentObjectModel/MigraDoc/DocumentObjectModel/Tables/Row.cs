namespace MigraDoc.DocumentObjectModel.Tables
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;
    using System.Reflection;

    public class Row : DocumentObject, IVisitable
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Borders borders;
        [DV]
        internal Unit bottomPadding;
        [DV]
        internal MigraDoc.DocumentObjectModel.Tables.Cells cells;
        [DV]
        internal NString comment;
        [DV]
        internal ParagraphFormat format;
        [DV]
        internal NBool headingFormat;
        [DV]
        internal Unit height;
        [DV(Type=typeof(RowHeightRule))]
        internal NEnum heightRule;
        [DV]
        internal NInt index;
        [DV]
        internal NInt keepWith;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shading shading;
        [DV]
        internal NString style;
        private MigraDoc.DocumentObjectModel.Tables.Table table;
        [DV]
        internal Unit topPadding;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment))]
        internal NEnum verticalAlignment;

        public Row()
        {
            this.index = NInt.NullValue;
            this.style = NString.NullValue;
            this.verticalAlignment = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment));
            this.height = Unit.NullValue;
            this.heightRule = NEnum.NullValue(typeof(RowHeightRule));
            this.topPadding = Unit.NullValue;
            this.bottomPadding = Unit.NullValue;
            this.headingFormat = NBool.NullValue;
            this.keepWith = NInt.NullValue;
            this.comment = NString.NullValue;
        }

        internal Row(DocumentObject parent) : base(parent)
        {
            this.index = NInt.NullValue;
            this.style = NString.NullValue;
            this.verticalAlignment = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment));
            this.height = Unit.NullValue;
            this.heightRule = NEnum.NullValue(typeof(RowHeightRule));
            this.topPadding = Unit.NullValue;
            this.bottomPadding = Unit.NullValue;
            this.headingFormat = NBool.NullValue;
            this.keepWith = NInt.NullValue;
            this.comment = NString.NullValue;
        }

        public Row Clone() => 
            ((Row) this.DeepCopy());

        protected override object DeepCopy()
        {
            Row row = (Row) base.DeepCopy();
            if (row.format != null)
            {
                row.format = row.format.Clone();
                row.format.parent = row;
            }
            if (row.borders != null)
            {
                row.borders = row.borders.Clone();
                row.borders.parent = row;
            }
            if (row.shading != null)
            {
                row.shading = row.shading.Clone();
                row.shading.parent = row;
            }
            if (row.cells != null)
            {
                row.cells = row.cells.Clone();
                row.cells.parent = row;
            }
            return row;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitRow(this);
            foreach (Cell cell in this.cells)
            {
                ((IVisitable) cell).AcceptVisitor(visitor, visitChildren);
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine(@"\row");
            int pos = serializer.BeginAttributes();
            if (this.style.Value != string.Empty)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.IsNull("Format"))
            {
                this.format.Serialize(serializer, "Format", null);
            }
            if (!this.height.IsNull)
            {
                serializer.WriteSimpleAttribute("Height", this.Height);
            }
            if (!this.heightRule.IsNull)
            {
                serializer.WriteSimpleAttribute("HeightRule", this.HeightRule);
            }
            if (!this.topPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("TopPadding", this.TopPadding);
            }
            if (!this.bottomPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("BottomPadding", this.BottomPadding);
            }
            if (!this.headingFormat.IsNull)
            {
                serializer.WriteSimpleAttribute("HeadingFormat", this.HeadingFormat);
            }
            if (!this.verticalAlignment.IsNull)
            {
                serializer.WriteSimpleAttribute("VerticalAlignment", this.VerticalAlignment);
            }
            if (!this.keepWith.IsNull)
            {
                serializer.WriteSimpleAttribute("KeepWith", this.KeepWith);
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
            serializer.BeginContent();
            if (!this.IsNull("Cells"))
            {
                this.cells.Serialize(serializer);
            }
            serializer.EndContent();
        }

        public MigraDoc.DocumentObjectModel.Borders Borders
        {
            get
            {
                if (this.borders == null)
                {
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

        public Unit BottomPadding
        {
            get => 
                this.bottomPadding;
            set
            {
                this.bottomPadding = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Tables.Cells Cells
        {
            get
            {
                if (this.cells == null)
                {
                    this.cells = new MigraDoc.DocumentObjectModel.Tables.Cells(this);
                }
                return this.cells;
            }
            set
            {
                base.SetParent(value);
                this.cells = value;
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

        public bool HeadingFormat
        {
            get => 
                this.headingFormat.Value;
            set
            {
                this.headingFormat.Value = value;
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

        public RowHeightRule HeightRule
        {
            get => 
                ((RowHeightRule) this.heightRule.Value);
            set
            {
                this.heightRule.Value = (int) value;
            }
        }

        public int Index
        {
            get
            {
                if (this.IsNull("index"))
                {
                    Rows parent = base.parent as Rows;
                    this.SetValue("Index", parent.IndexOf(this));
                }
                return (int) this.index;
            }
        }

        public Cell this[int index] =>
            this.Cells[index];

        public int KeepWith
        {
            get => 
                this.keepWith.Value;
            set
            {
                this.keepWith.Value = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Row));
                }
                return meta;
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
                    Rows parent = base.Parent as Rows;
                    if (parent != null)
                    {
                        this.table = parent.Table;
                    }
                }
                return this.table;
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
    }
}

