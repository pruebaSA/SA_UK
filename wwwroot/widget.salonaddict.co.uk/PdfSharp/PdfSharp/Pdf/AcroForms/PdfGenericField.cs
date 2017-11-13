namespace PdfSharp.Pdf.AcroForms
{
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfGenericField : PdfAcroField
    {
        internal PdfGenericField(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfGenericField(PdfDocument document) : base(document)
        {
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public class Keys : PdfAcroField.Keys
        {
            private static DictionaryMeta meta;

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfGenericField.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

