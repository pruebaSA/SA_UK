namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class DataLabel : DocumentObject
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Font font;
        [DV]
        internal NString format;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV(Type=typeof(DataLabelPosition))]
        internal NEnum position;
        [DV]
        internal NString style;
        [DV(Type=typeof(DataLabelType))]
        internal NEnum type;

        public DataLabel()
        {
            this.format = NString.NullValue;
            this.style = NString.NullValue;
            this.position = NEnum.NullValue(typeof(DataLabelPosition));
            this.type = NEnum.NullValue(typeof(DataLabelType));
        }

        internal DataLabel(DocumentObject parent) : base(parent)
        {
            this.format = NString.NullValue;
            this.style = NString.NullValue;
            this.position = NEnum.NullValue(typeof(DataLabelPosition));
            this.type = NEnum.NullValue(typeof(DataLabelType));
        }

        public DataLabel Clone() => 
            ((DataLabel) this.DeepCopy());

        protected override object DeepCopy()
        {
            DataLabel label = (DataLabel) base.DeepCopy();
            if (label.font != null)
            {
                label.font = label.font.Clone();
                label.font.parent = label;
            }
            return label;
        }

        internal override void Serialize(Serializer serializer)
        {
            int pos = serializer.BeginContent("DataLabel");
            if (this.Style != string.Empty)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (this.Format != string.Empty)
            {
                serializer.WriteSimpleAttribute("Format", this.Format);
            }
            if (!this.position.IsNull)
            {
                serializer.WriteSimpleAttribute("Position", this.Position);
            }
            if (!this.type.IsNull)
            {
                serializer.WriteSimpleAttribute("Type", this.Type);
            }
            if (!this.IsNull("Font"))
            {
                this.font.Serialize(serializer);
            }
            serializer.EndContent(pos);
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
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(DataLabel));
                }
                return meta;
            }
        }

        public DataLabelPosition Position
        {
            get => 
                ((DataLabelPosition) this.position.Value);
            set
            {
                this.position.Value = (int) value;
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

        public DataLabelType Type
        {
            get => 
                ((DataLabelType) this.type.Value);
            set
            {
                this.type.Value = (int) value;
            }
        }
    }
}

