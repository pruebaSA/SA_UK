namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class ControlValueProgram : OpenTypeFontTable
    {
        private byte[] bytes;
        public const string Tag = "prep";

        public ControlValueProgram(FontData fontData) : base(fontData, "prep")
        {
            base.DirectoryEntry.Tag = "prep";
            base.DirectoryEntry = fontData.tableDictionary["prep"];
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

