namespace PdfSharp.Pdf.Annotations
{
    using PdfSharp.Pdf;
    using System;

    internal sealed class PdfWidgetAnnotation : PdfAnnotation
    {
        public PdfWidgetAnnotation()
        {
            this.Initialize();
        }

        public PdfWidgetAnnotation(PdfDocument document) : base(document)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            base.Elements.SetName("/Subtype", "/Widget");
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        internal class Keys : PdfAnnotation.Keys
        {
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string H = "/H";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Dictionary)]
            public const string MK = "/MK";

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfWidgetAnnotation.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

