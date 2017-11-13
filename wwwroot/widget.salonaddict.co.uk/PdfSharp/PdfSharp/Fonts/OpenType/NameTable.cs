namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;
    using System.Text;

    internal class NameTable : OpenTypeFontTable
    {
        private byte[] bytes;
        public ushort count;
        public ushort format;
        public string Name;
        public ushort stringOffset;
        public string Style;
        public const string Tag = "name";

        public NameTable(FontData fontData) : base(fontData, "name")
        {
            this.Name = string.Empty;
            this.Style = string.Empty;
            this.Read();
        }

        public void Read()
        {
            try
            {
                this.bytes = new byte[base.DirectoryEntry.PaddedLength];
                Buffer.BlockCopy(base.fontData.Data, base.DirectoryEntry.Offset, this.bytes, 0, base.DirectoryEntry.Length);
                this.format = base.fontData.ReadUShort();
                this.count = base.fontData.ReadUShort();
                this.stringOffset = base.fontData.ReadUShort();
                for (int i = 0; i < this.count; i++)
                {
                    NameRecord record = this.ReadNameRecord();
                    byte[] dst = new byte[record.length];
                    Buffer.BlockCopy(base.fontData.Data, (base.DirectoryEntry.Offset + this.stringOffset) + record.offset, dst, 0, record.length);
                    if ((record.platformID == 0) || (record.platformID == 3))
                    {
                        if (((record.nameID == 1) && (record.languageID == 0x409)) && string.IsNullOrEmpty(this.Name))
                        {
                            this.Name = Encoding.BigEndianUnicode.GetString(dst, 0, dst.Length);
                        }
                        if (((record.nameID == 2) && (record.languageID == 0x409)) && string.IsNullOrEmpty(this.Style))
                        {
                            this.Style = Encoding.BigEndianUnicode.GetString(dst, 0, dst.Length);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }

        private NameRecord ReadNameRecord() => 
            new NameRecord { 
                platformID = base.fontData.ReadUShort(),
                encodingID = base.fontData.ReadUShort(),
                languageID = base.fontData.ReadUShort(),
                nameID = base.fontData.ReadUShort(),
                length = base.fontData.ReadUShort(),
                offset = base.fontData.ReadUShort()
            };

        private class NameRecord
        {
            public ushort encodingID;
            public ushort languageID;
            public ushort length;
            public ushort nameID;
            public ushort offset;
            public ushort platformID;
        }
    }
}

