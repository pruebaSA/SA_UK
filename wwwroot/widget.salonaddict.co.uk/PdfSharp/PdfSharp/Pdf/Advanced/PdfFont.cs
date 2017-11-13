namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Fonts;
    using PdfSharp.Pdf;
    using System;
    using System.Text;

    public class PdfFont : PdfDictionary
    {
        internal PdfSharp.Fonts.CMapInfo cmapInfo;
        internal PdfFontDescriptor fontDescriptor;
        internal PdfFontEmbedding FontEmbedding;
        internal PdfFontEncoding FontEncoding;
        internal PdfToUnicodeMap toUnicode;

        public PdfFont(PdfDocument document) : base(document)
        {
        }

        internal void AddChars(string text)
        {
            if (this.cmapInfo != null)
            {
                this.cmapInfo.AddChars(text);
            }
        }

        internal void AddGlyphIndices(string glyphIndices)
        {
            if (this.cmapInfo != null)
            {
                this.cmapInfo.AddGlyphIndices(glyphIndices);
            }
        }

        internal static string CreateEmbeddedFontSubsetName(string name)
        {
            StringBuilder builder = new StringBuilder(0x40);
            byte[] buffer = Guid.NewGuid().ToByteArray();
            for (int i = 0; i < 6; i++)
            {
                builder.Append((char) (0x41 + (buffer[i] % 0x1a)));
            }
            builder.Append('+');
            if (name.StartsWith("/"))
            {
                builder.Append(name.Substring(1));
            }
            else
            {
                builder.Append(name);
            }
            return builder.ToString();
        }

        internal PdfSharp.Fonts.CMapInfo CMapInfo
        {
            get => 
                this.cmapInfo;
            set
            {
                this.cmapInfo = value;
            }
        }

        internal PdfFontDescriptor FontDescriptor =>
            this.fontDescriptor;

        public bool IsSymbolFont =>
            this.fontDescriptor.IsSymbolFont;

        internal PdfToUnicodeMap ToUnicodeMap
        {
            get => 
                this.toUnicode;
            set
            {
                this.toUnicode = value;
            }
        }

        public class Keys : KeysBase
        {
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string BaseFont = "/BaseFont";
            [KeyInfo(KeyType.MustBeIndirect | KeyType.Dictionary, typeof(PdfFontDescriptor))]
            public const string FontDescriptor = "/FontDescriptor";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string Subtype = "/Subtype";
            [KeyInfo(KeyType.Required | KeyType.Name, FixedValue="Font")]
            public const string Type = "/Type";
        }
    }
}

