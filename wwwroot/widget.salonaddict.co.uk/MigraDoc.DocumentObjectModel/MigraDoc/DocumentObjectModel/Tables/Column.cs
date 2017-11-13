namespace MigraDoc.DocumentObjectModel.Tables
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Reflection;

    public class Column : DocumentObject
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Borders borders;
        [DV]
        internal NString comment;
        [DV]
        internal ParagraphFormat format;
        [DV]
        internal NBool headingFormat;
        [DV]
        internal NInt index;
        [DV]
        internal NInt keepWith;
        [DV]
        internal Unit leftPadding;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal Unit rightPadding;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shading shading;
        [DV]
        internal NString style;
        private MigraDoc.DocumentObjectModel.Tables.Table table;
        [DV]
        internal Unit width;

        public Column()
        {
            this.index = NInt.NullValue;
            this.style = NString.NullValue;
            this.width = Unit.NullValue;
            this.leftPadding = Unit.NullValue;
            this.rightPadding = Unit.NullValue;
            this.keepWith = NInt.NullValue;
            this.headingFormat = NBool.NullValue;
            this.comment = NString.NullValue;
        }

        internal Column(DocumentObject parent) : base(parent)
        {
            this.index = NInt.NullValue;
            this.style = NString.NullValue;
            this.width = Unit.NullValue;
            this.leftPadding = Unit.NullValue;
            this.rightPadding = Unit.NullValue;
            this.keepWith = NInt.NullValue;
            this.headingFormat = NBool.NullValue;
            this.comment = NString.NullValue;
        }

        public Column Clone() => 
            ((Column) this.DeepCopy());

        protected override object DeepCopy()
        {
            Column column = (Column) base.DeepCopy();
            if (column.format != null)
            {
                column.format = column.format.Clone();
                column.format.parent = column;
            }
            if (column.borders != null)
            {
                column.borders = column.borders.Clone();
                column.borders.parent = column;
            }
            if (column.shading != null)
            {
                column.shading = column.shading.Clone();
                column.shading.parent = column;
            }
            return column;
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine(@"\column");
            int pos = serializer.BeginAttributes();
            if (this.style.Value != string.Empty)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.IsNull("Format"))
            {
                this.format.Serialize(serializer, "Format", null);
            }
            if (!this.headingFormat.IsNull)
            {
                serializer.WriteSimpleAttribute("HeadingFormat", this.HeadingFormat);
            }
            if (!this.leftPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("LeftPadding", this.LeftPadding);
            }
            if (!this.rightPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("RightPadding", this.RightPadding);
            }
            if (!this.width.IsNull)
            {
                serializer.WriteSimpleAttribute("Width", this.Width);
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

        public int Index
        {
            get
            {
                if (this.IsNull("Index"))
                {
                    Columns parent = base.Parent as Columns;
                    this.SetValue("Index", parent.IndexOf(this));
                }
                return (int) this.index;
            }
        }

        public Cell this[int index] =>
            this.Table.Rows[index][(int) this.index];

        public int KeepWith
        {
            get => 
                this.keepWith.Value;
            set
            {
                this.keepWith.Value = value;
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
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Column));
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
                    Columns parent = base.Parent as Columns;
                    if (parent != null)
                    {
                        this.table = parent.Parent as MigraDoc.DocumentObjectModel.Tables.Table;
                    }
                }
                return this.table;
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

