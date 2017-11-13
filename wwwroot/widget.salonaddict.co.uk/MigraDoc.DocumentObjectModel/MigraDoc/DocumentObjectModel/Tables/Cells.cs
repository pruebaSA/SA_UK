namespace MigraDoc.DocumentObjectModel.Tables
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Reflection;

    public class Cells : DocumentObjectCollection
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        private MigraDoc.DocumentObjectModel.Tables.Row row;
        private MigraDoc.DocumentObjectModel.Tables.Table table;

        public Cells()
        {
        }

        internal Cells(DocumentObject parent) : base(parent)
        {
        }

        public Cells Clone() => 
            ((Cells) base.DeepCopy());

        private void Resize(int index)
        {
            for (int i = base.Count; i <= index; i++)
            {
                this.Add(new Cell());
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                this[i].Serialize(serializer);
            }
        }

        public Cell this[int index]
        {
            get
            {
                if ((index < 0) || ((this.Table != null) && (index >= this.Table.Columns.Count)))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                this.Resize(index);
                return (base[index] as Cell);
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Cells));
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
                    this.row = base.Parent as MigraDoc.DocumentObjectModel.Tables.Row;
                }
                return this.row;
            }
        }

        public MigraDoc.DocumentObjectModel.Tables.Table Table
        {
            get
            {
                if (this.table == null)
                {
                    MigraDoc.DocumentObjectModel.Tables.Row parent = base.Parent as MigraDoc.DocumentObjectModel.Tables.Row;
                    if (parent != null)
                    {
                        this.table = parent.Table;
                    }
                }
                return this.table;
            }
        }
    }
}

