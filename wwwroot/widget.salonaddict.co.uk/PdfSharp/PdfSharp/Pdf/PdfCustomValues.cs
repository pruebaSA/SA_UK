namespace PdfSharp.Pdf
{
    using System;
    using System.Reflection;

    public class PdfCustomValues : PdfDictionary
    {
        internal PdfCustomValues()
        {
        }

        internal PdfCustomValues(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfCustomValues(PdfDocument document) : base(document)
        {
        }

        public static void ClearAllCustomValues(PdfDocument document)
        {
            document.CustomValues = null;
            foreach (PdfPage page in document.Pages)
            {
                page.CustomValues = null;
            }
        }

        public bool Contains(string key) => 
            base.Elements.ContainsKey(key);

        internal static PdfCustomValues Get(PdfDictionary.DictionaryElements elem)
        {
            PdfCustomValues values;
            string customValueKey = elem.Owner.Owner.Internals.CustomValueKey;
            PdfDictionary dict = elem.GetDictionary(customValueKey);
            if (dict == null)
            {
                values = new PdfCustomValues();
                elem.Owner.Owner.Internals.AddObject(values);
                elem.Add(customValueKey, values);
                return values;
            }
            values = dict as PdfCustomValues;
            if (values == null)
            {
                values = new PdfCustomValues(dict);
            }
            return values;
        }

        internal static void Remove(PdfDictionary.DictionaryElements elem)
        {
            elem.Remove(elem.Owner.Owner.Internals.CustomValueKey);
        }

        public PdfCustomValueCompressionMode CompressionMode
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public PdfCustomValue this[string key]
        {
            get
            {
                PdfDictionary dict = base.Elements.GetDictionary(key);
                if (dict == null)
                {
                    return null;
                }
                PdfCustomValue value2 = dict as PdfCustomValue;
                if (value2 == null)
                {
                    value2 = new PdfCustomValue(dict);
                }
                return value2;
            }
            set
            {
                if (value == null)
                {
                    base.Elements.Remove(key);
                }
                else
                {
                    this.Owner.Internals.AddObject(value);
                    base.Elements.SetReference(key, value);
                }
            }
        }
    }
}

