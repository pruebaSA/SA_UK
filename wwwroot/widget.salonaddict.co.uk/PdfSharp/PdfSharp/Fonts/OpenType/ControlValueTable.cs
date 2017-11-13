namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class ControlValueTable : OpenTypeFontTable
    {
        private short[] array;
        public const string Tag = "cvt ";

        public ControlValueTable(FontData fontData) : base(fontData, "cvt ")
        {
            base.DirectoryEntry.Tag = "cvt ";
            base.DirectoryEntry = fontData.tableDictionary["cvt "];
            this.Read();
        }

        public void Read()
        {
            try
            {
                int num = base.DirectoryEntry.Length / 2;
                this.array = new short[num];
                for (int i = 0; i < num; i++)
                {
                    this.array[i] = base.fontData.ReadFWord();
                }
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }
    }
}

