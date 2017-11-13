namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;
    using System.Runtime.InteropServices;

    public abstract class PdfDictionaryWithContentStream : PdfDictionary, IContentStream
    {
        private PdfResources resources;

        public PdfDictionaryWithContentStream()
        {
        }

        protected PdfDictionaryWithContentStream(PdfDictionary dict) : base(dict)
        {
        }

        public PdfDictionaryWithContentStream(PdfDocument document) : base(document)
        {
        }

        internal string GetFontName(XFont font, out PdfFont pdfFont)
        {
            pdfFont = base.document.FontTable.GetFont(font);
            return this.Resources.AddFont(pdfFont);
        }

        internal string GetFontName(string idName, byte[] fontData, out PdfFont pdfFont)
        {
            pdfFont = base.document.FontTable.GetFont(idName, fontData);
            return this.Resources.AddFont(pdfFont);
        }

        internal string GetFormName(XForm form)
        {
            PdfFormXObject obj2 = base.document.FormTable.GetForm(form);
            return this.Resources.AddForm(obj2);
        }

        internal string GetImageName(XImage image)
        {
            PdfImage image2 = base.document.ImageTable.GetImage(image);
            return this.Resources.AddImage(image2);
        }

        string IContentStream.GetFontName(XFont font, out PdfFont pdfFont) => 
            this.GetFontName(font, out pdfFont);

        string IContentStream.GetFontName(string idName, byte[] fontData, out PdfFont pdfFont) => 
            this.GetFontName(idName, fontData, out pdfFont);

        string IContentStream.GetFormName(XForm form)
        {
            throw new NotImplementedException();
        }

        string IContentStream.GetImageName(XImage image)
        {
            throw new NotImplementedException();
        }

        PdfResources IContentStream.Resources =>
            this.Resources;

        internal PdfResources Resources
        {
            get
            {
                if (this.resources == null)
                {
                    this.resources = (PdfResources) base.Elements.GetValue("/Resources", VCF.Create);
                }
                return this.resources;
            }
        }

        public class Keys : PdfDictionary.PdfStream.Keys
        {
            [KeyInfo(KeyType.Optional | KeyType.Dictionary, typeof(PdfResources))]
            public const string Resources = "/Resources";
        }
    }
}

