namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class XSeries : ChartObject
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        protected XSeriesElements xSeriesElements = new XSeriesElements();

        public XValue Add(string value) => 
            this.xSeriesElements.Add(value);

        public void Add(params string[] values)
        {
            this.xSeriesElements.Add(values);
        }

        public void AddBlank()
        {
            this.xSeriesElements.AddBlank();
        }

        public XSeries Clone() => 
            ((XSeries) this.DeepCopy());

        protected override object DeepCopy()
        {
            XSeries series = (XSeries) base.DeepCopy();
            if (series.xSeriesElements != null)
            {
                series.xSeriesElements = series.xSeriesElements.Clone();
                series.xSeriesElements.parent = series;
            }
            return series;
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine(@"\xvalues");
            serializer.BeginContent();
            this.xSeriesElements.Serialize(serializer);
            serializer.WriteLine("");
            serializer.EndContent();
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(XSeries));
                }
                return meta;
            }
        }
    }
}

