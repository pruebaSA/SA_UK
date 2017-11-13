namespace PdfSharp.Pdf.Annotations
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfRubberStampAnnotation : PdfAnnotation
    {
        public PdfRubberStampAnnotation()
        {
            this.Initialize();
        }

        public PdfRubberStampAnnotation(PdfDocument document) : base(document)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            base.Elements.SetName("/Subtype", "/Stamp");
            base.Color = XColors.Yellow;
        }

        public PdfRubberStampAnnotationIcon Icon
        {
            get
            {
                string name = base.Elements.GetName("/Name");
                if (name == "")
                {
                    return PdfRubberStampAnnotationIcon.NoIcon;
                }
                name = name.Substring(1);
                if (!Enum.IsDefined(typeof(PdfRubberStampAnnotationIcon), name))
                {
                    return PdfRubberStampAnnotationIcon.NoIcon;
                }
                return (PdfRubberStampAnnotationIcon) Enum.Parse(typeof(PdfRubberStampAnnotationIcon), name, false);
            }
            set
            {
                if (Enum.IsDefined(typeof(PdfRubberStampAnnotationIcon), value) && (value != PdfRubberStampAnnotationIcon.NoIcon))
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

        internal class Keys : PdfAnnotation.Keys
        {
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string Name = "/Name";

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfRubberStampAnnotation.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

