namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class XValue : ChartObject
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        protected string Value;

        internal XValue()
        {
        }

        public XValue(string value) : this()
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.Value = value;
        }

        public XValue Clone() => 
            ((XValue) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            serializer.Write("\"" + this.Value + "\", ");
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(XValue));
                }
                return meta;
            }
        }
    }
}

