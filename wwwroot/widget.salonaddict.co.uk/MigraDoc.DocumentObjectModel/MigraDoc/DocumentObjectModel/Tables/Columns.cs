namespace MigraDoc.DocumentObjectModel.Tables
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;
    using System.Reflection;

    public class Columns : DocumentObjectCollection, IVisitable
    {
        [DV]
        internal NString comment;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal Unit width;

        public Columns()
        {
            this.width = Unit.NullValue;
            this.comment = NString.NullValue;
        }

        public Columns(params Unit[] widths)
        {
            this.width = Unit.NullValue;
            this.comment = NString.NullValue;
            foreach (Unit unit in widths)
            {
                Column column = new Column {
                    Width = unit
                };
                this.Add(column);
            }
        }

        internal Columns(DocumentObject parent) : base(parent)
        {
            this.width = Unit.NullValue;
            this.comment = NString.NullValue;
        }

        public Column AddColumn()
        {
            if (this.Table.Rows.Count > 0)
            {
                throw new InvalidOperationException("Cannot add column because rows collection is not empty.");
            }
            Column column = new Column();
            this.Add(column);
            return column;
        }

        public Columns Clone() => 
            ((Columns) base.DeepCopy());

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitColumns(this);
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine(@"\columns");
            int pos = serializer.BeginAttributes();
            if (!this.width.IsNull)
            {
                serializer.WriteSimpleAttribute("Width", this.Width);
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
                serializer.WriteComment("Invalid - no columns defined. Table will not render.");
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

        public Column this[int index] =>
            (base[index] as Column);

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Columns));
                }
                return meta;
            }
        }

        public MigraDoc.DocumentObjectModel.Tables.Table Table =>
            (base.parent as MigraDoc.DocumentObjectModel.Tables.Table);

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

