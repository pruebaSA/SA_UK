namespace PdfSharp.Pdf.AcroForms
{
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfComboBoxField : PdfChoiceField
    {
        internal PdfComboBoxField(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfComboBoxField(PdfDocument document) : base(document)
        {
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public int SelectedIndex
        {
            get
            {
                string str = base.Elements.GetString("/V");
                return base.IndexInOptArray(str);
            }
            set
            {
                string str = base.ValueInOptArray(value);
                base.Elements.SetString("/V", str);
            }
        }

        public class Keys : PdfAcroField.Keys
        {
            private static DictionaryMeta meta;

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfComboBoxField.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

