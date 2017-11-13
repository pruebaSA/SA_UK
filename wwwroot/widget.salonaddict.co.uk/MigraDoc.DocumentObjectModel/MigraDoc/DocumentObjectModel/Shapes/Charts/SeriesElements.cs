namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class SeriesElements : DocumentObjectCollection
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        internal SeriesElements()
        {
        }

        internal SeriesElements(DocumentObject parent) : base(parent)
        {
        }

        public Point Add(double value)
        {
            Point point = new Point(value);
            this.Add(point);
            return point;
        }

        public void Add(params double[] values)
        {
            foreach (double num in values)
            {
                this.Add(num);
            }
        }

        public void AddBlank()
        {
            base.Add(null);
        }

        public SeriesElements Clone() => 
            ((SeriesElements) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                Point point = this[i] as Point;
                if (point == null)
                {
                    serializer.Write("null, ");
                }
                else
                {
                    point.Serialize(serializer);
                }
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(SeriesElements));
                }
                return meta;
            }
        }
    }
}

