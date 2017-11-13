namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class XSeriesElements : DocumentObjectCollection
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public XValue Add(string value)
        {
            XValue value2 = new XValue(value);
            this.Add(value2);
            return value2;
        }

        public void Add(params string[] values)
        {
            foreach (string str in values)
            {
                this.Add(str);
            }
        }

        public void AddBlank()
        {
            base.Add(null);
        }

        public XSeriesElements Clone() => 
            ((XSeriesElements) base.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                XValue value2 = this[i] as XValue;
                if (value2 == null)
                {
                    serializer.Write("null, ");
                }
                else
                {
                    value2.Serialize(serializer);
                }
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(XSeriesElements));
                }
                return meta;
            }
        }
    }
}

