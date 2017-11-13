namespace PdfSharp.Pdf
{
    using System;

    public sealed class PdfDocumentInformation : PdfDictionary
    {
        private PdfDocumentInformation(PdfDictionary dict) : base(dict)
        {
        }

        public PdfDocumentInformation(PdfDocument document) : base(document)
        {
        }

        public string Author
        {
            get => 
                base.Elements.GetString("/Author");
            set
            {
                base.Elements.SetString("/Author", value);
            }
        }

        public DateTime CreationDate
        {
            get => 
                base.Elements.GetDateTime("/CreationDate", DateTime.Now);
            set
            {
                base.Elements.SetDateTime("/CreationDate", value);
            }
        }

        public string Creator
        {
            get => 
                base.Elements.GetString("/Creator");
            set
            {
                base.Elements.SetString("/Creator", value);
            }
        }

        public string Keywords
        {
            get => 
                base.Elements.GetString("/Keywords");
            set
            {
                base.Elements.SetString("/Keywords", value);
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public DateTime ModificationDate
        {
            get => 
                base.Elements.GetDateTime("/ModDate", DateTime.Now);
            set
            {
                base.Elements.SetDateTime("/ModDate", value);
            }
        }

        public string Producer =>
            base.Elements.GetString("/Producer");

        public string Subject
        {
            get => 
                base.Elements.GetString("/Subject");
            set
            {
                base.Elements.SetString("/Subject", value);
            }
        }

        public string Title
        {
            get => 
                base.Elements.GetString("/Title");
            set
            {
                base.Elements.SetString("/Title", value);
            }
        }

        internal sealed class Keys : KeysBase
        {
            [KeyInfo(KeyType.Optional | KeyType.String)]
            public const string Author = "/Author";
            [KeyInfo(KeyType.Optional | KeyType.Date)]
            public const string CreationDate = "/CreationDate";
            [KeyInfo(KeyType.Optional | KeyType.String)]
            public const string Creator = "/Creator";
            [KeyInfo(KeyType.Optional | KeyType.String)]
            public const string Keywords = "/Keywords";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.String)]
            public const string ModDate = "/ModDate";
            [KeyInfo(KeyType.Optional | KeyType.String)]
            public const string Producer = "/Producer";
            [KeyInfo(KeyType.Optional | KeyType.String)]
            public const string Subject = "/Subject";
            [KeyInfo(KeyType.Optional | KeyType.String)]
            public const string Title = "/Title";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Name)]
            public const string Trapped = "/Trapped";

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfDocumentInformation.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

