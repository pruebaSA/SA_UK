namespace PdfSharp.Fonts.OpenType
{
    using System;

    internal class IndexToLocationTable : OpenTypeFontTable
    {
        private byte[] bytes;
        internal int[] locaTable;
        public bool ShortIndex;
        public const string Tag = "loca";

        public IndexToLocationTable() : base(null, "loca")
        {
            base.DirectoryEntry.Tag = "loca";
        }

        public IndexToLocationTable(FontData fontData) : base(fontData, "loca")
        {
            base.DirectoryEntry = base.fontData.tableDictionary["loca"];
            this.Read();
        }

        public override void PrepareForCompilation()
        {
            base.DirectoryEntry.Offset = 0;
            if (this.ShortIndex)
            {
                base.DirectoryEntry.Length = this.locaTable.Length * 2;
            }
            else
            {
                base.DirectoryEntry.Length = this.locaTable.Length * 4;
            }
            this.bytes = new byte[base.DirectoryEntry.PaddedLength];
            int length = this.locaTable.Length;
            int num2 = 0;
            if (this.ShortIndex)
            {
                for (int i = 0; i < length; i++)
                {
                    int num4 = this.locaTable[i] / 2;
                    this.bytes[num2++] = (byte) (num4 >> 8);
                    this.bytes[num2++] = (byte) num4;
                }
            }
            else
            {
                for (int j = 0; j < length; j++)
                {
                    int num6 = this.locaTable[j];
                    this.bytes[num2++] = (byte) (num6 >> 0x18);
                    this.bytes[num2++] = (byte) (num6 >> 0x10);
                    this.bytes[num2++] = (byte) (num6 >> 8);
                    this.bytes[num2++] = (byte) num6;
                }
            }
            base.DirectoryEntry.CheckSum = OpenTypeFontTable.CalcChecksum(this.bytes);
        }

        public void Read()
        {
            try
            {
                this.ShortIndex = base.fontData.head.indexToLocFormat == 0;
                base.fontData.Position = base.DirectoryEntry.Offset;
                if (this.ShortIndex)
                {
                    int num = base.DirectoryEntry.Length / 2;
                    this.locaTable = new int[num];
                    for (int i = 0; i < num; i++)
                    {
                        this.locaTable[i] = 2 * base.fontData.ReadUFWord();
                    }
                }
                else
                {
                    int num3 = base.DirectoryEntry.Length / 4;
                    this.locaTable = new int[num3];
                    for (int j = 0; j < num3; j++)
                    {
                        this.locaTable[j] = base.fontData.ReadLong();
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public override void Write(OpenTypeFontWriter writer)
        {
            writer.Write(this.bytes, 0, base.DirectoryEntry.PaddedLength);
        }
    }
}

