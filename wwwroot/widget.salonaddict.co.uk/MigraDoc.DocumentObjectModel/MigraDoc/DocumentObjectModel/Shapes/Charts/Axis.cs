namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using System;

    public class Axis : ChartObject
    {
        [DV]
        internal NBool hasMajorGridlines;
        [DV]
        internal NBool hasMinorGridlines;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.LineFormat lineFormat;
        [DV]
        internal Gridlines majorGridlines;
        [DV]
        internal NDouble majorTick;
        [DV(Type=typeof(TickMarkType))]
        internal NEnum majorTickMark;
        [DV]
        internal NDouble maximumScale;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NDouble minimumScale;
        [DV]
        internal Gridlines minorGridlines;
        [DV]
        internal NDouble minorTick;
        [DV(Type=typeof(TickMarkType))]
        internal NEnum minorTickMark;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.Charts.TickLabels tickLabels;
        [DV]
        internal AxisTitle title;

        public Axis()
        {
            this.minimumScale = NDouble.NullValue;
            this.maximumScale = NDouble.NullValue;
            this.majorTick = NDouble.NullValue;
            this.minorTick = NDouble.NullValue;
            this.majorTickMark = NEnum.NullValue(typeof(TickMarkType));
            this.minorTickMark = NEnum.NullValue(typeof(TickMarkType));
            this.hasMajorGridlines = NBool.NullValue;
            this.hasMinorGridlines = NBool.NullValue;
        }

        internal Axis(DocumentObject parent) : base(parent)
        {
            this.minimumScale = NDouble.NullValue;
            this.maximumScale = NDouble.NullValue;
            this.majorTick = NDouble.NullValue;
            this.minorTick = NDouble.NullValue;
            this.majorTickMark = NEnum.NullValue(typeof(TickMarkType));
            this.minorTickMark = NEnum.NullValue(typeof(TickMarkType));
            this.hasMajorGridlines = NBool.NullValue;
            this.hasMinorGridlines = NBool.NullValue;
        }

        internal string CheckGridlines(Gridlines gridlines)
        {
            if ((this.majorGridlines != null) && (gridlines == this.majorGridlines))
            {
                return "MajorGridlines";
            }
            if ((this.minorGridlines != null) && (gridlines == this.minorGridlines))
            {
                return "MinorGridlines";
            }
            return "";
        }

        public Axis Clone() => 
            ((Axis) this.DeepCopy());

        protected override object DeepCopy()
        {
            Axis axis = (Axis) base.DeepCopy();
            if (axis.title != null)
            {
                axis.title = axis.title.Clone();
                axis.title.parent = axis;
            }
            if (axis.tickLabels != null)
            {
                axis.tickLabels = axis.tickLabels.Clone();
                axis.tickLabels.parent = axis;
            }
            if (axis.lineFormat != null)
            {
                axis.lineFormat = axis.lineFormat.Clone();
                axis.lineFormat.parent = axis;
            }
            if (axis.majorGridlines != null)
            {
                axis.majorGridlines = axis.majorGridlines.Clone();
                axis.majorGridlines.parent = axis;
            }
            if (axis.minorGridlines != null)
            {
                axis.minorGridlines = axis.minorGridlines.Clone();
                axis.minorGridlines.parent = axis;
            }
            return axis;
        }

        internal override void Serialize(Serializer serializer)
        {
            Chart parent = base.parent as Chart;
            serializer.WriteLine(@"\" + parent.CheckAxis(this));
            int pos = serializer.BeginAttributes();
            if (!this.minimumScale.IsNull)
            {
                serializer.WriteSimpleAttribute("MinimumScale", this.MinimumScale);
            }
            if (!this.maximumScale.IsNull)
            {
                serializer.WriteSimpleAttribute("MaximumScale", this.MaximumScale);
            }
            if (!this.majorTick.IsNull)
            {
                serializer.WriteSimpleAttribute("MajorTick", this.MajorTick);
            }
            if (!this.minorTick.IsNull)
            {
                serializer.WriteSimpleAttribute("MinorTick", this.MinorTick);
            }
            if (!this.hasMajorGridlines.IsNull)
            {
                serializer.WriteSimpleAttribute("HasMajorGridLines", this.HasMajorGridlines);
            }
            if (!this.hasMinorGridlines.IsNull)
            {
                serializer.WriteSimpleAttribute("HasMinorGridLines", this.HasMinorGridlines);
            }
            if (!this.majorTickMark.IsNull)
            {
                serializer.WriteSimpleAttribute("MajorTickMark", this.MajorTickMark);
            }
            if (!this.minorTickMark.IsNull)
            {
                serializer.WriteSimpleAttribute("MinorTickMark", this.MinorTickMark);
            }
            if (!this.IsNull("Title"))
            {
                this.title.Serialize(serializer);
            }
            if (!this.IsNull("LineFormat"))
            {
                this.lineFormat.Serialize(serializer);
            }
            if (!this.IsNull("MajorGridlines"))
            {
                this.majorGridlines.Serialize(serializer);
            }
            if (!this.IsNull("MinorGridlines"))
            {
                this.minorGridlines.Serialize(serializer);
            }
            if (!this.IsNull("TickLabels"))
            {
                this.tickLabels.Serialize(serializer);
            }
            serializer.EndAttributes(pos);
        }

        public bool HasMajorGridlines
        {
            get => 
                this.hasMajorGridlines.Value;
            set
            {
                this.hasMajorGridlines.Value = value;
            }
        }

        public bool HasMinorGridlines
        {
            get => 
                this.hasMinorGridlines.Value;
            set
            {
                this.hasMinorGridlines.Value = value;
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

        public Gridlines MajorGridlines
        {
            get
            {
                if (this.majorGridlines == null)
                {
                    this.majorGridlines = new Gridlines(this);
                }
                return this.majorGridlines;
            }
            set
            {
                base.SetParent(value);
                this.majorGridlines = value;
            }
        }

        public double MajorTick
        {
            get => 
                this.majorTick.Value;
            set
            {
                this.majorTick.Value = value;
            }
        }

        public TickMarkType MajorTickMark
        {
            get => 
                ((TickMarkType) this.majorTickMark.Value);
            set
            {
                this.majorTickMark.Value = (int) value;
            }
        }

        public double MaximumScale
        {
            get => 
                this.maximumScale.Value;
            set
            {
                this.maximumScale.Value = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Axis));
                }
                return meta;
            }
        }

        public double MinimumScale
        {
            get => 
                this.minimumScale.Value;
            set
            {
                this.minimumScale.Value = value;
            }
        }

        public Gridlines MinorGridlines
        {
            get
            {
                if (this.minorGridlines == null)
                {
                    this.minorGridlines = new Gridlines(this);
                }
                return this.minorGridlines;
            }
            set
            {
                base.SetParent(value);
                this.minorGridlines = value;
            }
        }

        public double MinorTick
        {
            get => 
                this.minorTick.Value;
            set
            {
                this.minorTick.Value = value;
            }
        }

        public TickMarkType MinorTickMark
        {
            get => 
                ((TickMarkType) this.minorTickMark.Value);
            set
            {
                this.minorTickMark.Value = (int) value;
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.Charts.TickLabels TickLabels
        {
            get
            {
                if (this.tickLabels == null)
                {
                    this.tickLabels = new MigraDoc.DocumentObjectModel.Shapes.Charts.TickLabels(this);
                }
                return this.tickLabels;
            }
            set
            {
                base.SetParent(value);
                this.tickLabels = value;
            }
        }

        public AxisTitle Title
        {
            get
            {
                if (this.title == null)
                {
                    this.title = new AxisTitle(this);
                }
                return this.title;
            }
            set
            {
                base.SetParent(value);
                this.title = value;
            }
        }
    }
}

