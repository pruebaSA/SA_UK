namespace PdfSharp.Fonts.OpenType
{
    using System;
    using System.Collections.Generic;

    internal class GlyphDataTable : OpenTypeFontTable
    {
        private const int ARG_1_AND_2_ARE_WORDS = 1;
        public byte[] glyphTable;
        private const int MORE_COMPONENTS = 0x20;
        public const string Tag = "glyf";
        private const int WE_HAVE_A_SCALE = 8;
        private const int WE_HAVE_A_TWO_BY_TWO = 0x80;
        private const int WE_HAVE_AN_X_AND_Y_SCALE = 0x40;

        public GlyphDataTable() : base(null, "glyf")
        {
            base.DirectoryEntry.Tag = "glyf";
        }

        public GlyphDataTable(FontData fontData) : base(fontData, "glyf")
        {
            base.DirectoryEntry.Tag = "glyf";
            this.Read();
        }

        private void AddCompositeGlyphs(Dictionary<int, object> glyphs, int glyph)
        {
            int offset = this.GetOffset(glyph);
            if (offset == this.GetOffset(glyph + 1))
            {
                return;
            }
            base.fontData.Position = offset;
            if (base.fontData.ReadShort() >= 0)
            {
                return;
            }
            base.fontData.SeekOffset(8);
            while (true)
            {
                int num3 = base.fontData.ReadUFWord();
                int key = base.fontData.ReadUFWord();
                if (!glyphs.ContainsKey(key))
                {
                    glyphs.Add(key, null);
                }
                if ((num3 & 0x20) == 0)
                {
                    return;
                }
                int num5 = ((num3 & 1) == 0) ? 2 : 4;
                if ((num3 & 8) != 0)
                {
                    num5 += 2;
                }
                else if ((num3 & 0x40) != 0)
                {
                    num5 += 4;
                }
                if ((num3 & 0x80) != 0)
                {
                    num5 += 8;
                }
                base.fontData.SeekOffset(num5);
            }
        }

        public void CompleteGlyphClosure(Dictionary<int, object> glyphs)
        {
            int count = glyphs.Count;
            int[] array = new int[glyphs.Count];
            glyphs.Keys.CopyTo(array, 0);
            if (!glyphs.ContainsKey(0))
            {
                glyphs.Add(0, null);
            }
            for (int i = 0; i < count; i++)
            {
                this.AddCompositeGlyphs(glyphs, array[i]);
            }
        }

        public byte[] GetGlyphData(int glyph)
        {
            IndexToLocationTable loca = base.fontData.loca;
            int offset = this.GetOffset(glyph);
            int count = this.GetOffset(glyph + 1) - offset;
            byte[] dst = new byte[count];
            Buffer.BlockCopy(base.fontData.Data, offset, dst, 0, count);
            return dst;
        }

        public int GetGlyphSize(int glyph)
        {
            IndexToLocationTable loca = base.fontData.loca;
            return (this.GetOffset(glyph + 1) - this.GetOffset(glyph));
        }

        public int GetOffset(int glyph) => 
            (base.DirectoryEntry.Offset + base.fontData.loca.locaTable[glyph]);

        public override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            if (base.DirectoryEntry.Length == 0)
            {
                base.DirectoryEntry.Length = this.glyphTable.Length;
            }
            base.DirectoryEntry.CheckSum = OpenTypeFontTable.CalcChecksum(this.glyphTable);
        }

        public void Read()
        {
        }

        public override void Write(OpenTypeFontWriter writer)
        {
            writer.Write(this.glyphTable, 0, base.DirectoryEntry.PaddedLength);
        }
    }
}

