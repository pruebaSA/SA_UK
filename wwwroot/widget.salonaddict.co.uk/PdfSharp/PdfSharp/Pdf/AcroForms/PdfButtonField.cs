namespace PdfSharp.Pdf.AcroForms
{
    using PdfSharp.Pdf;
    using System;
    using System.Collections.Generic;

    public abstract class PdfButtonField : PdfAcroField
    {
        protected PdfButtonField(PdfDictionary dict) : base(dict)
        {
        }

        protected PdfButtonField(PdfDocument document) : base(document)
        {
        }

        internal override void GetDescendantNames(ref List<PdfName> names, string partialName)
        {
            string str = base.Elements.GetString("/T");
            if (str == "")
            {
                str = "???";
            }
            if (str.Length > 0)
            {
                if (!string.IsNullOrEmpty(partialName))
                {
                    names.Add(new PdfName(partialName + "." + str));
                }
                else
                {
                    names.Add(new PdfName(str));
                }
            }
        }

        protected string GetNonOffValue()
        {
            PdfDictionary dictionary = base.Elements["/AP"] as PdfDictionary;
            if (dictionary != null)
            {
                PdfDictionary dictionary2 = dictionary.Elements["/N"] as PdfDictionary;
                if (dictionary2 != null)
                {
                    foreach (string str in dictionary2.Elements.Keys)
                    {
                        if (str != "/Off")
                        {
                            return str;
                        }
                    }
                }
            }
            return null;
        }

        public class Keys : PdfAcroField.Keys
        {
        }
    }
}

