namespace PdfSharp.Pdf
{
    using PdfSharp;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.Annotations;
    using PdfSharp.Pdf.IO;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;

    public sealed class PdfPage : PdfDictionary, IContentStream
    {
        private PdfAnnotations annotations;
        private bool closed;
        private PdfContents contents;
        private PdfCustomValues customValues;
        private PageOrientation orientation;
        private PageSize pageSize;
        internal PdfContent RenderContent;
        private PdfResources resources;
        private object tag;
        internal bool transparencyUsed;
        private PdfSharp.Pdf.TrimMargins trimMargins;

        public PdfPage()
        {
            this.trimMargins = new PdfSharp.Pdf.TrimMargins();
            base.Elements.SetName("/Type", "/Page");
            this.Initialize();
        }

        internal PdfPage(PdfDictionary dict) : base(dict)
        {
            this.trimMargins = new PdfSharp.Pdf.TrimMargins();
            if ((Math.Abs((int) (base.Elements.GetInteger("/Rotate") / 90)) % 2) == 1)
            {
                this.orientation = PageOrientation.Landscape;
            }
        }

        public PdfPage(PdfDocument document) : base(document)
        {
            this.trimMargins = new PdfSharp.Pdf.TrimMargins();
            base.Elements.SetName("/Type", "/Page");
            base.Elements["/Parent"] = document.Pages.Reference;
            this.Initialize();
        }

        public PdfLinkAnnotation AddDocumentLink(PdfRectangle rect, int destinationPage)
        {
            PdfLinkAnnotation annotation = PdfLinkAnnotation.CreateDocumentLink(rect, destinationPage);
            this.Annotations.Add(annotation);
            return annotation;
        }

        public PdfLinkAnnotation AddFileLink(PdfRectangle rect, string fileName)
        {
            PdfLinkAnnotation annotation = PdfLinkAnnotation.CreateFileLink(rect, fileName);
            this.Annotations.Add(annotation);
            return annotation;
        }

        public PdfLinkAnnotation AddWebLink(PdfRectangle rect, string url)
        {
            PdfLinkAnnotation annotation = PdfLinkAnnotation.CreateWebLink(rect, url);
            this.Annotations.Add(annotation);
            return annotation;
        }

