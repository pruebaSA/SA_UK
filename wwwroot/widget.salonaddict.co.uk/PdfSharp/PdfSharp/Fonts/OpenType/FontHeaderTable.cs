namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class FontHeaderTable : OpenTypeFontTable
    {
        public uint checkSumAdjustment;
        public long created;
        public ushort flags;
        public short fontDirectionHint;
        public int fontRevision;
        public short glyphDataFormat;
        public short indexToLocFormat;
        public ushort lowestRecPPEM;
        public ushort macStyle;
        public uint magicNumber;
        public long modified;
        public const string Tag = "head";
        public ushort unitsPerEm;
        public int version;
        public short xMax;
        public short xMin;
        public short yMax;
        public short yMin;

        public FontHeaderTable(FontData fontData) : base(fontData, "head")
        {
            this.Read();
        }

        public void Read()
        {
            try
            {
                this.version = base.fontData.ReadFixed();
                this.fontRevision = base.fontData.ReadFixed();
                this.checkSumAdjustment = base.fontData.ReadULong();
                this.magicNumber = base.fontData.ReadULong();
                this.flags = base.fontData.ReadUShort();
                this.unitsPerEm = base.fontData.ReadUShort();
                this.created = base.fontData.ReadLongDate();
                this.modified = base.fontData.ReadLongDate();
                this.xMin = base.fontData.ReadShort();
                this.yMin = base.fontData.ReadShort();
                this.xMax = base.fontData.ReadShort();
                this.yMax = base.fontData.ReadShort();
                this.macStyle = base.fontData.ReadUShort();
                this.lowestRecPPEM = base.fontData.ReadUShort();
                this.fontDirectionHint = base.fontData.ReadShort();
                this.indexToLocFormat = base.fontData.ReadShort();
                this.glyphDataFormat = base.fontData.ReadShort();
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }
    }
}

