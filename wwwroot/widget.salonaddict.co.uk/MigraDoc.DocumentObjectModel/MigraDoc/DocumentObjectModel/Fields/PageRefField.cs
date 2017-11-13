namespace MigraDoc.DocumentObjectModel.Fields
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class PageRefField : NumericFieldBase
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString name;

        internal PageRefField()
        {
            this.name = NString.NullValue;
        }

        internal PageRefField(DocumentObject parent) : base(parent)
        {
            this.name = NString.NullValue;
        }

        public PageRefField(string name) : this()
        {
            this.Name = name;
        }

        public PageRefField Clone() => 
            ((PageRefField) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            string str = @"\field(PageRef)";
            str = str + "[Name = \"" + this.Name + "\"";
            if (this.format.Value != "")
            {
                str = str + " Format = \"" + base.Format + "\"";
            }
            str = str + "]";
            serializer.Write(str);
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(PageRefField));
                }
                return meta;
            }
        }

        public string Name
        {
            get => 
                this.name.Value;
            set
            {
                this.name.Value = value;
            }
        }
    }
}

