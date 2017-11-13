namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class TickLabels : ChartObject
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Font font;
        [DV]
        internal NString format;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString style;

        public TickLabels()
        {
            this.style = NString.NullValue;
            this.format = NString.NullValue;
        }

        internal TickLabels(DocumentObject parent) : base(parent)
        {
            this.style = NString.NullValue;
            this.format = NString.NullValue;
        }

        public TickLabels Clone() => 
            ((TickLabels) this.DeepCopy());

        protected override object DeepCopy()
        {
            TickLabels labels = (TickLabels) base.DeepCopy();
            if (labels.font != null)
            {
                labels.font = labels.font.Clone();
                labels.font.parent = labels;
            }
            return labels;
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.BeginContent("TickLabels");
            if (!this.style.IsNull)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (this.font != null)
            {
                this.font.Serialize(serializer);
            }
            if (!this.format.IsNull)
            {
                serializer.WriteSimpleAttribute("Format", this.Format);
            }
            serializer.EndContent();
        }

        public MigraDoc.DocumentObjectModel.Font Font
        {
            get
            {
                if (this.font == null)
                {
                    this.font = new MigraDoc.DocumentObjectModel.Font(this);
                }
                return this.font;
            }
            set
            {
                base.SetParent(value);
                this.font = value;
            }
        }

        public string Format
        {
            get => 
                this.format.Value;
            set
            {
                this.format.Value = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(TickLabels));
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

