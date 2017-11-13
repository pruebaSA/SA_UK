namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public class Legend : ChartObject, IVisitable
    {
        [DV]
        internal ParagraphFormat format;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.LineFormat lineFormat;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString style;

        public Legend()
        {
            this.style = NString.NullValue;
        }

        internal Legend(DocumentObject parent) : base(parent)
        {
            this.style = NString.NullValue;
        }

        public Legend Clone() => 
            ((Legend) this.DeepCopy());

        protected override object DeepCopy()
        {
            Legend legend = (Legend) base.DeepCopy();
            if (legend.format != null)
            {
                legend.format = legend.format.Clone();
                legend.format.parent = legend;
            }
            if (legend.lineFormat != null)
            {
                legend.lineFormat = legend.lineFormat.Clone();
                legend.lineFormat.parent = legend;
            }
            return legend;
        }

        public override bool IsNull() => 
            false;

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitLegend(this);
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine(@"\legend");
            int pos = serializer.BeginAttributes();
            if (!this.style.IsNull)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.IsNull("Format"))
            {
                this.format.Serialize(serializer, "Format", null);
            }
            if (!this.IsNull("LineFormat"))
            {
                this.lineFormat.Serialize(serializer);
            }
            serializer.EndAttributes(pos);
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
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Legend));
                }
                return meta;
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
    }
}

