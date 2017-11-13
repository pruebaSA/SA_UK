namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;
    using System.Collections.Generic;

    internal class PdfResourceMap : PdfDictionary
    {
        public PdfResourceMap()
        {
        }

        protected PdfResourceMap(PdfDictionary dict) : base(dict)
        {
        }

        public PdfResourceMap(PdfDocument document) : base(document)
        {
        }

        internal void CollectResourceNames(Dictionary<string, object> usedResourceNames)
        {
            foreach (PdfName name in base.Elements.KeyNames)
            {
                usedResourceNames.Add(name.ToString(), null);
            }
        }
    }
}

