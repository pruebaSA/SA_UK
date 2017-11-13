namespace MigraDoc.DocumentObjectModel.Shapes
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class LineFormat : DocumentObject
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Color color;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Shapes.DashStyle))]
        internal NEnum dashStyle;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV(Type=typeof(LineStyle))]
        internal NEnum style;
        [DV]
        internal NBool visible;
        [DV]
        internal Unit width;

        public LineFormat()
        {
            this.visible = NBool.NullValue;
            this.width = Unit.NullValue;
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.dashStyle = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Shapes.DashStyle));
            this.style = NEnum.NullValue(typeof(LineStyle));
        }

        internal LineFormat(DocumentObject parent) : base(parent)
        {
            this.visible = NBool.NullValue;
            this.width = Unit.NullValue;
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.dashStyle = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Shapes.DashStyle));
            this.style = NEnum.NullValue(typeof(LineStyle));
        }

        public LineFormat Clone() => 
            ((LineFormat) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            serializer.BeginContent("LineFormat");
            if (!this.visible.IsNull)
            {
                serializer.WriteSimpleAttribute("Visible", this.Visible);
            }
            if (!this.style.IsNull)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.dashStyle.IsNull)
            {
                serializer.WriteSimpleAttribute("DashStyle", this.DashStyle);
            }
            if (!this.width.IsNull)
            {
                serializer.WriteSimpleAttribute("Width", this.Width);
            }
            if (!this.color.IsNull)
            {
                serializer.WriteSimpleAttribute("Color", this.Color);
            }
            serializer.EndContent();
        }

        public MigraDoc.DocumentObjectModel.Color Color
        {
            get => 
                this.color;
            set
            {
                this.color = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.DashStyle DashStyle
        {
            get => 
                ((MigraDoc.DocumentObjectModel.Shapes.DashStyle) this.dashStyle.Value);
            set
            {
                this.dashStyle.Value = (int) value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(LineFormat));
                }
                return meta;
            }
        }

        public LineStyle Style
        {
            get => 
                ((LineStyle) this.style.Value);
            set
            {
                this.style.Value = (int) value;
            }
        }

        public bool Visible
        {
            get => 
                this.visible.Value;
            set
            {
                this.visible.Value = value;
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

