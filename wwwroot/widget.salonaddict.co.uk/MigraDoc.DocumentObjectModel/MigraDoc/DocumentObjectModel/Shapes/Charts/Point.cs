namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using System;
    using System.Globalization;

    public class Point : ChartObject
    {
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.FillFormat fillFormat;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.LineFormat lineFormat;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NDouble value;

        internal Point()
        {
            this.value = NDouble.NullValue;
        }

        public Point(double value) : this()
        {
            this.Value = value;
        }

        public Point Clone() => 
            ((Point) this.DeepCopy());

        protected override object DeepCopy()
        {
            Point point = (Point) base.DeepCopy();
            if (point.lineFormat != null)
            {
                point.lineFormat = point.lineFormat.Clone();
                point.lineFormat.parent = point;
            }
            if (point.fillFormat != null)
            {
                point.fillFormat = point.fillFormat.Clone();
                point.fillFormat.parent = point;
            }
            return point;
        }

        internal override void Serialize(Serializer serializer)
        {
            if (!this.IsNull("LineFormat") || !this.IsNull("FillFormat"))
            {
                serializer.WriteLine("");
                serializer.WriteLine(@"\point");
                int pos = serializer.BeginAttributes();
                if (!this.IsNull("LineFormat"))
                {
                    this.lineFormat.Serialize(serializer);
                }
                if (!this.IsNull("FillFormat"))
                {
                    this.fillFormat.Serialize(serializer);
                }
                serializer.EndAttributes(pos);
                serializer.BeginContent();
                serializer.WriteLine(this.Value.ToString(CultureInfo.InvariantCulture));
                serializer.EndContent();
            }
            else
            {
                serializer.Write(this.Value.ToString(CultureInfo.InvariantCulture));
            }
            serializer.Write(", ");
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
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Point));
                }
                return meta;
            }
        }

        public double Value
        {
            get => 
                this.value.Value;
            set
            {
                this.value.Value = value;
            }
        }
    }
}

