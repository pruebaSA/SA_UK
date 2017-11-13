namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp.Drawing;
    using PdfSharp.Fonts;
    using System;

    public static class ExternalHelper
    {
        public static byte[] F74167FFE4044F53B28A4AF049E9EF25(XFont font, XPdfFontOptions options, bool subset)
        {
            if (subset)
            {
                OpenTypeDescriptor descriptor = new OpenTypeDescriptor(font, options);
                FontData fontData = descriptor.fontData;
                CMapInfo info = new CMapInfo(descriptor);
                info.AddAnsiChars();
                return fontData.CreateFontSubSet(info.GlyphIndices, false).Data;
            }
            FontData data2 = new FontData(font, options);
            return data2.Data;
        }
    }
}

