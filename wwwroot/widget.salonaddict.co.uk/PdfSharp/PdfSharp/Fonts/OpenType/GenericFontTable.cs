namespace PdfSharp.Fonts.OpenType
{
    using System;

    internal class GenericFontTable : OpenTypeFontTable
    {
        private byte[] table;

        public GenericFontTable(OpenTypeFontTable fontTable) : base(null, "xxxx")
        {
            base.DirectoryEntry.Tag = fontTable.DirectoryEntry.Tag;
            int length = fontTable.DirectoryEntry.Length;
            if (length > 0)
            {
                this.table = new byte[length];
                Buffer.BlockCopy(fontTable.FontData.Data, fontTable.DirectoryEntry.Offset, this.table, 0, length);
            }
        }

        public GenericFontTable(FontData fontData, string tag) : base(fontData, tag)
        {
            base.fontData = fontData;
        }

        protected override OpenTypeFontTable DeepCopy()
        {
            GenericFontTable table = (GenericFontTable) base.DeepCopy();
            table.table = (byte[]) this.table.Clone();
            return table;
        }
    }
}

