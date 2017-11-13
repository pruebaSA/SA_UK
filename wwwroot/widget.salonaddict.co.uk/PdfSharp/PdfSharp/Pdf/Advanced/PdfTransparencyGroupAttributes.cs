namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfTransparencyGroupAttributes : PdfGroupAttributes
    {
        internal PdfTransparencyGroupAttributes(PdfDocument thisDocument) : base(thisDocument)
        {
            base.Elements.SetName("/S", "/Transparency");
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public sealed class Keys : PdfGroupAttributes.Keys
        {
            [KeyInfo(KeyType.Optional | KeyType.NameOrArray)]
            public const string CS = "/CS";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string I = "/I";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string K = "/K";
            private static DictionaryMeta meta;

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfTransparencyGroupAttributes.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

