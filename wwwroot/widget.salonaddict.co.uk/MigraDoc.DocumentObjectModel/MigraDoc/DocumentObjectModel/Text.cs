namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class Text : DocumentObject
    {
        [DV]
        internal NString content;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public Text()
        {
            this.content = NString.NullValue;
        }

        internal Text(DocumentObject parent) : base(parent)
        {
            this.content = NString.NullValue;
        }

        public Text(string content) : this()
        {
            this.Content = content;
        }

        public Text Clone() => 
            ((Text) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            string str = DdlEncoder.StringToText(this.content.Value).Replace(new string('\x00ad', 1), @"\-");
            serializer.Write(str);
        }

        public string Content
        {
            get => 
                this.content.Value;
            set
            {
                this.content.Value = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Text));
                }
                return meta;
            }
        }
    }
}

