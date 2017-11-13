namespace PdfSharp.Fonts.OpenType
{
    using System;

    internal class IRefFontTable : OpenTypeFontTable
    {
        private TableDirectoryEntry irefDirectoryEntry;

        public IRefFontTable(FontData fontData, OpenTypeFontTable fontTable) : base(null, fontTable.DirectoryEntry.Tag)
        {
            base.fontData = fontData;
            this.irefDirectoryEntry = fontTable.DirectoryEntry;
        }

        public override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            base.DirectoryEntry.Length = this.irefDirectoryEntry.Length;
            base.DirectoryEntry.CheckSum = this.irefDirectoryEntry.CheckSum;
        }

        public override void Write(OpenTypeFontWriter writer)
        {
            writer.Write(this.irefDirectoryEntry.FontTable.fontData.Data, this.irefDirectoryEntry.Offset, this.irefDirectoryEntry.PaddedLength);
        }
    }
}

