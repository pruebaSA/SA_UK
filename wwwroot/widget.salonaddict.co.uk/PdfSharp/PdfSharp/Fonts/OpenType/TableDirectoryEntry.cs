namespace PdfSharp.Fonts.OpenType
{
    using System;

    internal class TableDirectoryEntry
    {
        public uint CheckSum;
        public OpenTypeFontTable FontTable;
        public int Length;
        public int Offset;
        public string Tag;

        public TableDirectoryEntry()
        {
        }

        public TableDirectoryEntry(string tag)
        {
            this.Tag = tag;
        }

        public void Read(FontData fontData)
        {
            this.Tag = fontData.ReadTag();
            this.CheckSum = fontData.ReadULong();
            this.Offset = fontData.ReadLong();
            this.Length = (int) fontData.ReadULong();
        }

        public static TableDirectoryEntry ReadFrom(FontData fontData) => 
            new TableDirectoryEntry { 
                Tag = fontData.ReadTag(),
                CheckSum = fontData.ReadULong(),
                Offset = fontData.ReadLong(),
                Length = (int) fontData.ReadULong()
            };

        public void Write(OpenTypeFontWriter writer)
        {
            writer.WriteTag(this.Tag);
            writer.WriteUInt(this.CheckSum);
            writer.WriteInt(this.Offset);
            writer.WriteUInt((uint) this.Length);
        }

        public int PaddedLength =>
            ((this.Length + 3) & -4);
    }
}

