namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Reflection;

    public class SeriesCollection : DocumentObjectCollection
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        internal SeriesCollection()
        {
        }

        internal SeriesCollection(DocumentObject parent) : base(parent)
        {
        }

        public Series AddSeries()
        {
            Series series = new Series();
            this.Add(series);
            return series;
        }

        public SeriesCollection Clone() => 
            ((SeriesCollection) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                this[i].Serialize(serializer);
            }
        }

        public Series this[int index] =>
            (base[index] as Series);

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(SeriesCollection));
                }
                return meta;
            }
        }
    }
}

