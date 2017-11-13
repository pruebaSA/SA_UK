namespace PdfSharp.Pdf.Annotations
{
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfTextAnnotation : PdfAnnotation
    {
        public PdfTextAnnotation()
        {
            this.Initialize();
        }

        public PdfTextAnnotation(PdfDocument document) : base(document)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            base.Elements.SetName("/Subtype", "/Text");
            this.Icon = PdfTextAnnotationIcon.Comment;
        }

        public PdfTextAnnotationIcon Icon
        {
            get
            {
                string name = base.Elements.GetName("/Name");
                if (name == "")
                {
                    return PdfTextAnnotationIcon.NoIcon;
                }
                name = name.Substring(1);
                if (!Enum.IsDefined(typeof(PdfTextAnnotationIcon), name))
                {
                    return PdfTextAnnotationIcon.NoIcon;
                }
                return (PdfTextAnnotationIcon) Enum.Parse(typeof(PdfTextAnnotationIcon), name, false);
            }
            set
            {
                if (Enum.IsDefined(typeof(PdfTextAnnotationIcon), value) && (value != PdfTextAnnotationIcon.NoIcon))
                {
                    base.Elements.SetName("/Name", "/" + value.ToString());
                }
                else
                {
                    base.Elements.Remove("/Name");
                }
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public bool Open
        {
            get => 
                base.Elements.GetBoolean("/Open");
            set
            {
                base.Elements.SetBoolean("/Open", value);
            }
        }

        internal class Keys : PdfAnnotation.Keys
        {
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string Name = "/Name";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string Open = "/Open";

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfTextAnnotation.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

