namespace PdfSharp.Pdf.AcroForms
{
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfAcroForm : PdfDictionary
    {
        private PdfAcroField.PdfAcroFieldCollection fields;

        internal PdfAcroForm(PdfDictionary dictionary) : base(dictionary)
        {
        }

        internal PdfAcroForm(PdfDocument document) : base(document)
        {
            base.document = document;
        }

        public PdfAcroField.PdfAcroFieldCollection Fields
        {
            get
            {
                if (this.fields == null)
                {
                    object obj2 = base.Elements.GetValue("/Fields", VCF.CreateIndirect);
                    this.fields = (PdfAcroField.PdfAcroFieldCollection) obj2;
                }
                return this.fields;
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public sealed class Keys : KeysBase
        {
            [KeyInfo(KeyType.Array)]
            public const string CO = "/CO";
            [KeyInfo(KeyType.Optional | KeyType.String)]
            public const string DA = "/DA";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary)]
            public const string DR = "/DR";
            [KeyInfo(KeyType.Required | KeyType.Array, typeof(PdfAcroField.PdfAcroFieldCollection))]
            public const string Fields = "/Fields";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string NeedAppearances = "/NeedAppearances";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string Q = "/Q";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Integer)]
            public const string SigFlags = "/SigFlags";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfAcroForm.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

