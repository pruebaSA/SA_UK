namespace MigraDoc.DocumentObjectModel.Shapes
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class Barcode : Shape
    {
        [DV]
        internal NBool bearerBars;
        [DV]
        internal NString code;
        [DV]
        internal NDouble lineHeight;
        [DV]
        internal NDouble lineRatio;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NDouble narrowLineWidth;
        [DV(Type=typeof(TextOrientation))]
        internal NEnum orientation;
        [DV]
        internal NBool text;
        [DV(Type=typeof(BarcodeType))]
        internal NEnum type;

        internal Barcode()
        {
            this.orientation = NEnum.NullValue(typeof(TextOrientation));
            this.type = NEnum.NullValue(typeof(BarcodeType));
            this.bearerBars = NBool.NullValue;
            this.text = NBool.NullValue;
            this.code = NString.NullValue;
            this.lineRatio = NDouble.NullValue;
            this.lineHeight = NDouble.NullValue;
            this.narrowLineWidth = NDouble.NullValue;
        }

        internal Barcode(DocumentObject parent) : base(parent)
        {
            this.orientation = NEnum.NullValue(typeof(TextOrientation));
            this.type = NEnum.NullValue(typeof(BarcodeType));
            this.bearerBars = NBool.NullValue;
            this.text = NBool.NullValue;
            this.code = NString.NullValue;
            this.lineRatio = NDouble.NullValue;
            this.lineHeight = NDouble.NullValue;
            this.narrowLineWidth = NDouble.NullValue;
        }

        public Barcode Clone() => 
            ((Barcode) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            if (this.code.Value == "")
            {
                throw new InvalidOperationException(DomSR.MissingObligatoryProperty("Name", "BookmarkField"));
            }
            serializer.WriteLine("\\barcode(\"" + this.Code + "\")");
            int pos = serializer.BeginAttributes();
            base.Serialize(serializer);
            if (!this.orientation.IsNull)
            {
                serializer.WriteSimpleAttribute("Orientation", this.Orientation);
            }
            if (!this.bearerBars.IsNull)
            {
                serializer.WriteSimpleAttribute("BearerBars", this.BearerBars);
            }
            if (!this.text.IsNull)
            {
                serializer.WriteSimpleAttribute("Text", this.Text);
            }
            if (!this.type.IsNull)
            {
                serializer.WriteSimpleAttribute("Type", this.Type);
            }
            if (!this.lineRatio.IsNull)
            {
                serializer.WriteSimpleAttribute("LineRatio", this.LineRatio);
            }
            if (!this.lineHeight.IsNull)
            {
                serializer.WriteSimpleAttribute("LineHeight", this.LineHeight);
            }
            if (!this.narrowLineWidth.IsNull)
            {
                serializer.WriteSimpleAttribute("NarrowLineWidth", this.NarrowLineWidth);
            }
            serializer.EndAttributes(pos);
        }

        public bool BearerBars
        {
            get => 
                this.bearerBars.Value;
            set
            {
                this.bearerBars.Value = value;
            }
        }

        public string Code
        {
            get => 
                this.code.Value;
            set
            {
                this.code.Value = value;
            }
        }

        public double LineHeight
        {
            get => 
                this.lineHeight.Value;
            set
            {
                this.lineHeight.Value = value;
            }
        }

        public double LineRatio
        {
            get => 
                this.lineRatio.Value;
            set
            {
                this.lineRatio.Value = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Barcode));
                }
                return meta;
            }
        }

        public double NarrowLineWidth
        {
            get => 
                this.narrowLineWidth.Value;
            set
            {
                this.narrowLineWidth.Value = value;
            }
        }

        public TextOrientation Orientation
        {
            get => 
                ((TextOrientation) this.orientation.Value);
            set
            {
                this.orientation.Value = (int) value;
            }
        }

        public bool Text
        {
            get => 
                this.text.Value;
            set
            {
                this.text.Value = value;
            }
        }

        public BarcodeType Type
        {
            get => 
                ((BarcodeType) this.type.Value);
            set
            {
                this.type.Value = (int) value;
            }
        }
    }
}

