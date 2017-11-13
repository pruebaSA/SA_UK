namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfTilingPattern : PdfDictionaryWithContentStream
    {
        public PdfTilingPattern(PdfDocument document) : base(document)
        {
            base.Elements.SetName("/Type", "/Pattern");
            base.Elements["/PatternType"] = new PdfInteger(1);
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        internal sealed class Keys : PdfDictionaryWithContentStream.Keys
        {
            [KeyInfo(KeyType.Optional | KeyType.Rectangle)]
            public const string BBox = "/BBox";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Matrix = "/Matrix";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string PaintType = "/PaintType";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string PatternType = "/PatternType";
            [KeyInfo(KeyType.Required | KeyType.Dictionary)]
            public const string Resources = "/Resources";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string TilingType = "/TilingType";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string Type = "/Type";
            [KeyInfo(KeyType.Required | KeyType.Real)]
            public const string XStep = "/XStep";
            [KeyInfo(KeyType.Required | KeyType.Real)]
            public const string YStep = "/YStep";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfTilingPattern.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

