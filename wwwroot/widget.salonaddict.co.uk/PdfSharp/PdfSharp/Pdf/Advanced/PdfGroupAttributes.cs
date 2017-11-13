namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;

    public abstract class PdfGroupAttributes : PdfDictionary
    {
        internal PdfGroupAttributes(PdfDocument thisDocument) : base(thisDocument)
        {
            base.Elements.SetName("/Type", "/Group");
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public class Keys : KeysBase
        {
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string S = "/S";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string Type = "/Type";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfGroupAttributes.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

