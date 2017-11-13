namespace PdfSharp.Pdf.Content
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Content.Objects;
    using System;

    public sealed class ContentReader
    {
        private ContentReader()
        {
        }

        public static CSequence ReadContent(PdfPage page)
        {
            CParser parser = new CParser(page);
            return parser.ReadContent();
        }

        public static CSequence ReadContent(byte[] content)
        {
            CParser parser = new CParser(content);
            return parser.ReadContent();
        }
    }
}

