namespace MigraDoc.Rendering.ChartMapper
{
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.Rendering;
    using PdfSharp.Charting;
    using PdfSharp.Drawing;
    using System;

    internal class FillFormatMapper
    {
        private FillFormatMapper()
        {
        }

        internal static void Map(FillFormat fillFormat, FillFormat domFillFormat)
        {
            new FillFormatMapper().MapObject(fillFormat, domFillFormat);
        }

        private void MapObject(FillFormat fillFormat, FillFormat domFillFormat)
        {
            if (domFillFormat.Color.IsEmpty)
            {
                fillFormat.set_Color(XColor.Empty);
            }
            else
            {
                fillFormat.set_Color(ColorHelper.ToXColor(domFillFormat.Color, domFillFormat.Document.UseCmykColor));
            }
            fillFormat.set_Visible(domFillFormat.Visible);
        }
    }
}

