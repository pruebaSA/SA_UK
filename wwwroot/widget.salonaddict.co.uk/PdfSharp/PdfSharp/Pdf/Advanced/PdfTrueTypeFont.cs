namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing;
    using PdfSharp.Fonts;
    using PdfSharp.Fonts.OpenType;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Filters;
    using System;

    internal class PdfTrueTypeFont : PdfFont
    {
        private XPdfFontOptions fontOptions;

        public PdfTrueTypeFont(PdfDocument document) : base(document)
        {
        }

        public PdfTrueTypeFont(PdfDocument document, XFont font) : base(document)
        {
            base.Elements.SetName("/Type", "/Font");
            base.Elements.SetName("/Subtype", "/TrueType");
            OpenTypeDescriptor descriptor = (OpenTypeDescriptor) FontDescriptorStock.Global.CreateDescriptor(font);
            base.fontDescriptor = new PdfFontDescriptor(document, descriptor);
            this.fontOptions = font.PdfOptions;
            base.cmapInfo = new CMapInfo(descriptor);
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
            if (this.fontOptions.FontEmbedding == PdfFontEmbedding.Always)
            {
                this.BaseFont = PdfFont.CreateEmbeddedFontSubsetName(this.BaseFont);
            }
            base.fontDescriptor.FontName = this.BaseFont;
            if (!base.IsSymbolFont)
            {
                this.Encoding = "/WinAnsiEncoding";
            }
            this.Owner.irefTable.Add(base.fontDescriptor);
            base.Elements["/FontDescriptor"] = base.fontDescriptor.Reference;
            base.FontEncoding = font.PdfOptions.FontEncoding;
            base.FontEmbedding = font.PdfOptions.FontEmbedding;
        }

        internal override void PrepareForSave()
        {
            base.PrepareForSave();
            if ((base.FontEmbedding == PdfFontEmbedding.Always) || (base.FontEmbedding == PdfFontEmbedding.Automatic))
            {
                byte[] data = base.fontDescriptor.descriptor.fontData.CreateFontSubSet(base.cmapInfo.GlyphIndices, false).Data;
                PdfDictionary dictionary = new PdfDictionary(this.Owner);
                this.Owner.Internals.AddObject(dictionary);
                base.fontDescriptor.Elements["/FontFile2"] = dictionary.Reference;
                dictionary.Elements["/Length1"] = new PdfInteger(data.Length);
                if (!this.Owner.Options.NoCompression)
                {
                    data = Filtering.FlateDecode.Encode(data);
                    dictionary.Elements["/Filter"] = new PdfName("/FlateDecode");
                }
                dictionary.Elements["/Length"] = new PdfInteger(data.Length);
                dictionary.CreateStream(data);
            }
            this.FirstChar = 0;
            this.LastChar = 0xff;
            PdfArray widths = this.Widths;
            for (int i = 0; i < 0x100; i++)
            {
                widths.Elements.Add(new PdfInteger(base.fontDescriptor.descriptor.widths[i]));
            }
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

        public string Encoding
        {
            get => 
                base.Elements.GetName("/Encoding");
            set
            {
                base.Elements.SetName("/Encoding", value);
            }
        }

        public int FirstChar
        {
            get => 
                base.Elements.GetInteger("/FirstChar");
            set
            {
                base.Elements.SetInteger("/FirstChar", value);
            }
        }

        private XPdfFontOptions FontOptions =>
            this.fontOptions;

        public int LastChar
        {
            get => 
                base.Elements.GetInteger("/LastChar");
            set
            {
                base.Elements.SetInteger("/LastChar", value);
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public PdfArray Widths =>
            ((PdfArray) base.Elements.GetValue("/Widths", VCF.Create));

        public sealed class Keys : PdfFont.Keys
        {
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string BaseFont = "/BaseFont";
            [KeyInfo(KeyType.Dictionary)]
            public const string Encoding = "/Encoding";
            [KeyInfo(KeyType.Integer)]
            public const string FirstChar = "/FirstChar";
            [KeyInfo(KeyType.MustBeIndirect | KeyType.Dictionary, typeof(PdfFontDescriptor))]
            public const string FontDescriptor = "/FontDescriptor";
            [KeyInfo(KeyType.Integer)]
            public const string LastChar = "/LastChar";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string Name = "/Name";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string Subtype = "/Subtype";
            [KeyInfo(KeyType.Optional | KeyType.Stream)]
            public const string ToUnicode = "/ToUnicode";
            [KeyInfo(KeyType.Required | KeyType.Name, FixedValue="Font")]
            public const string Type = "/Type";
            [KeyInfo(KeyType.Array, typeof(PdfArray))]
            public const string Widths = "/Widths";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfTrueTypeFont.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

