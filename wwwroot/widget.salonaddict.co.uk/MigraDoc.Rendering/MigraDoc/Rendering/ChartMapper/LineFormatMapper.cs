namespace MigraDoc.Rendering.ChartMapper
{
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.Rendering;
    using PdfSharp.Charting;
    using PdfSharp.Drawing;
    using System;

    public class LineFormatMapper
    {
        internal static void Map(LineFormat lineFormat, LineFormat domLineFormat)
        {
            new LineFormatMapper().MapObject(lineFormat, domLineFormat);
        }

        private void MapObject(LineFormat lineFormat, LineFormat domLineFormat)
        {
            if (domLineFormat.Color.IsEmpty)
            {
                lineFormat.set_Color(XColor.Empty);
            }
            else
            {
                lineFormat.set_Color(ColorHelper.ToXColor(domLineFormat.Color, domLineFormat.Document.UseCmykColor));
            }
            switch (domLineFormat.DashStyle)
            {
                case DashStyle.Solid:
                    lineFormat.set_DashStyle(XDashStyle.Solid);
                    break;

                case DashStyle.Dash:
                    lineFormat.set_DashStyle(XDashStyle.Dash);
                    break;

                case DashStyle.DashDot:
                    lineFormat.set_DashStyle(XDashStyle.DashDot);
                    break;

                case DashStyle.DashDotDot:
                    lineFormat.set_DashStyle(XDashStyle.DashDotDot);
                    break;

                case DashStyle.SquareDot:
                    lineFormat.set_DashStyle(XDashStyle.Dot);
                    break;

                default:
                    lineFormat.set_DashStyle(XDashStyle.Solid);
                    break;
            }
            if (domLineFormat.Style == LineStyle.Single)
            {
                lineFormat.set_Style(0);
            }
            lineFormat.set_Visible(domLineFormat.Visible);
            if (domLineFormat.IsNull("Visible"))
            {
                lineFormat.set_Visible(true);
            }
            lineFormat.set_Width(domLineFormat.Width.Point);
        }
    }
}

