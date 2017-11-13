namespace PdfSharp.Fonts.OpenType
{
    using System;

    internal class OpenTypeFontTable : ICloneable
    {
        public TableDirectoryEntry DirectoryEntry;
        internal PdfSharp.Fonts.OpenType.FontData fontData;

        public OpenTypeFontTable(PdfSharp.Fonts.OpenType.FontData fontData, string tag)
        {
            this.fontData = fontData;
            if ((fontData != null) && fontData.tableDictionary.ContainsKey(tag))
            {
                this.DirectoryEntry = fontData.tableDictionary[tag];
            }
            else
            {
                this.DirectoryEntry = new TableDirectoryEntry(tag);
            }
            this.DirectoryEntry.FontTable = this;
        }

        public static uint CalcChecksum(byte[] bytes)
        {
            uint num2;
            uint num3;
            uint num4;
            uint num = num2 = num3 = num4 = 0;
            int length = bytes.Length;
            int num6 = 0;
            while (num6 < length)
            {
                num += bytes[num6++];
                num2 += bytes[num6++];
                num3 += bytes[num6++];
                num4 += bytes[num6++];
            }
            return ((((num << 0x18) + (num2 << 0x10)) + (num3 << 8)) + num4);
        }

        public object Clone() => 
            this.DeepCopy();

        protected virtual OpenTypeFontTable DeepCopy()
        {
            OpenTypeFontTable table = (OpenTypeFontTable) base.MemberwiseClone();
            table.DirectoryEntry.Offset = 0;
            table.DirectoryEntry.FontTable = table;
            return table;
        }

        public virtual void PrepareForCompilation()
        {
        }

        public virtual void Write(OpenTypeFontWriter writer)
        {
        }

        public PdfSharp.Fonts.OpenType.FontData FontData =>
            this.fontData;
    }
}

