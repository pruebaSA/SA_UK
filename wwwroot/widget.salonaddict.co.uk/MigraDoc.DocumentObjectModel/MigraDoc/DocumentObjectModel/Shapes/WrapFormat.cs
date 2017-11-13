namespace MigraDoc.DocumentObjectModel.Shapes
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class WrapFormat : DocumentObject
    {
        [DV]
        protected Unit distanceBottom;
        [DV]
        protected Unit distanceLeft;
        [DV]
        protected Unit distanceRight;
        [DV]
        protected Unit distanceTop;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV(Type=typeof(WrapStyle))]
        internal NEnum style;

        public WrapFormat()
        {
            this.style = NEnum.NullValue(typeof(WrapStyle));
            this.distanceTop = Unit.NullValue;
            this.distanceBottom = Unit.NullValue;
            this.distanceLeft = Unit.NullValue;
            this.distanceRight = Unit.NullValue;
        }

        internal WrapFormat(DocumentObject parent) : base(parent)
        {
            this.style = NEnum.NullValue(typeof(WrapStyle));
            this.distanceTop = Unit.NullValue;
            this.distanceBottom = Unit.NullValue;
            this.distanceLeft = Unit.NullValue;
            this.distanceRight = Unit.NullValue;
        }

        public WrapFormat Clone() => 
            ((WrapFormat) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            serializer.BeginContent("WrapFormat");
            if (!this.style.IsNull)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.distanceTop.IsNull)
            {
                serializer.WriteSimpleAttribute("DistanceTop", this.DistanceTop);
            }
            if (!this.distanceLeft.IsNull)
            {
                serializer.WriteSimpleAttribute("DistanceLeft", this.DistanceLeft);
            }
            if (!this.distanceRight.IsNull)
            {
                serializer.WriteSimpleAttribute("DistanceRight", this.DistanceRight);
            }
            if (!this.distanceBottom.IsNull)
            {
                serializer.WriteSimpleAttribute("DistanceBottom", this.DistanceBottom);
            }
            serializer.EndContent();
        }

        public Unit DistanceBottom
        {
            get => 
                this.distanceBottom;
            set
            {
                this.distanceBottom = value;
            }
        }

        public Unit DistanceLeft
        {
            get => 
                this.distanceLeft;
            set
            {
                this.distanceLeft = value;
            }
        }

        public Unit DistanceRight
        {
            get => 
                this.distanceRight;
            set
            {
                this.distanceRight = value;
            }
        }

        public Unit DistanceTop
        {
            get => 
                this.distanceTop;
            set
            {
                this.distanceTop = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(WrapFormat));
                }
                return meta;
            }
        }

        public WrapStyle Style
        {
            get => 
                ((WrapStyle) this.style.Value);
            set
            {
                this.style.Value = (int) value;
            }
        }
    }
}