        public void Close()
        {
            this.closed = true;
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

        internal static void InheritValues(PdfDictionary page, InheritedValues values)
        {
            if (values.Resources != null)
            {
                PdfDictionary dictionary;
                PdfItem item = page.Elements["/Resources"];
                if (item is PdfReference)
                {
                    dictionary = (PdfDictionary) ((PdfReference) item).Value.Clone();
                    dictionary.Document = page.Owner;
                }
                else
                {
                    dictionary = (PdfDictionary) item;
                }
                if (dictionary == null)
                {
                    dictionary = values.Resources.Clone();
                    dictionary.Document = page.Owner;
                    page.Elements.Add("/Resources", dictionary);
                }
                else
                {
                    foreach (PdfName name in values.Resources.Elements.KeyNames)
                    {
                        if (!dictionary.Elements.ContainsKey(name.Value))
                        {
                            PdfItem item2 = values.Resources.Elements[name];
                            if (item2 is PdfObject)
                            {
                                item2 = item2.Clone();
                            }
                            dictionary.Elements.Add(name.ToString(), item2);
                        }
                    }
                }
            }
            if ((values.MediaBox != null) && (page.Elements["/MediaBox"] == null))
            {
                page.Elements["/MediaBox"] = values.MediaBox;
            }
            if ((values.CropBox != null) && (page.Elements["/CropBox"] == null))
            {
                page.Elements["/CropBox"] = values.CropBox;
            }
            if ((values.Rotate != null) && (page.Elements["/Rotate"] == null))
            {
                page.Elements["/Rotate"] = values.Rotate;
            }
        }

        internal static void InheritValues(PdfDictionary page, ref InheritedValues values)
        {
            PdfItem item = page.Elements["/Resources"];
            if (item != null)
            {
                if (item is PdfReference)
                {
                    values.Resources = (PdfDictionary) ((PdfReference) item).Value;
                }
                else
                {
                    values.Resources = (PdfDictionary) item;
                }
            }
            item = page.Elements["/MediaBox"];
            if (item != null)
            {
                values.MediaBox = new PdfRectangle(item);
            }
            item = page.Elements["/CropBox"];
            if (item != null)
            {
                values.CropBox = new PdfRectangle(item);
            }
            item = page.Elements["/Rotate"];
            if (item != null)
            {
                if (item is PdfReference)
                {
                    item = ((PdfReference) item).Value;
                }
                values.Rotate = (PdfInteger) item;
            }
        }

        private void Initialize()
        {
            this.Size = RegionInfo.CurrentRegion.IsMetric ? PageSize.A4 : PageSize.Letter;
            PdfRectangle mediaBox = this.MediaBox;
        }

        string IContentStream.GetFontName(XFont font, out PdfFont pdfFont) => 
            this.GetFontName(font, out pdfFont);

        string IContentStream.GetFontName(string idName, byte[] fontData, out PdfFont pdfFont) => 
            this.GetFontName(idName, fontData, out pdfFont);

        string IContentStream.GetFormName(XForm form) => 
            this.GetFormName(form);

        string IContentStream.GetImageName(XImage image) => 
            this.GetImageName(image);

        internal override void PrepareForSave()
        {
            if (this.trimMargins.AreSet)
            {
                double num = (this.trimMargins.Left.Point + this.Width.Point) + this.trimMargins.Right.Point;
                double num2 = (this.trimMargins.Top.Point + this.Height.Point) + this.trimMargins.Bottom.Point;
                this.MediaBox = new PdfRectangle(0.0, 0.0, num, num2);
                this.CropBox = new PdfRectangle(0.0, 0.0, num, num2);
                this.BleedBox = new PdfRectangle(0.0, 0.0, num, num2);
                PdfRectangle rectangle = new PdfRectangle(this.trimMargins.Left.Point, this.trimMargins.Top.Point, num - this.trimMargins.Right.Point, num2 - this.trimMargins.Bottom.Point);
                this.TrimBox = rectangle;
                this.ArtBox = rectangle.Clone();
            }
        }

        internal string TryGetFontName(string idName, out PdfFont pdfFont)
        {
            pdfFont = base.document.FontTable.TryGetFont(idName);
            string str = null;
            if (pdfFont != null)
            {
                str = this.Resources.AddFont(pdfFont);
            }
            return str;
        }

        internal override void WriteObject(PdfWriter writer)
        {
            PdfRectangle mediaBox = this.MediaBox;
            if (this.orientation == PageOrientation.Landscape)
            {
                this.MediaBox = new PdfRectangle(mediaBox.X1, mediaBox.Y1, mediaBox.Y2, mediaBox.X2);
            }
            this.transparencyUsed = true;
            if (this.transparencyUsed && !base.Elements.ContainsKey("/Group"))
            {
                PdfDictionary dictionary = new PdfDictionary();
                base.elements["/Group"] = dictionary;
                if (base.document.Options.ColorMode != PdfColorMode.Cmyk)
                {
                    dictionary.Elements.SetName("/CS", "/DeviceRGB");
                }
                else
                {
                    dictionary.Elements.SetName("/CS", "/DeviceCMYK");
                }
                dictionary.Elements.SetName("/S", "/Transparency");
                dictionary.Elements["/I"] = new PdfBoolean(false);
                dictionary.Elements["/K"] = new PdfBoolean(false);
            }
            base.WriteObject(writer);
            if (this.orientation == PageOrientation.Landscape)
            {
                this.MediaBox = mediaBox;
            }
        }

        public PdfAnnotations Annotations
        {
            get
            {
                if (this.annotations == null)
                {
                    this.annotations = (PdfAnnotations) base.Elements.GetValue("/Annots", VCF.Create);
                    this.annotations.Page = this;
                }
                return this.annotations;
            }
        }

        public PdfRectangle ArtBox
        {
            get => 
                base.Elements.GetRectangle("/ArtBox", true);
            set
            {
                base.Elements.SetRectangle("/ArtBox", value);
            }
        }

        public PdfRectangle BleedBox
        {
            get => 
                base.Elements.GetRectangle("/BleedBox", true);
            set
            {
                base.Elements.SetRectangle("/BleedBox", value);
            }
        }

        public PdfContents Contents
        {
            get
            {
                if (this.contents == null)
                {
                    PdfItem item = base.Elements["/Contents"];
                    if (item == null)
                    {
                        this.contents = new PdfContents(this.Owner);
                    }
                    else
                    {
                        if (item is PdfReference)
                        {
                            item = ((PdfReference) item).Value;
                        }
                        PdfArray array = item as PdfArray;
                        if (array != null)
                        {
                            if (array.IsIndirect)
                            {
                                array = array.Clone();
                                array.Document = this.Owner;
                            }
                            this.contents = new PdfContents(array);
                        }
                        else
                        {
                            this.contents = new PdfContents(this.Owner);
                            PdfContent content = new PdfContent((PdfDictionary) item);
                            this.contents.Elements.Add(content.Reference);
                        }
                    }
                    base.Elements["/Contents"] = this.contents;
                }
                return this.contents;
            }
        }

        public PdfRectangle CropBox
        {
            get => 
                base.Elements.GetRectangle("/CropBox", true);
            set
            {
                base.Elements.SetRectangle("/CropBox", value);
            }
        }

        public PdfCustomValues CustomValues
        {
            get
            {
                if (this.customValues == null)
                {
                    this.customValues = PdfCustomValues.Get(base.Elements);
                }
                return this.customValues;
            }
            set
            {
                if (value != null)
                {
                    throw new ArgumentException("Only null is allowed to clear all custom values.");
                }
                PdfCustomValues.Remove(base.Elements);
                this.customValues = null;
            }
        }

        internal override PdfDocument Document
        {
            set
            {
                if (!object.ReferenceEquals(base.document, value))
                {
                    if (base.document != null)
                    {
                        throw new InvalidOperationException("Cannot change document.");
                    }
                    base.document = value;
                    if (base.iref != null)
                    {
                        base.iref.Document = value;
                    }
                    base.Elements["/Parent"] = base.document.Pages.Reference;
                }
            }
        }

        public XUnit Height
        {
            get
            {
                PdfRectangle mediaBox = this.MediaBox;
                return ((this.orientation == PageOrientation.Portrait) ? mediaBox.Height : mediaBox.Width);
            }
            set
            {
                PdfRectangle mediaBox = this.MediaBox;
                if (this.orientation == PageOrientation.Portrait)
                {
                    this.MediaBox = new PdfRectangle(mediaBox.X1, 0.0, mediaBox.X2, (double) value);
                }
                else
                {
                    this.MediaBox = new PdfRectangle(0.0, mediaBox.Y1, (double) value, mediaBox.Y2);
                }
                this.pageSize = PageSize.Undefined;
            }
        }

        internal bool IsClosed =>
            this.closed;

        public PdfRectangle MediaBox
        {
            get => 
                base.Elements.GetRectangle("/MediaBox", true);
            set
            {
                base.Elements.SetRectangle("/MediaBox", value);
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public PageOrientation Orientation
        {
            get => 
                this.orientation;
            set
            {
                this.orientation = value;
            }
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

        public int Rotate
        {
            get => 
                base.elements.GetInteger("/Rotate");
            set
            {
                if (((value / 90) * 90) != value)
                {
                    throw new ArgumentException("Value must be a multiple of 90.");
                }
                base.elements.SetInteger("/Rotate", value);
            }
        }

        public PageSize Size
        {
            get => 
                this.pageSize;
            set
            {
                if (!Enum.IsDefined(typeof(PageSize), value))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(PageSize));
                }
                XSize size = PageSizeConverter.ToSize(value);
                this.MediaBox = new PdfRectangle(0.0, 0.0, size.Width, size.Height);
                this.pageSize = value;
            }
        }

        public object Tag
        {
            get => 
                this.tag;
            set
            {
                this.tag = value;
            }
        }

        public PdfRectangle TrimBox
        {
            get => 
                base.Elements.GetRectangle("/TrimBox", true);
            set
            {
                base.Elements.SetRectangle("/TrimBox", value);
            }
        }

        public PdfSharp.Pdf.TrimMargins TrimMargins
        {
            get
            {
                if (this.trimMargins == null)
                {
                    this.trimMargins = new PdfSharp.Pdf.TrimMargins();
                }
                return this.trimMargins;
            }
            set
            {
                if (this.trimMargins == null)
                {
                    this.trimMargins = new PdfSharp.Pdf.TrimMargins();
                }
                if (value != null)
                {
                    this.trimMargins.Left = value.Left;
                    this.trimMargins.Right = value.Right;
                    this.trimMargins.Top = value.Top;
                    this.trimMargins.Bottom = value.Bottom;
                }
                else
                {
                    this.trimMargins.All = 0;
                }
            }
        }

        public XUnit Width
        {
            get
            {
                PdfRectangle mediaBox = this.MediaBox;
                return ((this.orientation == PageOrientation.Portrait) ? mediaBox.Width : mediaBox.Height);
            }
            set
            {
                PdfRectangle mediaBox = this.MediaBox;
                if (this.orientation == PageOrientation.Portrait)
                {
                    this.MediaBox = new PdfRectangle(0.0, mediaBox.Y1, (double) value, mediaBox.Y2);
                }
                else
                {
                    this.MediaBox = new PdfRectangle(mediaBox.X1, 0.0, mediaBox.X2, (double) value);
                }
                this.pageSize = PageSize.Undefined;
            }
        }

        public class InheritablePageKeys : KeysBase
        {
            [KeyInfo(KeyType.Inheritable | KeyType.Optional | KeyType.Rectangle)]
            public const string CropBox = "/CropBox";
            [KeyInfo(KeyType.Inheritable | KeyType.Required | KeyType.Rectangle)]
            public const string MediaBox = "/MediaBox";
            [KeyInfo(KeyType.Inheritable | KeyType.Required | KeyType.Dictionary, typeof(PdfResources))]
            public const string Resources = "/Resources";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string Rotate = "/Rotate";
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct InheritedValues
        {
            public PdfDictionary Resources;
            public PdfRectangle MediaBox;
            public PdfRectangle CropBox;
            public PdfInteger Rotate;
        }

        public sealed class Keys : PdfPage.InheritablePageKeys
        {
            [KeyInfo("1.2", KeyType.Optional | KeyType.Dictionary)]
            public const string AA = "/AA";
            [KeyInfo(KeyType.Optional | KeyType.Array, typeof(PdfAnnotations))]
            public const string Annots = "/Annots";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Rectangle)]
            public const string ArtBox = "/ArtBox";
            [KeyInfo("1.1", KeyType.Optional | KeyType.Array)]
            public const string B = "/B";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Rectangle)]
            public const string BleedBox = "/BleedBox";
            [KeyInfo("1.4", KeyType.Optional | KeyType.Dictionary)]
            public const string BoxColorInfo = "/BoxColorInfo";
            [KeyInfo(KeyType.Optional | KeyType.Stream)]
            public const string Contents = "/Contents";
            [KeyInfo("1.1", KeyType.Optional | KeyType.Real)]
            public const string Dur = "/Dur";
            [KeyInfo("1.4", KeyType.Optional | KeyType.Dictionary)]
            public const string Group = "/Group";
            [KeyInfo("1.3", KeyType.Optional | KeyType.String)]
            public const string ID = "/ID";
            [KeyInfo(KeyType.Date)]
            public const string LastModified = "/LastModified";
            private static DictionaryMeta meta;
            [KeyInfo("1.4", KeyType.Optional | KeyType.Stream)]
            public const string Metadata = "/Metadata";
            [KeyInfo(KeyType.MustBeIndirect | KeyType.Required | KeyType.Dictionary)]
            public const string Parent = "/Parent";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Dictionary)]
            public const string PieceInfo = "/PieceInfo";
            [KeyInfo("1.5", KeyType.Optional | KeyType.Dictionary)]
            public const string PresSteps = "/PresSteps";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Real)]
            public const string PZ = "/PZ";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Dictionary)]
            public const string SeparationInfo = "/SeparationInfo";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string StructParents = "/StructParents";
            [KeyInfo("1.5", KeyType.Optional | KeyType.Name)]
            public const string Tabs = "/Tabs";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string TemplateInstantiated = "/TemplateInstantiated";
            [KeyInfo(KeyType.Optional | KeyType.Stream)]
            public const string Thumb = "/Thumb";
            [KeyInfo("1.1", KeyType.Optional | KeyType.Dictionary)]
            public const string Trans = "/Trans";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Rectangle)]
            public const string TrimBox = "/TrimBox";
            [KeyInfo(KeyType.Required | KeyType.Name, FixedValue="Page")]
            public const string Type = "/Type";
            [KeyInfo("1.6", KeyType.Optional | KeyType.Real)]
            public const string UserUnit = "/UserUnit";
            [KeyInfo("1.6", KeyType.Optional | KeyType.Dictionary)]
            public const string VP = "/VP";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfPage.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

