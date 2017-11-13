namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using System;

    public class PlotArea : ChartObject
    {
        [DV]
        internal Unit bottomPadding;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.FillFormat fillFormat;
        [DV]
        internal Unit leftPadding;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.LineFormat lineFormat;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal Unit rightPadding;
        [DV]
        internal Unit topPadding;

        internal PlotArea()
        {
            this.leftPadding = Unit.NullValue;
            this.rightPadding = Unit.NullValue;
            this.topPadding = Unit.NullValue;
            this.bottomPadding = Unit.NullValue;
        }

        internal PlotArea(DocumentObject parent) : base(parent)
        {
            this.leftPadding = Unit.NullValue;
            this.rightPadding = Unit.NullValue;
            this.topPadding = Unit.NullValue;
            this.bottomPadding = Unit.NullValue;
        }

        public PlotArea Clone() => 
            ((PlotArea) this.DeepCopy());

        protected override object DeepCopy()
        {
            PlotArea area = (PlotArea) base.DeepCopy();
            if (area.lineFormat != null)
            {
                area.lineFormat = area.lineFormat.Clone();
                area.lineFormat.parent = area;
            }
            if (area.fillFormat != null)
            {
                area.fillFormat = area.fillFormat.Clone();
                area.fillFormat.parent = area;
            }
            return area;
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine(@"\plotarea");
            int pos = serializer.BeginAttributes();
            if (!this.topPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("TopPadding", this.TopPadding);
            }
            if (!this.leftPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("LeftPadding", this.LeftPadding);
            }
            if (!this.rightPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("RightPadding", this.RightPadding);
            }
            if (!this.bottomPadding.IsNull)
            {
                serializer.WriteSimpleAttribute("BottomPadding", this.BottomPadding);
            }
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
            serializer.EndContent();
        }

        public Unit BottomPadding
        {
            get => 
                this.bottomPadding;
            set
            {
                this.bottomPadding = value;
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

        public Unit LeftPadding
        {
            get => 
                this.leftPadding;
            set
            {
                this.leftPadding = value;
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
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(PlotArea));
                }
                return meta;
            }
        }

        public Unit RightPadding
        {
            get => 
                this.rightPadding;
            set
            {
                this.rightPadding = value;
            }
        }

        public Unit TopPadding
        {
            get => 
                this.topPadding;
            set
            {
                this.topPadding = value;
            }
        }
    }
}

