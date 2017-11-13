namespace PdfSharp.Pdf.AcroForms
{
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfCheckBoxField : PdfButtonField
    {
        internal PdfCheckBoxField(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfCheckBoxField(PdfDocument document) : base(document)
        {
            base.document = document;
        }

        public bool Checked
        {
            get
            {
                string str = base.Elements.GetString("/V");
                return ((str.Length != 0) && (str != "/Off"));
            }
            set
            {
                string str = value ? base.GetNonOffValue() : "/Off";
                base.Elements.SetName("/V", str);
                base.Elements.SetName("/AS", str);
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public class Keys : PdfButtonField.Keys
        {
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.TextString)]
            public const string Opt = "/Opt";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfCheckBoxField.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

