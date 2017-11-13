namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class CMap4 : OpenTypeFontTable
    {
        public WinEncodingId encodingId;
        public ushort[] endCount;
        public ushort entrySelector;
        public ushort format;
        public int glyphCount;
        public ushort[] glyphIdArray;
        public short[] idDelta;
        public ushort[] idRangeOffs;
        public ushort language;
        public ushort length;
        public ushort rangeShift;
        public ushort searchRange;
        public ushort segCountX2;
        public ushort[] startCount;

        public CMap4(FontData fontData, WinEncodingId encodingId) : base(fontData, "----")
        {
            this.encodingId = encodingId;
            this.Read();
        }

        internal void Read()
        {
            try
            {
                this.format = base.fontData.ReadUShort();
                this.length = base.fontData.ReadUShort();
                this.language = base.fontData.ReadUShort();
                this.segCountX2 = base.fontData.ReadUShort();
                this.searchRange = base.fontData.ReadUShort();
                this.entrySelector = base.fontData.ReadUShort();
                this.rangeShift = base.fontData.ReadUShort();
                int num = this.segCountX2 / 2;
                this.glyphCount = (this.length - (0x10 + (8 * num))) / 2;
                this.endCount = new ushort[num];
                this.startCount = new ushort[num];
                this.idDelta = new short[num];
                this.idRangeOffs = new ushort[num];
                this.glyphIdArray = new ushort[this.glyphCount];
                for (int i = 0; i < num; i++)
                {
                    this.endCount[i] = base.fontData.ReadUShort();
                }
                base.fontData.ReadUShort();
                for (int j = 0; j < num; j++)
                {
                    this.startCount[j] = base.fontData.ReadUShort();
                }
                for (int k = 0; k < num; k++)
                {
                    this.idDelta[k] = base.fontData.ReadShort();
                }
                for (int m = 0; m < num; m++)
                {
                    this.idRangeOffs[m] = base.fontData.ReadUShort();
                }
                for (int n = 0; n < this.glyphCount; n++)
                {
                    this.glyphIdArray[n] = base.fontData.ReadUShort();
                }
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }
    }
}

