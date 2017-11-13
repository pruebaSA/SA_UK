namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class CMapTable : OpenTypeFontTable
    {
        public CMap4 cmap4;
        public ushort numTables;
        public bool symbol;
        public const string Tag = "cmap";
        public ushort version;

        public CMapTable(FontData fontData) : base(fontData, "cmap")
        {
            this.Read();
        }

        internal void Read()
        {
            try
            {
                int position = base.fontData.Position;
                this.version = base.fontData.ReadUShort();
                this.numTables = base.fontData.ReadUShort();
                bool flag = false;
                for (int i = 0; i < this.numTables; i++)
                {
                    PlatformId id = (PlatformId) base.fontData.ReadUShort();
                    WinEncodingId encodingId = (WinEncodingId) base.fontData.ReadUShort();
                    int num3 = base.fontData.ReadLong();
                    int num4 = base.fontData.Position;
                    if ((id == PlatformId.Win) && ((encodingId == WinEncodingId.Symbol) || (encodingId == WinEncodingId.Unicode)))
                    {
                        this.symbol = encodingId == WinEncodingId.Symbol;
                        base.fontData.Position = position + num3;
                        this.cmap4 = new CMap4(base.fontData, encodingId);
                        base.fontData.Position = num4;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    throw new InvalidOperationException("Font has no usable platform or encoding ID. It cannot be used with PDFsharp.");
                }
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }
    }
}

