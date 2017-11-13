namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfShadingPattern : PdfDictionaryWithContentStream
    {
        public PdfShadingPattern(PdfDocument document) : base(document)
        {
            base.Elements.SetName("/Type", "/Pattern");
            base.Elements["/PatternType"] = new PdfInteger(2);
        }

        public void SetupFromBrush(XLinearGradientBrush brush, XMatrix matrix)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            PdfShading shading = new PdfShading(base.document);
            shading.SetupFromBrush(brush);
            base.Elements["/Shading"] = shading;
            base.Elements.SetMatrix("/Matrix", matrix);
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        internal sealed class Keys : PdfDictionaryWithContentStream.Keys
        {
            [KeyInfo(KeyType.Optional | KeyType.Dictionary)]
            public const string ExtGState = "/ExtGState";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Matrix = "/Matrix";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string PatternType = "/PatternType";
            [KeyInfo(KeyType.Required | KeyType.Dictionary)]
            public const string Shading = "/Shading";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string Type = "/Type";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfShadingPattern.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

