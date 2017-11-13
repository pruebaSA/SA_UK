namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;

    public class AxisTitle : ChartObject
    {
        [DV(Type=typeof(HorizontalAlignment))]
        internal NEnum alignment;
        [DV]
        internal NString caption;
        [DV]
        internal MigraDoc.DocumentObjectModel.Font font;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal Unit orientation;
        [DV]
        internal NString style;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment))]
        internal NEnum verticalAlignment;

        public AxisTitle()
        {
            this.style = NString.NullValue;
            this.caption = NString.NullValue;
            this.orientation = Unit.NullValue;
            this.alignment = NEnum.NullValue(typeof(HorizontalAlignment));
            this.verticalAlignment = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment));
        }

        internal AxisTitle(DocumentObject parent) : base(parent)
        {
            this.style = NString.NullValue;
            this.caption = NString.NullValue;
            this.orientation = Unit.NullValue;
            this.alignment = NEnum.NullValue(typeof(HorizontalAlignment));
            this.verticalAlignment = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Tables.VerticalAlignment));
        }

        public AxisTitle Clone() => 
            ((AxisTitle) this.DeepCopy());

        protected override object DeepCopy()
        {
            AxisTitle title = (AxisTitle) base.DeepCopy();
            if (title.font != null)
            {
                title.font = title.font.Clone();
                title.font.parent = title;
            }
            return title;
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.BeginContent("Title");
            if (!this.style.IsNull)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.IsNull("Font"))
            {
                this.font.Serialize(serializer);
            }
            if (!this.orientation.IsNull)
            {
                serializer.WriteSimpleAttribute("Orientation", this.Orientation);
            }
            if (!this.alignment.IsNull)
            {
                serializer.WriteSimpleAttribute("Alignment", this.Alignment);
            }
            if (!this.verticalAlignment.IsNull)
            {
                serializer.WriteSimpleAttribute("VerticalAlignment", this.VerticalAlignment);
            }
            if (!this.caption.IsNull)
            {
                serializer.WriteSimpleAttribute("Caption", this.Caption);
            }
            serializer.EndContent();
        }

        public HorizontalAlignment Alignment
        {
            get => 
                ((HorizontalAlignment) this.alignment.Value);
            set
            {
                this.alignment.Value = (int) value;
            }
        }

        public string Caption
        {
            get => 
                this.caption.Value;
            set
            {
                this.caption.Value = value;
            }
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

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(AxisTitle));
                }
                return meta;
            }
        }

        public Unit Orientation
        {
            get => 
                this.orientation;
            set
            {
                this.orientation = value;
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

