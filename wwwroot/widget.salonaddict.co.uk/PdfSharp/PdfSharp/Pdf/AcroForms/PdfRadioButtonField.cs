namespace PdfSharp.Pdf.AcroForms
{
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfRadioButtonField : PdfButtonField
    {
        internal PdfRadioButtonField(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfRadioButtonField(PdfDocument document) : base(document)
        {
            base.document = document;
        }

        private int IndexInOptStrings(string value)
        {
            PdfArray array = base.Elements["/Opt"] as PdfArray;
            if (array != null)
            {
                int count = array.Elements.Count;
                for (int i = 0; i < count; i++)
                {
                    PdfItem item = array.Elements[i];
                    if ((item is PdfString) && (item.ToString() == value))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public int SelectedIndex
        {
            get
            {
                string str = base.Elements.GetString("/V");
                return this.IndexInOptStrings(str);
            }
            set
            {
                PdfArray array = base.Elements["/Opt"] as PdfArray;
                if (array != null)
                {
                    int count = array.Elements.Count;
                    if ((value < 0) || (value >= count))
                    {
                        throw new ArgumentOutOfRangeException("value");
                    }
                    base.Elements.SetName("/V", array.Elements[value].ToString());
                }
            }
        }

        public class Keys : PdfButtonField.Keys
        {
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Opt = "/Opt";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfRadioButtonField.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

