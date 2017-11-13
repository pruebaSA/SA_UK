namespace MigraDoc.DocumentObjectModel.Fields
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class DateField : DocumentObject
    {
        [DV]
        internal NString format;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public DateField()
        {
            this.format = NString.NullValue;
        }

        internal DateField(DocumentObject parent) : base(parent)
        {
            this.format = NString.NullValue;
        }

        public DateField Clone() => 
            ((DateField) this.DeepCopy());

        public override bool IsNull() => 
            false;

        internal override void Serialize(Serializer serializer)
        {
            string str = @"\field(Date)";
            if (this.format.Value != string.Empty)
            {
                str = str + "[Format = \"" + this.Format + "\"]";
            }
            else
            {
                str = str + "[]";
            }
            serializer.Write(str);
        }

        public string Format
        {
            get => 
                this.format.Value;
            set
            {
                this.format.Value = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(DateField));
                }
                return meta;
            }
        }
    }
}

