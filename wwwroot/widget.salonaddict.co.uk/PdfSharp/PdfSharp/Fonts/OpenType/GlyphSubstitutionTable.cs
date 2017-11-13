namespace PdfSharp.Fonts.OpenType
{
    using System;

    internal class GlyphSubstitutionTable : OpenTypeFontTable
    {
        public const string Tag = "GSUB";

        public GlyphSubstitutionTable(FontData fontData) : base(fontData, "GSUB")
        {
            base.DirectoryEntry.Tag = "GSUB";
            base.DirectoryEntry = fontData.tableDictionary["GSUB"];
            this.Read();
        }

        public void Read()
        {
        }
    }
}

