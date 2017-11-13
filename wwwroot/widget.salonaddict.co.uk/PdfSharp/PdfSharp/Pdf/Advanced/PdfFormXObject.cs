namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;
    using System.Runtime.InteropServices;

    public sealed class PdfFormXObject : PdfXObject, IContentStream
    {
        private double dpiX;
        private double dpiY;
        private PdfResources resources;

        internal PdfFormXObject(PdfDocument thisDocument) : base(thisDocument)
        {
            this.dpiX = 72.0;
            this.dpiY = 72.0;
            base.Elements.SetName("/Type", "/XObject");
            base.Elements.SetName("/Subtype", "/Form");
        }

        internal PdfFormXObject(PdfDocument thisDocument, XForm form) : base(thisDocument)
        {
            this.dpiX = 72.0;
            this.dpiY = 72.0;
            base.Elements.SetName("/Type", "/XObject");
            base.Elements.SetName("/Subtype", "/Form");
        }

        internal PdfFormXObject(PdfDocument thisDocument, PdfImportedObjectTable importedObjectTable, XPdfForm form) : base(thisDocument)
        {
            this.dpiX = 72.0;
            this.dpiY = 72.0;
            base.Elements.SetName("/Type", "/XObject");
            base.Elements.SetName("/Subtype", "/Form");
            if (!form.IsTemplate)
            {
                XPdfForm form2 = form;
                PdfPages pages = importedObjectTable.ExternalDocument.Pages;
                if ((form2.PageNumber < 1) || (form2.PageNumber > pages.Count))
                {
                    PSSR.ImportPageNumberOutOfRange(form2.PageNumber, pages.Count, form.path);
                }
                PdfPage page = pages[form2.PageNumber - 1];
                PdfItem item = page.Elements["/Resources"];
                if (item != null)
                {
                    PdfObject obj2;
                    if (item is PdfReference)
                    {
                        obj2 = ((PdfReference) item).Value;
                    }
                    else
                    {
                        obj2 = (PdfDictionary) item;
                    }
                    obj2 = PdfObject.ImportClosure(importedObjectTable, thisDocument, obj2);
                    if (obj2.Reference == null)
                    {
                        thisDocument.irefTable.Add(obj2);
                    }
                    base.Elements["/Resources"] = obj2.Reference;
                }
                PdfRectangle rectangle = page.Elements.GetRectangle("/MediaBox");
                int integer = page.Elements.GetInteger("/Rotate");
                if (integer == 0)
                {
                    base.Elements["/BBox"] = rectangle;
                }
                else
                {
                    base.Elements["/BBox"] = rectangle;
                    XMatrix matrix = new XMatrix();
                    double width = rectangle.Width;
                    double height = rectangle.Height;
                    matrix.RotateAtPrepend((double) -integer, new XPoint(width / 2.0, height / 2.0));
                    double offsetX = (height - width) / 2.0;
                    if (height > width)
                    {
                        matrix.TranslatePrepend(offsetX, offsetX);
                    }
                    else
                    {
                        matrix.TranslatePrepend(-offsetX, -offsetX);
                    }
                    base.Elements.SetMatrix("/Matrix", matrix);
                }
                PdfContent content = page.Contents.CreateSingleContent();
                content.Compressed = true;
                PdfItem item2 = content.Elements["/Filter"];
                if (item2 != null)
                {
                    base.Elements["/Filter"] = item2.Clone();
                }
                base.Stream = content.Stream;
                base.Elements.SetInteger("/Length", content.Stream.Value.Length);
            }
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

        internal double DpiX
        {
            get => 
                this.dpiX;
            set
            {
                this.dpiX = value;
            }
        }

        internal double DpiY
        {
            get => 
                this.dpiY;
            set
            {
                this.dpiY = value;
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

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

        public sealed class Keys : PdfXObject.Keys
        {
            [KeyInfo(KeyType.Required | KeyType.Rectangle)]
            public const string BBox = "/BBox";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string FormType = "/FormType";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary)]
            public const string Group = "/Group";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Matrix = "/Matrix";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Dictionary, typeof(PdfResources))]
            public const string Resources = "/Resources";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string Subtype = "/Subtype";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string Type = "/Type";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfFormXObject.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

