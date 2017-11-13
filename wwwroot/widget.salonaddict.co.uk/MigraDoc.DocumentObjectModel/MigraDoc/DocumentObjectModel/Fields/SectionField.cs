namespace MigraDoc.DocumentObjectModel.Fields
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class SectionField : NumericFieldBase
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        internal SectionField()
        {
        }

        internal SectionField(DocumentObject parent) : base(parent)
        {
        }

        public SectionField Clone() => 
            ((SectionField) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            string str = @"\field(Section)";
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
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(SectionField));
                }
                return meta;
            }
        }
    }
}

