namespace MigraDoc.DocumentObjectModel.Tables
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;
    using System.Reflection;

    public class Rows : DocumentObjectCollection, IVisitable
    {
        [DV(Type=typeof(RowAlignment))]
        internal NEnum alignment;
        [DV]
        internal NString comment;
        [DV]
        internal Unit height;
        [DV(Type=typeof(RowHeightRule))]
        internal NEnum heightRule;
        [DV]
        internal Unit leftIndent;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment))]
        internal NEnum verticalAlignment;

        public Rows()
        {
            this.alignment = NEnum.NullValue(typeof(RowAlignment));
            this.leftIndent = Unit.NullValue;
            this.verticalAlignment = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment));
            this.height = Unit.NullValue;
            this.heightRule = NEnum.NullValue(typeof(RowHeightRule));
            this.comment = NString.NullValue;
        }

        internal Rows(DocumentObject parent) : base(parent)
        {
            this.alignment = NEnum.NullValue(typeof(RowAlignment));
            this.leftIndent = Unit.NullValue;
            this.verticalAlignment = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment));
            this.height = Unit.NullValue;
            this.heightRule = NEnum.NullValue(typeof(RowHeightRule));
            this.comment = NString.NullValue;
        }

        public Row AddRow()
        {
            if (this.Table.Columns.Count == 0)
            {
                throw new InvalidOperationException("Cannot add row, because no columns exists.");
            }
            Row row = new Row();
            this.Add(row);
            return row;
        }

        public Rows Clone() => 
            ((Rows) base.DeepCopy());

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitRows(this);
            foreach (Row row in this)
            {
                ((IVisitable) row).AcceptVisitor(visitor, visitChildren);
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine(@"\rows");
            int pos = serializer.BeginAttributes();
            if (!this.alignment.IsNull)
            {
                serializer.WriteSimpleAttribute("Alignment", this.Alignment);
            }
            if (!this.height.IsNull)
            {
                serializer.WriteSimpleAttribute("Height", this.Height);
            }
            if (!this.heightRule.IsNull)
            {
                serializer.WriteSimpleAttribute("HeightRule", this.HeightRule);
            }
            if (!this.leftIndent.IsNull)
            {
                serializer.WriteSimpleAttribute("LeftIndent", this.LeftIndent);
            }
            if (!this.verticalAlignment.IsNull)
            {
                serializer.WriteSimpleAttribute("VerticalAlignment", this.VerticalAlignment);
            }
            serializer.EndAttributes(pos);
            serializer.BeginContent();
            int count = base.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    this[i].Serialize(serializer);
                }
            }
            else
            {
                serializer.WriteComment("Invalid - no rows defined. Table will not render.");
            }
            serializer.EndContent();
        }

        public RowAlignment Alignment
        {
            get => 
                ((RowAlignment) this.alignment.Value);
            set
            {
                this.alignment.Value = (int) value;
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

        public Row this[int index] =>
            (base[index] as Row);

        public Unit LeftIndent
        {
            get => 
                this.leftIndent;
            set
            {
                this.leftIndent = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Rows));
                }
                return meta;
            }
        }

        public MigraDoc.DocumentObjectModel.Tables.Table Table =>
            (base.parent as MigraDoc.DocumentObjectModel.Tables.Table);

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

