namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class DocumentInfo : DocumentObject
    {
        [DV]
        internal NString author;
        [DV]
        internal NString comment;
        [DV]
        internal NString keywords;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString subject;
        [DV]
        internal NString title;

        public DocumentInfo()
        {
            this.title = NString.NullValue;
            this.author = NString.NullValue;
            this.keywords = NString.NullValue;
            this.subject = NString.NullValue;
            this.comment = NString.NullValue;
        }

        internal DocumentInfo(DocumentObject parent) : base(parent)
        {
            this.title = NString.NullValue;
            this.author = NString.NullValue;
            this.keywords = NString.NullValue;
            this.subject = NString.NullValue;
            this.comment = NString.NullValue;
        }

        public DocumentInfo Clone() => 
            ((DocumentInfo) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            int pos = serializer.BeginContent("Info");
            if (this.Title != string.Empty)
            {
                serializer.WriteSimpleAttribute("Title", this.Title);
            }
            if (this.Subject != string.Empty)
            {
                serializer.WriteSimpleAttribute("Subject", this.Subject);
            }
            if (this.Author != string.Empty)
            {
                serializer.WriteSimpleAttribute("Author", this.Author);
            }
            if (this.Keywords != string.Empty)
            {
                serializer.WriteSimpleAttribute("Keywords", this.Keywords);
            }
            serializer.EndContent(pos);
        }

        public string Author
        {
            get => 
                this.author.Value;
            set
            {
                this.author.Value = value;
            }
        }

        public string Comment
        {
            get => 
                this.comment.Value;
            set
            {
                this.comment.Value = value;
            }
        }

        public string Keywords
        {
            get => 
                this.keywords.Value;
            set
            {
                this.keywords.Value = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(DocumentInfo));
                }
                return meta;
            }
        }

        public string Subject
        {
            get => 
                this.subject.Value;
            set
            {
                this.subject.Value = value;
            }
        }

        public string Title
        {
            get => 
                this.title.Value;
            set
            {
                this.title.Value = value;
            }
        }
    }
}

