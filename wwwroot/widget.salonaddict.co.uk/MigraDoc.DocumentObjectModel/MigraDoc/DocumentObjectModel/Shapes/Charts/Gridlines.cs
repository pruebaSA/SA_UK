namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using System;

    public class Gridlines : ChartObject
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.LineFormat lineFormat;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public Gridlines()
        {
        }

        internal Gridlines(DocumentObject parent) : base(parent)
        {
        }

        public Gridlines Clone() => 
            ((Gridlines) this.DeepCopy());

        protected override object DeepCopy()
        {
            Gridlines gridlines = (Gridlines) base.DeepCopy();
            if (gridlines.lineFormat != null)
            {
                gridlines.lineFormat = gridlines.lineFormat.Clone();
                gridlines.lineFormat.parent = gridlines;
            }
            return gridlines;
        }

        internal override void Serialize(Serializer serializer)
        {
            Axis parent = base.parent as Axis;
            serializer.BeginContent(parent.CheckGridlines(this));
            if (!this.IsNull("LineFormat"))
            {
                this.lineFormat.Serialize(serializer);
            }
            serializer.EndContent();
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
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Gridlines));
                }
                return meta;
            }
        }
    }
}

