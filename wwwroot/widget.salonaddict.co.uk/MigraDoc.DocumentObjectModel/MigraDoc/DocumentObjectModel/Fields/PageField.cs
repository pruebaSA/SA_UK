namespace MigraDoc.DocumentObjectModel.Fields
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class PageField : NumericFieldBase
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public PageField()
        {
        }

        internal PageField(DocumentObject parent) : base(parent)
        {
        }

        public PageField Clone() => 
            ((PageField) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            string str = @"\field(Page)";
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
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(PageField));
                }
                return meta;
            }
        }
    }
}

