namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing;
    using PdfSharp.Fonts;
    using PdfSharp.Fonts.OpenType;
    using PdfSharp.Pdf;
    using System;
    using System.Text;

    internal sealed class PdfType0Font : PdfFont
    {
        private PdfCIDFont descendantFont;
        private XPdfFontOptions fontOptions;

        public PdfType0Font(PdfDocument document) : base(document)
        {
        }

        public PdfType0Font(PdfDocument document, XFont font, bool vertical) : base(document)
        {
            base.Elements.SetName("/Type", "/Font");
            base.Elements.SetName("/Subtype", "/Type0");
            base.Elements.SetName("/Encoding", vertical ? "/Identity-V" : "/Identity-H");
            OpenTypeDescriptor descriptor = (OpenTypeDescriptor) FontDescriptorStock.Global.CreateDescriptor(font);
            base.fontDescriptor = new PdfFontDescriptor(document, descriptor);
            this.fontOptions = font.PdfOptions;
            base.cmapInfo = new CMapInfo(descriptor);
            this.descendantFont = new PdfCIDFont(document, base.fontDescriptor, font);
            this.descendantFont.CMapInfo = base.cmapInfo;
            base.toUnicode = new PdfToUnicodeMap(document, base.cmapInfo);
            document.Internals.AddObject(base.toUnicode);
            base.Elements.Add("/ToUnicode", base.toUnicode);
            this.BaseFont = font.Name.Replace(" ", "");
            switch ((font.Style & XFontStyle.BoldItalic))
            {
                case XFontStyle.Bold:
                    this.BaseFont = this.BaseFont + ",Bold";
                    break;

                case XFontStyle.Italic:
                    this.BaseFont = this.BaseFont + ",Italic";
                    break;

                case XFontStyle.BoldItalic:
                    this.BaseFont = this.BaseFont + ",BoldItalic";
                    break;
            }
            this.BaseFont = PdfFont.CreateEmbeddedFontSubsetName(this.BaseFont);
            base.fontDescriptor.FontName = this.BaseFont;
            this.descendantFont.BaseFont = this.BaseFont;
            PdfArray array = new PdfArray(document);
            this.Owner.irefTable.Add(this.descendantFont);
            array.Elements.Add(this.descendantFont.Reference);
            base.Elements["/DescendantFonts"] = array;
        }

        public PdfType0Font(PdfDocument document, string idName, byte[] fontData, bool vertical) : base(document)
        {
            base.Elements.SetName("/Type", "/Font");
            base.Elements.SetName("/Subtype", "/Type0");
            base.Elements.SetName("/Encoding", vertical ? "/Identity-V" : "/Identity-H");
            OpenTypeDescriptor descriptor = (OpenTypeDescriptor) FontDescriptorStock.Global.CreateDescriptor(idName, fontData);
            base.fontDescriptor = new PdfFontDescriptor(document, descriptor);
            this.fontOptions = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            base.cmapInfo = new CMapInfo(descriptor);
            this.descendantFont = new PdfCIDFont(document, base.fontDescriptor, fontData);
            this.descendantFont.CMapInfo = base.cmapInfo;
            base.toUnicode = new PdfToUnicodeMap(document, base.cmapInfo);
            document.Internals.AddObject(base.toUnicode);
            base.Elements.Add("/ToUnicode", base.toUnicode);
            this.BaseFont = descriptor.FontName.Replace(" ", "");
            if (!this.BaseFont.Contains("+"))
            {
                this.BaseFont = PdfFont.CreateEmbeddedFontSubsetName(this.BaseFont);
            }
            base.fontDescriptor.FontName = this.BaseFont;
            this.descendantFont.BaseFont = this.BaseFont;
            PdfArray array = new PdfArray(document);
            this.Owner.irefTable.Add(this.descendantFont);
            array.Elements.Add(this.descendantFont.Reference);
            base.Elements["/DescendantFonts"] = array;
        }

        internal override void PrepareForSave()
        {
            base.PrepareForSave();
            OpenTypeDescriptor descriptor = base.fontDescriptor.descriptor;
            StringBuilder builder = new StringBuilder("[");
            if (base.cmapInfo != null)
            {
                int[] glyphIndices = base.cmapInfo.GetGlyphIndices();
                int length = glyphIndices.Length;
                int[] numArray2 = new int[length];
                for (int i = 0; i < length; i++)
                {
                    numArray2[i] = descriptor.GlyphIndexToPdfWidth(glyphIndices[i]);
                }
                for (int j = 0; j < length; j++)
                {
                    builder.AppendFormat("{0}[{1}]", glyphIndices[j], numArray2[j]);
                }
                builder.Append("]");
                this.descendantFont.Elements.SetValue("/W", new PdfLiteral(builder.ToString()));
            }
            this.descendantFont.PrepareForSave();
            base.toUnicode.PrepareForSave();
        }

        public string BaseFont
        {
            get => 
                base.Elements.GetName("/BaseFont");
            set
            {
                base.Elements.SetName("/BaseFont", value);
            }
        }

        internal PdfCIDFont DescendantFont =>
            this.descendantFont;

        private XPdfFontOptions FontOptions =>
            this.fontOptions;

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public sealed class Keys : PdfFont.Keys
        {
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string BaseFont = "/BaseFont";
            [KeyInfo(KeyType.Required | KeyType.Array)]
            public const string DescendantFonts = "/DescendantFonts";
            [KeyInfo(KeyType.Required | KeyType.StreamOrName)]
            public const string Encoding = "/Encoding";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string Subtype = "/Subtype";
            [KeyInfo(KeyType.Optional | KeyType.Stream)]
            public const string ToUnicode = "/ToUnicode";
            [KeyInfo(KeyType.Required | KeyType.Name, FixedValue="Font")]
            public const string Type = "/Type";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfType0Font.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

