namespace MigraDoc.DocumentObjectModel.Fields
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class SectionPagesField : NumericFieldBase
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public SectionPagesField()
        {
        }

        internal SectionPagesField(DocumentObject parent) : base(parent)
        {
        }

        public SectionPagesField Clone() => 
            ((SectionPagesField) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            string str = @"\field(SectionPages)";
            if (this.format.Value != "")
            {
                str = str + "[Format = \"" + base.Format + "\"]";
            }
            else
            {
                str = str + "[]";
            }
            serializer.Write(str);
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(SectionPagesField));
                }
                return meta;
            }
        }
    }
}

