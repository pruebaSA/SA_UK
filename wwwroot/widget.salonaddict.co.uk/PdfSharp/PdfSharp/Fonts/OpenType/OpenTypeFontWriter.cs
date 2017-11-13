namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp.Fonts;
    using System;
    using System.IO;

    internal class OpenTypeFontWriter : FontWriter
    {
        public OpenTypeFontWriter(Stream stream) : base(stream)
        {
        }

        public void WriteTag(string tag)
        {
            base.WriteByte((byte) tag[0]);
            base.WriteByte((byte) tag[1]);
            base.WriteByte((byte) tag[2]);
            base.WriteByte((byte) tag[3]);
        }
    }
}

