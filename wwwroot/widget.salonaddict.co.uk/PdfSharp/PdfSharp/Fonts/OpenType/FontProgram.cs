namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class FontProgram : OpenTypeFontTable
    {
        private byte[] bytes;
        public const string Tag = "fpgm";

        public FontProgram(FontData fontData) : base(fontData, "fpgm")
        {
            base.DirectoryEntry.Tag = "fpgm";
            base.DirectoryEntry = fontData.tableDictionary["fpgm"];
            this.Read();
        }

        public void Read()
        {
            try
            {
                int length = base.DirectoryEntry.Length;
                this.bytes = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    this.bytes[i] = base.fontData.ReadByte();
                }
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }
    }
}

