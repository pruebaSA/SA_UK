namespace MigraDoc.DocumentObjectModel.Shapes
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class Shape : DocumentObject
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.FillFormat fillFormat;
        [DV]
        internal Unit height;
        [DV]
        internal LeftPosition left;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.LineFormat lineFormat;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Shapes.RelativeHorizontal))]
        internal NEnum relativeHorizontal;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Shapes.RelativeVertical))]
        internal NEnum relativeVertical;
        [DV]
        internal TopPosition top;
        [DV]
        internal Unit width;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.WrapFormat wrapFormat;

        public Shape()
        {
            this.relativeVertical = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Shapes.RelativeVertical));
            this.relativeHorizontal = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Shapes.RelativeHorizontal));
            this.top = TopPosition.NullValue;
            this.left = LeftPosition.NullValue;
            this.height = Unit.NullValue;
            this.width = Unit.NullValue;
        }

        internal Shape(DocumentObject parent) : base(parent)
        {
            this.relativeVertical = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Shapes.RelativeVertical));
            this.relativeHorizontal = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Shapes.RelativeHorizontal));
            this.top = TopPosition.NullValue;
            this.left = LeftPosition.NullValue;
            this.height = Unit.NullValue;
            this.width = Unit.NullValue;
        }

        public Shape Clone() => 
            ((Shape) this.DeepCopy());

        protected override object DeepCopy()
        {
            Shape shape = (Shape) base.DeepCopy();
            if (shape.wrapFormat != null)
            {
                shape.wrapFormat = shape.wrapFormat.Clone();
                shape.wrapFormat.parent = shape;
            }
            if (shape.lineFormat != null)
            {
                shape.lineFormat = shape.lineFormat.Clone();
                shape.lineFormat.parent = shape;
            }
            if (shape.fillFormat != null)
            {
                shape.fillFormat = shape.fillFormat.Clone();
                shape.fillFormat.parent = shape;
            }
            return shape;
        }

        internal override void Serialize(Serializer serializer)
        {
            if (!this.height.IsNull)
            {
                serializer.WriteSimpleAttribute("Height", this.Height);
            }
            if (!this.width.IsNull)
            {
                serializer.WriteSimpleAttribute("Width", this.Width);
            }
            if (!this.relativeHorizontal.IsNull)
            {
                serializer.WriteSimpleAttribute("RelativeHorizontal", this.RelativeHorizontal);
            }
            if (!this.relativeVertical.IsNull)
            {
                serializer.WriteSimpleAttribute("RelativeVertical", this.RelativeVertical);
            }
            if (!this.IsNull("Left"))
            {
                this.left.Serialize(serializer);
            }
            if (!this.IsNull("Top"))
            {
                this.top.Serialize(serializer);
            }
            if (!this.IsNull("WrapFormat"))
            {
                this.wrapFormat.Serialize(serializer);
            }
            if (!this.IsNull("LineFormat"))
            {
                this.lineFormat.Serialize(serializer);
            }
            if (!this.IsNull("FillFormat"))
            {
                this.fillFormat.Serialize(serializer);
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.FillFormat FillFormat
        {
            get
            {
                if (this.fillFormat == null)
                {
                    this.fillFormat = new MigraDoc.DocumentObjectModel.Shapes.FillFormat(this);
                }
                return this.fillFormat;
            }
            set
            {
                base.SetParent(value);
                this.fillFormat = value;
            }
        }

        public Unit Height
        {
            get => 
                this.height;
            set
            {
                this.height = value;
            }
        }

        public LeftPosition Left
        {
            get => 
                this.left;
            set
            {
                this.left = value;
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
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Shape));
                }
                return meta;
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.RelativeHorizontal RelativeHorizontal
        {
            get => 
                ((MigraDoc.DocumentObjectModel.Shapes.RelativeHorizontal) this.relativeHorizontal.Value);
            set
            {
                this.relativeHorizontal.Value = (int) value;
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.RelativeVertical RelativeVertical
        {
            get => 
                ((MigraDoc.DocumentObjectModel.Shapes.RelativeVertical) this.relativeVertical.Value);
            set
            {
                this.relativeVertical.Value = (int) value;
            }
        }

        public TopPosition Top
        {
            get => 
                this.top;
            set
            {
                this.top = value;
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

        public MigraDoc.DocumentObjectModel.Shapes.WrapFormat WrapFormat
        {
            get
            {
                if (this.wrapFormat == null)
                {
                    this.wrapFormat = new MigraDoc.DocumentObjectModel.Shapes.WrapFormat(this);
                }
                return this.wrapFormat;
            }
            set
            {
                base.SetParent(value);
                this.wrapFormat = value;
            }
        }
    }
}

