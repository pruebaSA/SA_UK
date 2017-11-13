namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Reflection;

    public class XValues : DocumentObjectCollection
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public XValues()
        {
        }

        internal XValues(DocumentObject parent) : base(parent)
        {
        }

        public XSeries AddXSeries()
        {
            XSeries series = new XSeries();
            this.Add(series);
            return series;
        }

        public XValues Clone() => 
            ((XValues) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                this[i].Serialize(serializer);
            }
        }

        public XSeries this[int index] =>
            (base[index] as XSeries);

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(XValues));
                }
                return meta;
            }
        }
    }
}

