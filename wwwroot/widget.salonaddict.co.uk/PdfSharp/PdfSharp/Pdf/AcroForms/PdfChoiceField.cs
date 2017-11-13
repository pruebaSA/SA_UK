namespace PdfSharp.Pdf.AcroForms
{
    using PdfSharp.Pdf;
    using System;

    public abstract class PdfChoiceField : PdfAcroField
    {
        protected PdfChoiceField(PdfDictionary dict) : base(dict)
        {
        }

        protected PdfChoiceField(PdfDocument document) : base(document)
        {
        }

        protected int IndexInOptArray(string value)
        {
            PdfArray array = base.Elements["/Opt"] as PdfArray;
            if (array != null)
            {
                int count = array.Elements.Count;
                for (int i = 0; i < count; i++)
                {
                    PdfItem item = array.Elements[i];
                    if (item is PdfString)
                    {
                        if (item.ToString() == value)
                        {
                            return i;
                        }
                    }
                    else if (item is PdfArray)
                    {
                        PdfArray array2 = (PdfArray) item;
                        if ((array2.Elements.Count != 0) && (array2.Elements[i].ToString() == value))
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        protected string ValueInOptArray(int index)
        {
            PdfArray array = base.Elements["/Opt"] as PdfArray;
            if (array != null)
            {
                int count = array.Elements.Count;
                if ((index < 0) || (index >= count))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                PdfItem item = array.Elements[index];
                if (item is PdfString)
                {
                    return item.ToString();
                }
                if (item is PdfArray)
                {
                    PdfArray array2 = (PdfArray) item;
                    return array2.Elements[0].ToString();
                }
            }
            return "";
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public class Keys : PdfAcroField.Keys
        {
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string I = "/I";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Opt = "/Opt";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string TI = "/TI";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfChoiceField.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

