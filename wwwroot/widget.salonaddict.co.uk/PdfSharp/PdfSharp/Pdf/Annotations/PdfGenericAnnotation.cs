namespace PdfSharp.Pdf.Annotations
{
    using PdfSharp.Pdf;
    using System;

    internal sealed class PdfGenericAnnotation : PdfAnnotation
    {
        public PdfGenericAnnotation(PdfDictionary dict) : base(dict)
        {
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        internal class Keys : PdfAnnotation.Keys
        {
            private static DictionaryMeta meta;

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfGenericAnnotation.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

