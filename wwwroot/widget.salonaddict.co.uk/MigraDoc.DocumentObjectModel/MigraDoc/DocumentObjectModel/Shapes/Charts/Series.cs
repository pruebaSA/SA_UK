namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using System;

    public class Series : ChartObject
    {
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Shapes.Charts.ChartType))]
        internal NEnum chartType = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Shapes.Charts.ChartType));
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.Charts.DataLabel dataLabel;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.FillFormat fillFormat;
        [DV]
        internal NBool hasDataLabel = NBool.NullValue;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.LineFormat lineFormat;
        [DV]
        internal Color markerBackgroundColor = Color.Empty;
        [DV]
        internal Color markerForegroundColor = Color.Empty;
        [DV]
        internal Unit markerSize = Unit.NullValue;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Shapes.Charts.MarkerStyle))]
        internal NEnum markerStyle = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Shapes.Charts.MarkerStyle));
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString name = NString.NullValue;
        [DV]
        internal SeriesElements seriesElements;

        public Point Add(double value) => 
            this.Elements.Add(value);

        public void Add(params double[] values)
        {
            this.Elements.Add(values);
        }

        public void AddBlank()
        {
            this.Elements.AddBlank();
        }

        public Series Clone() => 
            ((Series) this.DeepCopy());

        protected override object DeepCopy()
        {
            Series series = (Series) base.DeepCopy();
            if (series.seriesElements != null)
            {
                series.seriesElements = series.seriesElements.Clone();
                series.seriesElements.parent = series;
            }
            if (series.lineFormat != null)
            {
                series.lineFormat = series.lineFormat.Clone();
                series.lineFormat.parent = series;
            }
            if (series.fillFormat != null)
            {
                series.fillFormat = series.fillFormat.Clone();
                series.fillFormat.parent = series;
            }
            if (series.dataLabel != null)
            {
                series.dataLabel = series.dataLabel.Clone();
                series.dataLabel.parent = series;
            }
            return series;
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine(@"\series");
            int pos = serializer.BeginAttributes();
            if (!this.name.IsNull)
            {
                serializer.WriteSimpleAttribute("Name", this.Name);
            }
            if (!this.markerSize.IsNull)
            {
                serializer.WriteSimpleAttribute("MarkerSize", this.MarkerSize);
            }
            if (!this.markerStyle.IsNull)
            {
                serializer.WriteSimpleAttribute("MarkerStyle", this.MarkerStyle);
            }
            if (!this.markerBackgroundColor.IsNull)
            {
                serializer.WriteSimpleAttribute("MarkerBackgroundColor", this.MarkerBackgroundColor);
            }
            if (!this.markerForegroundColor.IsNull)
            {
                serializer.WriteSimpleAttribute("MarkerForegroundColor", this.MarkerForegroundColor);
            }
            if (!this.chartType.IsNull)
            {
                serializer.WriteSimpleAttribute("ChartType", this.ChartType);
            }
            if (!this.hasDataLabel.IsNull)
            {
                serializer.WriteSimpleAttribute("HasDataLabel", this.HasDataLabel);
            }
            if (!this.IsNull("LineFormat"))
            {
                this.lineFormat.Serialize(serializer);
            }
            if (!this.IsNull("FillFormat"))
            {
                this.fillFormat.Serialize(serializer);
            }
            if (!this.IsNull("DataLabel"))
            {
                this.dataLabel.Serialize(serializer);
            }
            serializer.EndAttributes(pos);
            serializer.BeginContent();
            this.seriesElements.Serialize(serializer);
            serializer.WriteLine("");
            serializer.EndContent();
        }

        public MigraDoc.DocumentObjectModel.Shapes.Charts.ChartType ChartType
        {
            get => 
                ((MigraDoc.DocumentObjectModel.Shapes.Charts.ChartType) this.chartType.Value);
            set
            {
                this.chartType.Value = (int) value;
            }
        }

        public int Count
        {
            get
            {
                if (this.seriesElements != null)
                {
                    return this.seriesElements.Count;
                }
                return 0;
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

        public SeriesElements Elements
        {
            get
            {
                if (this.seriesElements == null)
                {
                    this.seriesElements = new SeriesElements(this);
                }
                return this.seriesElements;
            }
            set
            {
                base.SetParent(value);
                this.seriesElements = value;
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

        public bool HasDataLabel
        {
            get => 
                this.hasDataLabel.Value;
            set
            {
                this.hasDataLabel.Value = value;
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

        public Color MarkerBackgroundColor
        {
            get => 
                this.markerBackgroundColor;
            set
            {
                this.markerBackgroundColor = value;
            }
        }

        public Color MarkerForegroundColor
        {
            get => 
                this.markerForegroundColor;
            set
            {
                this.markerForegroundColor = value;
            }
        }

        public Unit MarkerSize
        {
            get => 
                this.markerSize;
            set
            {
                this.markerSize = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.Charts.MarkerStyle MarkerStyle
        {
            get => 
                ((MigraDoc.DocumentObjectModel.Shapes.Charts.MarkerStyle) this.markerStyle.Value);
            set
            {
                this.markerStyle.Value = (int) value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Series));
                }
                return meta;
            }
        }

        public string Name
        {
            get => 
                this.name.Value;
            set
            {
                this.name.Value = value;
            }
        }
    }
}

