namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class OS2Table : OpenTypeFontTable
    {
        public string achVendID;
        public ushort fsSelection;
        public ushort fsType;
        public byte[] panose;
        public short sCapHeight;
        public short sFamilyClass;
        public short sTypoAscender;
        public short sTypoDescender;
        public short sTypoLineGap;
        public short sxHeight;
        public const string Tag = "OS/2";
        public uint ulCodePageRange1;
        public uint ulCodePageRange2;
        public uint ulUnicodeRange1;
        public uint ulUnicodeRange2;
        public uint ulUnicodeRange3;
        public uint ulUnicodeRange4;
        public ushort usBreakChar;
        public ushort usDefaultChar;
        public ushort usFirstCharIndex;
        public ushort usLastCharIndex;
        public ushort usMaxContext;
        public ushort usWeightClass;
        public ushort usWidthClass;
        public ushort usWinAscent;
        public ushort usWinDescent;
        public ushort version;
        public short xAvgCharWidth;
        public short yStrikeoutPosition;
        public short yStrikeoutSize;
        public short ySubscriptXOffset;
        public short ySubscriptXSize;
        public short ySubscriptYOffset;
        public short ySubscriptYSize;
        public short ySuperscriptXOffset;
        public short ySuperscriptXSize;
        public short ySuperscriptYOffset;
        public short ySuperscriptYSize;

        public OS2Table(FontData fontData) : base(fontData, "OS/2")
        {
            this.Read();
        }

        public void Read()
        {
            try
            {
                this.version = base.fontData.ReadUShort();
                this.xAvgCharWidth = base.fontData.ReadShort();
                this.usWeightClass = base.fontData.ReadUShort();
                this.usWidthClass = base.fontData.ReadUShort();
                this.fsType = base.fontData.ReadUShort();
                this.ySubscriptXSize = base.fontData.ReadShort();
                this.ySubscriptYSize = base.fontData.ReadShort();
                this.ySubscriptXOffset = base.fontData.ReadShort();
                this.ySubscriptYOffset = base.fontData.ReadShort();
                this.ySuperscriptXSize = base.fontData.ReadShort();
                this.ySuperscriptYSize = base.fontData.ReadShort();
                this.ySuperscriptXOffset = base.fontData.ReadShort();
                this.ySuperscriptYOffset = base.fontData.ReadShort();
                this.yStrikeoutSize = base.fontData.ReadShort();
                this.yStrikeoutPosition = base.fontData.ReadShort();
                this.sFamilyClass = base.fontData.ReadShort();
                this.panose = base.fontData.ReadBytes(10);
                this.ulUnicodeRange1 = base.fontData.ReadULong();
                this.ulUnicodeRange2 = base.fontData.ReadULong();
                this.ulUnicodeRange3 = base.fontData.ReadULong();
                this.ulUnicodeRange4 = base.fontData.ReadULong();
                this.achVendID = base.fontData.ReadString(4);
                this.fsSelection = base.fontData.ReadUShort();
                this.usFirstCharIndex = base.fontData.ReadUShort();
                this.usLastCharIndex = base.fontData.ReadUShort();
                this.sTypoAscender = base.fontData.ReadShort();
                this.sTypoDescender = base.fontData.ReadShort();
                this.sTypoLineGap = base.fontData.ReadShort();
                this.usWinAscent = base.fontData.ReadUShort();
                this.usWinDescent = base.fontData.ReadUShort();
                if (this.version >= 1)
                {
                    this.ulCodePageRange1 = base.fontData.ReadULong();
                    this.ulCodePageRange2 = base.fontData.ReadULong();
                    if (this.version >= 2)
                    {
                        this.sxHeight = base.fontData.ReadShort();
                        this.sCapHeight = base.fontData.ReadShort();
                        this.usDefaultChar = base.fontData.ReadUShort();
                        this.usBreakChar = base.fontData.ReadUShort();
                        this.usMaxContext = base.fontData.ReadUShort();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }

        [Flags]
        public enum FontSelectionFlags : ushort
        {
            Bold = 0x20,
            Italic = 1,
            Regular = 0x40
        }
    }
}

