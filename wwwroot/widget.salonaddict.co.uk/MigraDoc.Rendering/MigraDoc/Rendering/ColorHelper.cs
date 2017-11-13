namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using PdfSharp.Drawing;
    using System;

    internal static class ColorHelper
    {
        public static XColor ToXColor(Color color, bool cmyk)
        {
            if (color.IsEmpty)
            {
                return XColor.Empty;
            }
            if (cmyk)
            {
                return XColor.FromCmyk(color.Alpha / 100.0, color.C / 100.0, color.M / 100.0, color.Y / 100.0, color.K / 100.0);
            }
            return XColor.FromArgb((int) color.Argb);
        }
    }
}

