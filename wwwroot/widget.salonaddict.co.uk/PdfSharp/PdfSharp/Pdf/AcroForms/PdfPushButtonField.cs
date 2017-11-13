namespace PdfSharp.Pdf.AcroForms
{
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfPushButtonField : PdfButtonField
    {
        internal PdfPushButtonField(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfPushButtonField(PdfDocument document) : base(document)
        {
            base.document = document;
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
                        meta = KeysBase.CreateMeta(typeof(PdfPushButtonField.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

