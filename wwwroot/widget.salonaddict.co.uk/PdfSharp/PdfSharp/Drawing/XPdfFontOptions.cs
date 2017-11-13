namespace PdfSharp.Drawing
{
    using PdfSharp.Pdf;
    using System;

    public class XPdfFontOptions
    {
        private PdfFontEmbedding fontEmbedding;
        private PdfFontEncoding fontEncoding;

        internal XPdfFontOptions()
        {
        }

        public XPdfFontOptions(PdfFontEmbedding embedding)
        {
            this.fontEncoding = PdfFontEncoding.WinAnsi;
            this.fontEmbedding = embedding;
        }

        public XPdfFontOptions(PdfFontEncoding encoding)
        {
            this.fontEncoding = encoding;
            this.fontEmbedding = PdfFontEmbedding.None;
        }

        [Obsolete("Use other constructor")]
        public XPdfFontOptions(bool embed) : this(embed, false, "", "")
        {
        }

        [Obsolete("Use other constructor")]
        public XPdfFontOptions(PdfFontEmbedding fontEmbedding, bool unicode) : this((fontEmbedding == PdfFontEmbedding.Always) || (fontEmbedding == PdfFontEmbedding.Automatic), unicode, "", "")
        {
        }

        public XPdfFontOptions(PdfFontEncoding encoding, PdfFontEmbedding embedding)
        {
            this.fontEncoding = encoding;
            this.fontEmbedding = embedding;
        }

        [Obsolete("Use other constructor")]
        public XPdfFontOptions(bool unicode, byte[] fontData)
        {
            this.fontEmbedding = PdfFontEmbedding.None;
            this.fontEncoding = unicode ? PdfFontEncoding.Unicode : PdfFontEncoding.WinAnsi;
        }

        [Obsolete("Use other constructor")]
        public XPdfFontOptions(bool embed, bool unicode) : this(embed, unicode, "", "")
        {
        }

        [Obsolete("Use other constructor")]
        public XPdfFontOptions(bool embed, string baseFont) : this(embed, false, baseFont, "")
        {
        }

        [Obsolete("Use other constructor")]
        private XPdfFontOptions(bool embed, bool unicode, string baseFont, string fontFile)
        {
            this.fontEmbedding = embed ? PdfFontEmbedding.Always : PdfFontEmbedding.None;
            this.fontEncoding = unicode ? PdfFontEncoding.Unicode : PdfFontEncoding.WinAnsi;
            this.fontEmbedding = PdfFontEmbedding.Default;
            this.fontEncoding = PdfFontEncoding.WinAnsi;
        }

        [Obsolete("Not yet implemented")]
        public string BaseFont =>
            "";

        [Obsolete("Use FontEmbedding")]
        public bool Embed =>
            (this.fontEmbedding != PdfFontEmbedding.None);

        [Obsolete("Not yet implemented")]
        public byte[] FontData =>
            null;

        public PdfFontEmbedding FontEmbedding =>
            this.fontEmbedding;

        public PdfFontEncoding FontEncoding =>
            this.fontEncoding;

        [Obsolete("Not yet implemented")]
        public string FontFile =>
            "";

        [Obsolete("Use FontEncoding")]
        public bool Unicode =>
            (this.fontEncoding == PdfFontEncoding.Unicode);
    }
}

