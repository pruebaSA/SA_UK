namespace PdfSharp.Pdf.Actions
{
    using PdfSharp.Pdf;
    using System;

    public abstract class PdfAction : PdfDictionary
    {
        protected PdfAction()
        {
            base.Elements.SetName("/Type", "/Action");
        }

        protected PdfAction(PdfDocument document) : base(document)
        {
            base.Elements.SetName("/Type", "/Action");
        }

        internal class Keys : KeysBase
        {
            [KeyInfo(KeyType.Optional | KeyType.ArrayOrDictionary)]
            public const string Next = "/Next";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string S = "/S";
            [KeyInfo(KeyType.Optional | KeyType.Name, FixedValue="Action")]
            public const string Type = "/Type";
        }
    }
}

