namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class MaximumProfileTable : OpenTypeFontTable
    {
        public ushort maxComponentDepth;
        public ushort maxComponentElements;
        public ushort maxCompositeContours;
        public ushort maxCompositePoints;
        public ushort maxContours;
        public ushort maxFunctionDefs;
        public ushort maxInstructionDefs;
        public ushort maxPoints;
        public ushort maxSizeOfInstructions;
        public ushort maxStackElements;
        public ushort maxStorage;
        public ushort maxTwilightPoints;
        public ushort maxZones;
        public ushort numGlyphs;
        public const string Tag = "maxp";
        public int version;

        public MaximumProfileTable(FontData fontData) : base(fontData, "maxp")
        {
            this.Read();
        }

        public void Read()
        {
            try
            {
                this.version = base.fontData.ReadFixed();
                this.numGlyphs = base.fontData.ReadUShort();
                this.maxPoints = base.fontData.ReadUShort();
                this.maxContours = base.fontData.ReadUShort();
                this.maxCompositePoints = base.fontData.ReadUShort();
                this.maxCompositeContours = base.fontData.ReadUShort();
                this.maxZones = base.fontData.ReadUShort();
                this.maxTwilightPoints = base.fontData.ReadUShort();
                this.maxStorage = base.fontData.ReadUShort();
                this.maxFunctionDefs = base.fontData.ReadUShort();
                this.maxInstructionDefs = base.fontData.ReadUShort();
                this.maxStackElements = base.fontData.ReadUShort();
                this.maxSizeOfInstructions = base.fontData.ReadUShort();
                this.maxComponentElements = base.fontData.ReadUShort();
                this.maxComponentDepth = base.fontData.ReadUShort();
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }
    }
}

