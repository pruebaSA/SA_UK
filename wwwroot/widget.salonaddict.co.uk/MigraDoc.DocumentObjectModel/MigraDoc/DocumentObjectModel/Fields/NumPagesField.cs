namespace MigraDoc.DocumentObjectModel.Fields
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class NumPagesField : NumericFieldBase
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public NumPagesField()
        {
        }

        internal NumPagesField(DocumentObject parent) : base(parent)
        {
        }

        public NumPagesField Clone() => 
            ((NumPagesField) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            string str = @"\field(NumPages)";
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
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(NumPagesField));
                }
                return meta;
            }
        }
    }
}

