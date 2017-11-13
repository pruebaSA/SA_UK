namespace MigraDoc.Rendering.ChartMapper
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.Rendering;
    using PdfSharp.Charting;
    using PdfSharp.Drawing;
    using System;

    internal class FontMapper
    {
        private FontMapper()
        {
        }

        internal static void Map(Font font, Font domFont)
        {
            new FontMapper().MapObject(font, domFont);
        }

        internal static void Map(Font font, Document domDocument, string domStyleName)
        {
            Style style = domDocument.Styles[domStyleName];
            if (style != null)
            {
                new FontMapper().MapObject(font, style.Font);
            }
        }

        private void MapObject(Font font, Font domFont)
        {
            font.set_Bold(domFont.Bold);
            if (domFont.Color.IsEmpty)
            {
                font.set_Color(XColor.Empty);
            }
            else
            {
                font.set_Color(ColorHelper.ToXColor(domFont.Color, domFont.Document.UseCmykColor));
            }
            font.set_Italic(domFont.Italic);
            if (!domFont.IsNull("Name"))
            {
                font.set_Name(domFont.Name);
            }
            if (!domFont.IsNull("Size"))
            {
                font.set_Size(domFont.Size.Point);
            }
            font.set_Subscript(domFont.Subscript);
            font.set_Superscript(domFont.Superscript);
            font.set_Underline((Underline) domFont.Underline);
        }
    }
}

