namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class ChartObject : DocumentObject
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public ChartObject()
        {
        }

        internal ChartObject(DocumentObject parent) : base(parent)
        {
        }

        internal override void Serialize(Serializer _serializer)
        {
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(ChartObject));
                }
                return meta;
            }
        }
    }
}

