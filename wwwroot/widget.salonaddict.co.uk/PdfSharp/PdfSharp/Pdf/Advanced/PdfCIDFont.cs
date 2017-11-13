namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing;
    using PdfSharp.Fonts.OpenType;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Filters;
    using System;

    internal class PdfCIDFont : PdfFont
    {
        public PdfCIDFont(PdfDocument document) : base(document)
        {
        }

        public PdfCIDFont(PdfDocument document, PdfFontDescriptor fontDescriptor, XFont font) : base(document)
        {
            base.Elements.SetName("/Type", "/Font");
            base.Elements.SetName("/Subtype", "/CIDFontType2");
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Elements.SetString("/Ordering", "Identity");
            dictionary.Elements.SetString("/Registry", "Adobe");
            dictionary.Elements.SetInteger("/Supplement", 0);
            base.Elements.SetValue("/CIDSystemInfo", dictionary);
            base.fontDescriptor = fontDescriptor;
            this.Owner.irefTable.Add(fontDescriptor);
            base.Elements["/FontDescriptor"] = fontDescriptor.Reference;
            base.FontEncoding = font.PdfOptions.FontEncoding;
            base.FontEmbedding = font.PdfOptions.FontEmbedding;
        }

        public PdfCIDFont(PdfDocument document, PdfFontDescriptor fontDescriptor, byte[] fontData) : base(document)
        {
            base.Elements.SetName("/Type", "/Font");
            base.Elements.SetName("/Subtype", "/CIDFontType2");
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Elements.SetString("/Ordering", "Identity");
            dictionary.Elements.SetString("/Registry", "Adobe");
            dictionary.Elements.SetInteger("/Supplement", 0);
            base.Elements.SetValue("/CIDSystemInfo", dictionary);
            base.fontDescriptor = fontDescriptor;
            this.Owner.irefTable.Add(fontDescriptor);
            base.Elements["/FontDescriptor"] = fontDescriptor.Reference;
            base.FontEncoding = PdfFontEncoding.Unicode;
            base.FontEmbedding = PdfFontEmbedding.Always;
        }

        internal override void PrepareForSave()
        {
            base.PrepareForSave();
            FontData fontData = null;
            if (base.fontDescriptor.descriptor.fontData.loca == null)
            {
                fontData = base.fontDescriptor.descriptor.fontData;
            }
            else
            {
                fontData = base.fontDescriptor.descriptor.fontData.CreateFontSubSet(base.cmapInfo.GlyphIndices, true);
            }
            byte[] buffer = fontData.Data;
            PdfDictionary dictionary = new PdfDictionary(this.Owner);
            this.Owner.Internals.AddObject(dictionary);
            base.fontDescriptor.Elements["/FontFile2"] = dictionary.Reference;
            dictionary.Elements["/Length1"] = new PdfInteger(buffer.Length);
            if (!this.Owner.Options.NoCompression)
            {
                buffer = Filtering.FlateDecode.Encode(buffer);
                dictionary.Elements["/Filter"] = new PdfName("/FlateDecode");
            }
            dictionary.Elements["/Length"] = new PdfInteger(buffer.Length);
            dictionary.CreateStream(buffer);
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

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public sealed class Keys : PdfFont.Keys
        {
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string BaseFont = "/BaseFont";
            [KeyInfo(KeyType.Required | KeyType.Dictionary)]
            public const string CIDSystemInfo = "/CIDSystemInfo";
            [KeyInfo(KeyType.StreamOrName | KeyType.Dictionary)]
            public const string CIDToGIDMap = "/CIDToGIDMap";
            [KeyInfo(KeyType.Integer)]
            public const string DW = "/DW";
            [KeyInfo(KeyType.Array)]
            public const string DW2 = "/DW2";
            [KeyInfo(KeyType.MustBeIndirect | KeyType.Dictionary, typeof(PdfFontDescriptor))]
            public const string FontDescriptor = "/FontDescriptor";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string Subtype = "/Subtype";
            [KeyInfo(KeyType.Required | KeyType.Name, FixedValue="Font")]
            public const string Type = "/Type";
            [KeyInfo(KeyType.Array, typeof(PdfArray))]
            public const string W = "/W";
            [KeyInfo(KeyType.Array, typeof(PdfArray))]
            public const string W2 = "/W2";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfCIDFont.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

