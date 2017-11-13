namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;

    internal class FontHandler
    {
        internal static XBrush FontColorToXBrush(Font font) => 
            new XSolidBrush(ColorHelper.ToXColor(font.Color, font.Document.UseCmykColor));

        internal static XFont FontToXFont(Font font, XPrivateFontCollection pfc, PdfFontEncoding encoding, PdfFontEmbedding fontEmbedding)
        {
            XFont font2 = null;
            XPdfFontOptions pdfOptions = null;
            pdfOptions = new XPdfFontOptions(encoding, fontEmbedding);
            XFontStyle xStyle = GetXStyle(font);
            if (font2 == null)
            {
                font2 = new XFont(font.Name, (double) font.Size, xStyle, pdfOptions);
            }
            return font2;
        }

        internal static XUnit GetAscent(XFont font)
        {
            XUnit unit = font.Metrics.Ascent * font.Size;
            return (((double) unit) / ((double) font.FontFamily.GetEmHeight(font.Style)));
        }

        internal static XUnit GetDescent(XFont font)
        {
            XUnit unit = -font.Metrics.Descent;
            unit = ((double) unit) * font.Size;
            return (((double) unit) / ((double) font.FontFamily.GetEmHeight(font.Style)));
        }

        internal static double GetSubSuperScaling(XFont font) => 
            ((0.8 * ((double) GetAscent(font))) / font.GetHeight());

        internal static XFontStyle GetXStyle(Font font)
        {
            XFontStyle regular = XFontStyle.Regular;
            if (font.Bold)
            {
                if (font.Italic)
                {
                    return XFontStyle.BoldItalic;
                }
                return XFontStyle.Bold;
            }
            if (font.Italic)
            {
                regular = XFontStyle.Italic;
            }
            return regular;
        }

        internal static XFont ToSubSuperFont(XFont font) => 
            new XFont(font.Name, font.Size * GetSubSuperScaling(font), font.Style, font.PdfOptions);
    }
}

