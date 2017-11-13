namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class HorizontalMetrics : OpenTypeFontTable
    {
        public ushort advanceWidth;
        public short lsb;
        public const string Tag = "----";

        public HorizontalMetrics(FontData fontData) : base(fontData, "----")
        {
            this.Read();
        }

        public void Read()
        {
            try
            {
                this.advanceWidth = base.fontData.ReadUFWord();
                this.lsb = base.fontData.ReadFWord();
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }
    }
}

