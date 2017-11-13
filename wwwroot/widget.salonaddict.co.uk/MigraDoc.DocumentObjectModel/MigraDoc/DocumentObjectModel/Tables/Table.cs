namespace MigraDoc.DocumentObjectModel.Tables
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;
    using System.Reflection;

    public class Table : DocumentObject, IVisitable
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Borders borders;
        [DV]
        internal Unit bottomPadding;
        [DV]
        internal MigraDoc.DocumentObjectModel.Tables.Columns columns;
        [DV]
        internal NString comment;
        [DV]
        internal ParagraphFormat format;
        [DV]
        internal NBool keepTogether;
        [DV]
        internal Unit leftPadding;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal Unit rightPadding;
        [DV]
        internal MigraDoc.DocumentObjectModel.Tables.Rows rows;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shading shading;
        [DV]
        internal NString style;
        [DV]
        internal Unit topPadding;

        public Table()
        {
            this.style = NString.NullValue;
            this.topPadding = Unit.NullValue;
            this.bottomPadding = Unit.NullValue;
            this.leftPadding = Unit.NullValue;
            this.rightPadding = Unit.NullValue;
            this.keepTogether = NBool.NullValue;
            this.comment = NString.NullValue;
        }

        internal Table(DocumentObject parent) : base(parent)
        {
            this.style = NString.NullValue;
            this.topPadding = Unit.NullValue;
            this.bottomPadding = Unit.NullValue;
            this.leftPadding = Unit.NullValue;
            this.rightPadding = Unit.NullValue;
            this.keepTogether = NBool.NullValue;
            this.comment = NString.NullValue;
        }

        public Column AddColumn() => 
            this.Columns.AddColumn();

        public Column AddColumn(Unit width)
        {
            Column column = this.Columns.AddColumn();
            column.Width = width;
            return column;
        }

        public Row AddRow() => 
            this.rows.AddRow();

        public Table Clone() => 
            ((Table) this.DeepCopy());

        protected override object DeepCopy()
        {
            Table table = (Table) base.DeepCopy();
            if (table.columns != null)
            {
                table.columns = table.columns.Clone();
                table.columns.parent = table;
            }
            if (table.rows != null)
            {
                table.rows = table.rows.Clone();
                table.rows.parent = table;
            }
            if (table.format != null)
            {
                table.format = table.format.Clone();
                table.format.parent = table;
            }
            if (table.borders != null)
            {
                table.borders = table.borders.Clone();
                table.borders.parent = table;
            }
            if (table.shading != null)
            {
                table.shading = table.shading.Clone();
                table.shading.parent = table;
            }
            return table;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitTable(this);
            ((IVisitable) this.columns).AcceptVisitor(visitor, visitChildren);
            ((IVisitable) this.rows).AcceptVisitor(visitor, visitChildren);
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine(@"\table");
            int pos = serializer.BeginAttributes();
            if (this.style.Value != string.Empty)
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
            this.Columns.Serialize(serializer);
            this.Rows.Serialize(serializer);
            serializer.EndContent();
        }

        public void SetEdge(int clm, int row, int clms, int rows, Edge edge, BorderStyle style, Unit width)
        {
            this.SetEdge(clm, row, clms, rows, edge, style, width, Color.Empty);
        }

        public void SetEdge(int clm, int row, int clms, int rows, Edge edge, BorderStyle style, Unit width, Color clr)
        {
            int num = (row + rows) - 1;
            int num2 = (clm + clms) - 1;
            for (int i = row; i <= num; i++)
            {
                Row row2 = this.rows[i];
                for (int j = clm; j <= num2; j++)
                {
                    Border top;
                    Cell cell = row2[j];
                    if (((edge & Edge.Top) == Edge.Top) && (i == row))
                    {
                        top = cell.Borders.Top;
                        top.Style = style;
                        top.Width = width;
                        if (clr != Color.Empty)
                        {
                            top.Color = clr;
                        }
                    }
                    if (((edge & Edge.Left) == Edge.Left) && (j == clm))
                    {
                        top = cell.Borders.Left;
                        top.Style = style;
                        top.Width = width;
                        if (clr != Color.Empty)
                        {
                            top.Color = clr;
                        }
                    }
                    if (((edge & Edge.Bottom) == Edge.Bottom) && (i == num))
                    {
                        top = cell.Borders.Bottom;
                        top.Style = style;
                        top.Width = width;
                        if (clr != Color.Empty)
                        {
                            top.Color = clr;
                        }
                    }
                    if (((edge & Edge.Right) == Edge.Right) && (j == num2))
                    {
                        top = cell.Borders.Right;
                        top.Style = style;
                        top.Width = width;
                        if (clr != Color.Empty)
                        {
                            top.Color = clr;
                        }
                    }
                    if (((edge & Edge.Horizontal) == Edge.Horizontal) && (i < num))
                    {
                        top = cell.Borders.Bottom;
                        top.Style = style;
                        top.Width = width;
                        if (clr != Color.Empty)
                        {
                            top.Color = clr;
                        }
                    }
                    if (((edge & Edge.Vertical) == Edge.Vertical) && (j < num2))
                    {
                        top = cell.Borders.Right;
                        top.Style = style;
                        top.Width = width;
                        if (clr != Color.Empty)
                        {
                            top.Color = clr;
                        }
                    }
                    if ((edge & Edge.DiagonalDown) == Edge.DiagonalDown)
                    {
                        top = cell.Borders.DiagonalDown;
                        top.Style = style;
                        top.Width = width;
                        if (clr != Color.Empty)
                        {
                            top.Color = clr;
                        }
                    }
                    if ((edge & Edge.DiagonalUp) == Edge.DiagonalUp)
                    {
                        top = cell.Borders.DiagonalUp;
                        top.Style = style;
                        top.Width = width;
                        if (clr != Color.Empty)
                        {
                            top.Color = clr;
                        }
                    }
                }
            }
        }

        public void SetShading(int clm, int row, int clms, int rows, Color clr)
        {
            int count = this.rows.Count;
            int num2 = this.columns.Count;
            if ((row < 0) || (row >= count))
            {
                throw new ArgumentOutOfRangeException("row", row, "Invalid row index.");
            }
            if ((clm < 0) || (clm >= num2))
            {
                throw new ArgumentOutOfRangeException("clm", clm, "Invalid column index.");
            }
            if ((rows <= 0) || ((row + rows) > count))
            {
                throw new ArgumentOutOfRangeException("rows", rows, "Invalid row count.");
            }
            if ((clms <= 0) || ((clm + clms) > num2))
            {
                throw new ArgumentOutOfRangeException("clms", clms, "Invalid column count.");
            }
            int num3 = (row + rows) - 1;
            int num4 = (clm + clms) - 1;
            for (int i = row; i <= num3; i++)
            {
                Row row2 = this.rows[i];
                for (int j = clm; j <= num4; j++)
                {
                    row2[j].Shading.Color = clr;
                }
            }
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

        public MigraDoc.DocumentObjectModel.Tables.Columns Columns
        {
            get
            {
                if (this.columns == null)
                {
                    this.columns = new MigraDoc.DocumentObjectModel.Tables.Columns(this);
                }
                return this.columns;
            }
            set
            {
                base.SetParent(value);
                this.columns = value;
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

        public bool IsEmpty
        {
            get
            {
                if (this.Rows.Count != 0)
                {
                    return (this.Columns.Count == 0);
                }
                return true;
            }
        }

        public Cell this[int rwIdx, int clmIdx] =>
            this.Rows[rwIdx].Cells[clmIdx];

        public bool KeepTogether
        {
            get => 
                this.keepTogether.Value;
            set
            {
                this.keepTogether.Value = value;
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

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Table));
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

        public MigraDoc.DocumentObjectModel.Tables.Rows Rows
        {
            get
            {
                if (this.rows == null)
                {
                    this.rows = new MigraDoc.DocumentObjectModel.Tables.Rows(this);
                }
                return this.rows;
            }
            set
            {
                base.SetParent(value);
                this.rows = value;
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

        public Unit TopPadding
        {
            get => 
                this.topPadding;
            set
            {
                this.topPadding = value;
            }
        }
    }
}

