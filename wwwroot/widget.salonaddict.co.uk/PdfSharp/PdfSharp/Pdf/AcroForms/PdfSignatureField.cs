namespace PdfSharp.Pdf.AcroForms
{
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfSignatureField : PdfAcroField
    {
        internal PdfSignatureField(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfSignatureField(PdfDocument document) : base(document)
        {
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public class Keys : PdfAcroField.Keys
        {
            [KeyInfo(KeyType.Required | KeyType.Array)]
            public const string ByteRange = "/ByteRange";
            [KeyInfo(KeyType.Required | KeyType.String)]
            public const string Contents = "/Contents";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string Filter = "/Filter";
            [KeyInfo(KeyType.Optional | KeyType.TextString)]
            public const string Location = "/Location";
            [KeyInfo(KeyType.Optional | KeyType.Date)]
            public const string M = "/M";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.TextString)]
            public const string Name = "/Name";
            [KeyInfo(KeyType.Optional | KeyType.TextString)]
            public const string Reason = "/Reason";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string SubFilter = "/SubFilter";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string Type = "/Type";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfSignatureField.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

