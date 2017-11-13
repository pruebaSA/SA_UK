namespace MigraDoc.DocumentObjectModel.Fields
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class InfoField : DocumentObject
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString name;
        private static string[] validNames = Enum.GetNames(typeof(InfoFieldType));

        internal InfoField()
        {
            this.name = NString.NullValue;
        }

        internal InfoField(DocumentObject parent) : base(parent)
        {
            this.name = NString.NullValue;
        }

        public InfoField Clone() => 
            ((InfoField) this.DeepCopy());

        public override bool IsNull() => 
            false;

        private bool IsValidName(string name)
        {
            foreach (string str in validNames)
            {
                if (string.Compare(str, name, true) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        internal override void Serialize(Serializer serializer)
        {
            string str = @"\field(Info)";
            if (this.Name == "")
            {
                throw new InvalidOperationException(DomSR.MissingObligatoryProperty("Name", "InfoField"));
            }
            str = str + "[Name = \"" + this.Name + "\"]";
            serializer.Write(str);
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(InfoField));
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
                if (!this.IsValidName(value))
                {
                    throw new ArgumentException(DomSR.InvalidInfoFieldName(value));
                }
                this.name.Value = value;
            }
        }
    }
}

