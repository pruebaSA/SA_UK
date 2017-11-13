namespace MigraDoc.DocumentObjectModel.Shapes
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class PictureFormat : DocumentObject
    {
        [DV]
        protected Unit cropBottom;
        [DV]
        protected Unit cropLeft;
        [DV]
        protected Unit cropRight;
        [DV]
        protected Unit cropTop;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public PictureFormat()
        {
            this.cropLeft = Unit.NullValue;
            this.cropRight = Unit.NullValue;
            this.cropTop = Unit.NullValue;
            this.cropBottom = Unit.NullValue;
        }

        internal PictureFormat(DocumentObject parent) : base(parent)
        {
            this.cropLeft = Unit.NullValue;
            this.cropRight = Unit.NullValue;
            this.cropTop = Unit.NullValue;
            this.cropBottom = Unit.NullValue;
        }

        public PictureFormat Clone() => 
            ((PictureFormat) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            serializer.BeginContent("PictureFormat");
            if (!this.cropLeft.IsNull)
            {
                serializer.WriteSimpleAttribute("CropLeft", this.CropLeft);
            }
            if (!this.cropRight.IsNull)
            {
                serializer.WriteSimpleAttribute("CropRight", this.CropRight);
            }
            if (!this.cropTop.IsNull)
            {
                serializer.WriteSimpleAttribute("CropTop", this.CropTop);
            }
            if (!this.cropBottom.IsNull)
            {
                serializer.WriteSimpleAttribute("CropBottom", this.CropBottom);
            }
            serializer.EndContent();
        }

        public Unit CropBottom
        {
            get => 
                this.cropBottom;
            set
            {
                this.cropBottom = value;
            }
        }

        public Unit CropLeft
        {
            get => 
                this.cropLeft;
            set
            {
                this.cropLeft = value;
            }
        }

        public Unit CropRight
        {
            get => 
                this.cropRight;
            set
            {
                this.cropRight = value;
            }
        }

        public Unit CropTop
        {
            get => 
                this.cropTop;
            set
            {
                this.cropTop = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(PictureFormat));
                }
                return meta;
            }
        }
    }
}

