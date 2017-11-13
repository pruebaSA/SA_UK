namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public class Chart : Shape, IVisitable
    {
        [DV]
        internal TextArea bottomArea;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.Charts.DataLabel dataLabel;
        [DV(Type=typeof(BlankType))]
        internal NEnum displayBlanksAs;
        [DV]
        internal TextArea footerArea;
        [DV]
        internal ParagraphFormat format;
        [DV]
        internal NBool hasDataLabel;
        [DV]
        internal TextArea headerArea;
        [DV]
        internal TextArea leftArea;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NBool pivotChart;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.Charts.PlotArea plotArea;
        [DV]
        internal TextArea rightArea;
        [DV(ItemType=typeof(Series))]
        internal MigraDoc.DocumentObjectModel.Shapes.Charts.SeriesCollection seriesCollection;
        [DV]
        internal NString style;
        [DV]
        internal TextArea topArea;
        [DV(Type=typeof(ChartType))]
        internal NEnum type;
        [DV]
        internal Axis xAxis;
        [DV(ItemType=typeof(Series))]
        internal MigraDoc.DocumentObjectModel.Shapes.Charts.XValues xValues;
        [DV]
        internal Axis yAxis;
        [DV]
        internal Axis zAxis;

        public Chart()
        {
            this.type = NEnum.NullValue(typeof(ChartType));
            this.style = NString.NullValue;
            this.displayBlanksAs = NEnum.NullValue(typeof(BlankType));
            this.pivotChart = NBool.NullValue;
            this.hasDataLabel = NBool.NullValue;
        }

        internal Chart(DocumentObject parent) : base(parent)
        {
            this.type = NEnum.NullValue(typeof(ChartType));
            this.style = NString.NullValue;
            this.displayBlanksAs = NEnum.NullValue(typeof(BlankType));
            this.pivotChart = NBool.NullValue;
            this.hasDataLabel = NBool.NullValue;
        }

        public Chart(ChartType type) : this()
        {
            this.Type = type;
        }

        internal string CheckAxis(Axis axis)
        {
            if ((this.xAxis != null) && (axis == this.xAxis))
            {
                return "xaxis";
            }
            if ((this.yAxis != null) && (axis == this.yAxis))
            {
                return "yaxis";
            }
            if ((this.zAxis != null) && (axis == this.zAxis))
            {
                return "zaxis";
            }
            return "";
        }

        internal string CheckTextArea(TextArea textArea)
        {
            if ((this.headerArea != null) && (textArea == this.headerArea))
            {
                return "headerarea";
            }
            if ((this.footerArea != null) && (textArea == this.footerArea))
            {
                return "footerarea";
            }
            if ((this.leftArea != null) && (textArea == this.leftArea))
            {
                return "leftarea";
            }
            if ((this.rightArea != null) && (textArea == this.rightArea))
            {
                return "rightarea";
            }
            if ((this.topArea != null) && (textArea == this.topArea))
            {
                return "toparea";
            }
            if ((this.bottomArea != null) && (textArea == this.bottomArea))
            {
                return "bottomarea";
            }
            return "";
        }

        public Chart Clone() => 
            ((Chart) this.DeepCopy());

        protected override object DeepCopy()
        {
            Chart chart = (Chart) base.DeepCopy();
            if (chart.format != null)
            {
                chart.format = chart.format.Clone();
                chart.format.parent = chart;
            }
            if (chart.xAxis != null)
            {
                chart.xAxis = chart.xAxis.Clone();
                chart.xAxis.parent = chart;
            }
            if (chart.yAxis != null)
            {
                chart.yAxis = chart.yAxis.Clone();
                chart.yAxis.parent = chart;
            }
            if (chart.zAxis != null)
            {
                chart.zAxis = chart.zAxis.Clone();
                chart.zAxis.parent = chart;
            }
            if (chart.seriesCollection != null)
            {
                chart.seriesCollection = chart.seriesCollection.Clone();
                chart.seriesCollection.parent = chart;
            }
            if (chart.xValues != null)
            {
                chart.xValues = chart.xValues.Clone();
                chart.xValues.parent = chart;
            }
            if (chart.headerArea != null)
            {
                chart.headerArea = chart.headerArea.Clone();
                chart.headerArea.parent = chart;
            }
            if (chart.bottomArea != null)
            {
                chart.bottomArea = chart.bottomArea.Clone();
                chart.bottomArea.parent = chart;
            }
            if (chart.topArea != null)
            {
                chart.topArea = chart.topArea.Clone();
                chart.topArea.parent = chart;
            }
            if (chart.footerArea != null)
            {
                chart.footerArea = chart.footerArea.Clone();
                chart.footerArea.parent = chart;
            }
            if (chart.leftArea != null)
            {
                chart.leftArea = chart.leftArea.Clone();
                chart.leftArea.parent = chart;
            }
            if (chart.rightArea != null)
            {
                chart.rightArea = chart.rightArea.Clone();
                chart.rightArea.parent = chart;
            }
            if (chart.plotArea != null)
            {
                chart.plotArea = chart.plotArea.Clone();
                chart.plotArea.parent = chart;
            }
            if (chart.dataLabel != null)
            {
                chart.dataLabel = chart.dataLabel.Clone();
                chart.dataLabel.parent = chart;
            }
            return chart;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitChart(this);
            if (visitChildren)
            {
                if (this.bottomArea != null)
                {
                    ((IVisitable) this.bottomArea).AcceptVisitor(visitor, visitChildren);
                }
                if (this.footerArea != null)
                {
                    ((IVisitable) this.footerArea).AcceptVisitor(visitor, visitChildren);
                }
                if (this.headerArea != null)
                {
                    ((IVisitable) this.headerArea).AcceptVisitor(visitor, visitChildren);
                }
                if (this.leftArea != null)
                {
                    ((IVisitable) this.leftArea).AcceptVisitor(visitor, visitChildren);
                }
                if (this.rightArea != null)
                {
                    ((IVisitable) this.rightArea).AcceptVisitor(visitor, visitChildren);
                }
                if (this.topArea != null)
                {
                    ((IVisitable) this.topArea).AcceptVisitor(visitor, visitChildren);
                }
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine(@"\chart(" + this.Type + ")");
            int pos = serializer.BeginAttributes();
            base.Serialize(serializer);
            if (!this.displayBlanksAs.IsNull)
            {
                serializer.WriteSimpleAttribute("DisplayBlanksAs", this.DisplayBlanksAs);
            }
            if (!this.pivotChart.IsNull)
            {
                serializer.WriteSimpleAttribute("PivotChart", this.PivotChart);
            }
            if (!this.hasDataLabel.IsNull)
            {
                serializer.WriteSimpleAttribute("HasDataLabel", this.HasDataLabel);
            }
            if (!this.style.IsNull)
            {
                serializer.WriteSimpleAttribute("Style", this.Style);
            }
            if (!this.IsNull("Format"))
            {
                this.format.Serialize(serializer, "Format", null);
            }
            if (!this.IsNull("DataLabel"))
            {
                this.dataLabel.Serialize(serializer);
            }
            serializer.EndAttributes(pos);
            serializer.BeginContent();
            if (!this.IsNull("PlotArea"))
            {
                this.plotArea.Serialize(serializer);
            }
            if (!this.IsNull("HeaderArea"))
            {
                this.headerArea.Serialize(serializer);
            }
            if (!this.IsNull("FooterArea"))
            {
                this.footerArea.Serialize(serializer);
            }
            if (!this.IsNull("TopArea"))
            {
                this.topArea.Serialize(serializer);
            }
            if (!this.IsNull("BottomArea"))
            {
                this.bottomArea.Serialize(serializer);
            }
            if (!this.IsNull("LeftArea"))
            {
                this.leftArea.Serialize(serializer);
            }
            if (!this.IsNull("RightArea"))
            {
                this.rightArea.Serialize(serializer);
            }
            if (!this.IsNull("XAxis"))
            {
                this.xAxis.Serialize(serializer);
            }
            if (!this.IsNull("YAxis"))
            {
                this.yAxis.Serialize(serializer);
            }
            if (!this.IsNull("ZAxis"))
            {
                this.zAxis.Serialize(serializer);
            }
            if (!this.IsNull("SeriesCollection"))
            {
                this.seriesCollection.Serialize(serializer);
            }
            if (!this.IsNull("XValues"))
            {
                this.xValues.Serialize(serializer);
            }
            serializer.EndContent();
        }

        public TextArea BottomArea
        {
            get
            {
                if (this.bottomArea == null)
                {
                    this.bottomArea = new TextArea(this);
                }
                return this.bottomArea;
            }
            set
            {
                base.SetParent(value);
                this.bottomArea = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.Charts.DataLabel DataLabel
        {
            get
            {
                if (this.dataLabel == null)
                {
                    this.dataLabel = new MigraDoc.DocumentObjectModel.Shapes.Charts.DataLabel(this);
                }
                return this.dataLabel;
            }
            set
            {
                base.SetParent(value);
                this.dataLabel = value;
            }
        }

        public BlankType DisplayBlanksAs
        {
            get => 
                ((BlankType) this.displayBlanksAs.Value);
            set
            {
                this.displayBlanksAs.Value = (int) value;
            }
        }

        public TextArea FooterArea
        {
            get
            {
                if (this.footerArea == null)
                {
                    this.footerArea = new TextArea(this);
                }
                return this.footerArea;
            }
            set
            {
                base.SetParent(value);
                this.footerArea = value;
            }
        }

        public ParagraphFormat Format
        {
            get
            {
                if (this.format == null)
                {
                    this.format = new ParagraphFormat(this);
                }
                return this.format;
            }
            set
            {
                base.SetParent(value);
                this.format = value;
            }
        }

        public bool HasDataLabel
        {
            get => 
                this.hasDataLabel.Value;
            set
            {
                this.hasDataLabel.Value = value;
            }
        }

        public TextArea HeaderArea
        {
            get
            {
                if (this.headerArea == null)
                {
                    this.headerArea = new TextArea(this);
                }
                return this.headerArea;
            }
            set
            {
                base.SetParent(value);
                this.headerArea = value;
            }
        }

        public TextArea LeftArea
        {
            get
            {
                if (this.leftArea == null)
                {
                    this.leftArea = new TextArea(this);
                }
                return this.leftArea;
            }
            set
            {
                base.SetParent(value);
                this.leftArea = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Chart));
                }
                return meta;
            }
        }

        public bool PivotChart
        {
            get => 
                this.pivotChart.Value;
            set
            {
                this.pivotChart.Value = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.Charts.PlotArea PlotArea
        {
            get
            {
                if (this.plotArea == null)
                {
                    this.plotArea = new MigraDoc.DocumentObjectModel.Shapes.Charts.PlotArea(this);
                }
                return this.plotArea;
            }
            set
            {
                base.SetParent(value);
                this.plotArea = value;
            }
        }

        public TextArea RightArea
        {
            get
            {
                if (this.rightArea == null)
                {
                    this.rightArea = new TextArea(this);
                }
                return this.rightArea;
            }
            set
            {
                base.SetParent(value);
                this.rightArea = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.Charts.SeriesCollection SeriesCollection
        {
            get
            {
                if (this.seriesCollection == null)
                {
                    this.seriesCollection = new MigraDoc.DocumentObjectModel.Shapes.Charts.SeriesCollection(this);
                }
                return this.seriesCollection;
            }
            set
            {
                base.SetParent(value);
                this.seriesCollection = value;
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

        public TextArea TopArea
        {
            get
            {
                if (this.topArea == null)
                {
                    this.topArea = new TextArea(this);
                }
                return this.topArea;
            }
            set
            {
                base.SetParent(value);
                this.topArea = value;
            }
        }

        public ChartType Type
        {
            get => 
                ((ChartType) this.type.Value);
            set
            {
                this.type.Value = (int) value;
            }
        }

        public Axis XAxis
        {
            get
            {
                if (this.xAxis == null)
                {
                    this.xAxis = new Axis(this);
                }
                return this.xAxis;
            }
            set
            {
                base.SetParent(value);
                this.xAxis = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.Charts.XValues XValues
        {
            get
            {
                if (this.xValues == null)
                {
                    this.xValues = new MigraDoc.DocumentObjectModel.Shapes.Charts.XValues(this);
                }
                return this.xValues;
            }
            set
            {
                base.SetParent(value);
                this.xValues = value;
            }
        }

        public Axis YAxis
        {
            get
            {
                if (this.yAxis == null)
                {
                    this.yAxis = new Axis(this);
                }
                return this.yAxis;
            }
            set
            {
                base.SetParent(value);
                this.yAxis = value;
            }
        }

        public Axis ZAxis
        {
            get
            {
                if (this.zAxis == null)
                {
                    this.zAxis = new Axis(this);
                }
                return this.zAxis;
            }
            set
            {
                base.SetParent(value);
                this.zAxis = value;
            }
        }
    }
}

