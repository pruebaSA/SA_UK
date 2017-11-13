namespace MigraDoc.DocumentObjectModel.Fields
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class BookmarkField : DocumentObject
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString name;

        internal BookmarkField()
        {
            this.name = NString.NullValue;
        }

        internal BookmarkField(DocumentObject parent) : base(parent)
        {
            this.name = NString.NullValue;
        }

        public BookmarkField(string name) : this()
        {
            this.Name = name;
        }

        public BookmarkField Clone() => 
            ((BookmarkField) this.DeepCopy());

        public override bool IsNull() => 
            false;

        internal override void Serialize(Serializer serializer)
        {
            if (this.name.Value == string.Empty)
            {
                throw new InvalidOperationException(DomSR.MissingObligatoryProperty("Name", "BookmarkField"));
            }
            serializer.Write("\\field(Bookmark)[Name = \"" + this.Name.Replace(@"\", @"\\").Replace("\"", "\\\"") + "\"]");
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(BookmarkField));
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

