namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Fonts.OpenType;
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfFontDescriptor : PdfDictionary
    {
        internal OpenTypeDescriptor descriptor;
        private bool isSymbolFont;

        internal PdfFontDescriptor(PdfDocument document, OpenTypeDescriptor descriptor) : base(document)
        {
            this.descriptor = descriptor;
            base.Elements.SetName("/Type", "/FontDescriptor");
            base.Elements.SetInteger("/Ascent", this.descriptor.DesignUnitsToPdf((double) this.descriptor.Ascender));
            base.Elements.SetInteger("/CapHeight", this.descriptor.DesignUnitsToPdf((double) this.descriptor.CapHeight));
            base.Elements.SetInteger("/Descent", this.descriptor.DesignUnitsToPdf((double) this.descriptor.Descender));
            base.Elements.SetInteger("/Flags", (int) this.FlagsFromDescriptor(this.descriptor));
            base.Elements.SetRectangle("/FontBBox", new PdfRectangle((double) this.descriptor.DesignUnitsToPdf((double) this.descriptor.XMin), (double) this.descriptor.DesignUnitsToPdf((double) this.descriptor.YMin), (double) this.descriptor.DesignUnitsToPdf((double) this.descriptor.XMax), (double) this.descriptor.DesignUnitsToPdf((double) this.descriptor.YMax)));
            base.Elements.SetReal("/ItalicAngle", (double) this.descriptor.ItalicAngle);
            base.Elements.SetInteger("/StemV", this.descriptor.StemV);
            base.Elements.SetInteger("/XHeight", this.descriptor.DesignUnitsToPdf((double) this.descriptor.XHeight));
        }

        private PdfFontDescriptorFlags FlagsFromDescriptor(OpenTypeDescriptor descriptor)
        {
            PdfFontDescriptorFlags flags = 0;
            this.isSymbolFont = descriptor.fontData.cmap.symbol;
            return (flags | (descriptor.fontData.cmap.symbol ? PdfFontDescriptorFlags.Symbolic : PdfFontDescriptorFlags.Nonsymbolic));
        }

        public string FontName
        {
            get => 
                base.Elements.GetName("/FontName");
            set
            {
                base.Elements.SetName("/FontName", value);
            }
        }

        public bool IsSymbolFont =>
            this.isSymbolFont;

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public sealed class Keys : KeysBase
        {
            [KeyInfo(KeyType.Required | KeyType.Real)]
            public const string Ascent = "/Ascent";
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string AvgWidth = "/AvgWidth";
            [KeyInfo(KeyType.Required | KeyType.Real)]
            public const string CapHeight = "/CapHeight";
            [KeyInfo(KeyType.Optional | KeyType.String)]
            public const string CharSet = "/CharSet";
            [KeyInfo(KeyType.Required | KeyType.Real)]
            public const string Descent = "/Descent";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string Flags = "/Flags";
            [KeyInfo(KeyType.Required | KeyType.Rectangle)]
            public const string FontBBox = "/FontBBox";
            [KeyInfo(KeyType.Optional | KeyType.String)]
            public const string FontFamily = "/FontFamily";
            [KeyInfo(KeyType.Optional | KeyType.Stream)]
            public const string FontFile = "/FontFile";
            [KeyInfo(KeyType.Optional | KeyType.Stream)]
            public const string FontFile2 = "/FontFile2";
            [KeyInfo(KeyType.Optional | KeyType.Stream)]
            public const string FontFile3 = "/FontFile3";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string FontName = "/FontName";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string FontStretch = "/FontStretch";
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string FontWeight = "/FontWeight";
            [KeyInfo(KeyType.Required | KeyType.Real)]
            public const string ItalicAngle = "/ItalicAngle";
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string Leading = "/Leading";
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string MaxWidth = "/MaxWidth";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string MissingWidth = "/MissingWidth";
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string StemH = "/StemH";
            [KeyInfo(KeyType.Required | KeyType.Real)]
            public const string StemV = "/StemV";
            [KeyInfo(KeyType.Required | KeyType.Name, FixedValue="FontDescriptor")]
            public const string Type = "/Type";
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string XHeight = "/XHeight";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfFontDescriptor.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

