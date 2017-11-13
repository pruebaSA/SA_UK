namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class PageBreak : DocumentObject
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public PageBreak()
        {
        }

        internal PageBreak(DocumentObject parent) : base(parent)
        {
        }

        public PageBreak Clone() => 
            ((PageBreak) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine(@"\pagebreak");
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(PageBreak));
                }
                return meta;
            }
        }
    }
}

