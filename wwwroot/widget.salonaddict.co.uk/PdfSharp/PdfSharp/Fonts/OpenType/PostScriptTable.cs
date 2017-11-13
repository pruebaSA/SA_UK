namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class PostScriptTable : OpenTypeFontTable
    {
        public int formatType;
        public ulong isFixedPitch;
        public float italicAngle;
        public ulong maxMemType1;
        public ulong maxMemType42;
        public ulong minMemType1;
        public ulong minMemType42;
        public const string Tag = "post";
        public short underlinePosition;
        public short underlineThickness;

        public PostScriptTable(FontData fontData) : base(fontData, "post")
        {
            this.Read();
        }

        public void Read()
        {
            try
            {
                this.formatType = base.fontData.ReadFixed();
                this.italicAngle = ((float) base.fontData.ReadFixed()) / 65536f;
                this.underlinePosition = base.fontData.ReadFWord();
                this.underlineThickness = base.fontData.ReadFWord();
                this.isFixedPitch = base.fontData.ReadULong();
                this.minMemType42 = base.fontData.ReadULong();
                this.maxMemType42 = base.fontData.ReadULong();
                this.minMemType1 = base.fontData.ReadULong();
                this.maxMemType1 = base.fontData.ReadULong();
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }
    }
}

