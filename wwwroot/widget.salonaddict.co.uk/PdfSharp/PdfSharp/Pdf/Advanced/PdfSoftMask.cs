namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;

    public class PdfSoftMask : PdfDictionary
    {
        public PdfSoftMask(PdfDocument document) : base(document)
        {
            base.Elements.SetName("/Type", "/Mask");
        }

        public class Keys : KeysBase
        {
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string BC = "/BC";
            [KeyInfo(KeyType.Required | KeyType.Stream)]
            public const string G = "/G";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string S = "/S";
            [KeyInfo(KeyType.Optional | KeyType.FunctionOrName)]
            public const string TR = "/TR";
            [KeyInfo(KeyType.Optional | KeyType.Name, FixedValue="Mask")]
            public const string Type = "/Type";
        }
    }
}

