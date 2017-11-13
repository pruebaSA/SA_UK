namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class HorizontalHeaderTable : OpenTypeFontTable
    {
        public ushort advanceWidthMax;
        public short ascender;
        public short caretSlopeRise;
        public short caretSlopeRun;
        public short descender;
        public short lineGap;
        public short metricDataFormat;
        public short minLeftSideBearing;
        public short minRightSideBearing;
        public ushort numberOfHMetrics;
        public short reserved1;
        public short reserved2;
        public short reserved3;
        public short reserved4;
        public short reserved5;
        public const string Tag = "hhea";
        public int version;
        public short xMaxExtent;

        public HorizontalHeaderTable(FontData fontData) : base(fontData, "hhea")
        {
            this.Read();
        }

        public void Read()
        {
            try
            {
                this.version = base.fontData.ReadFixed();
                this.ascender = base.fontData.ReadFWord();
                this.descender = base.fontData.ReadFWord();
                this.lineGap = base.fontData.ReadFWord();
                this.advanceWidthMax = base.fontData.ReadUFWord();
                this.minLeftSideBearing = base.fontData.ReadFWord();
                this.minRightSideBearing = base.fontData.ReadFWord();
                this.xMaxExtent = base.fontData.ReadFWord();
                this.caretSlopeRise = base.fontData.ReadShort();
                this.caretSlopeRun = base.fontData.ReadShort();
                this.reserved1 = base.fontData.ReadShort();
                this.reserved2 = base.fontData.ReadShort();
                this.reserved3 = base.fontData.ReadShort();
                this.reserved4 = base.fontData.ReadShort();
                this.reserved5 = base.fontData.ReadShort();
                this.metricDataFormat = base.fontData.ReadShort();
                this.numberOfHMetrics = base.fontData.ReadUShort();
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }
    }
}

