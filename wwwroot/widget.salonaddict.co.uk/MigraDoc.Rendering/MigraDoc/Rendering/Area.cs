namespace MigraDoc.Rendering
{
    using PdfSharp.Drawing;
    using System;

    internal abstract class Area
    {
        internal Area()
        {
        }

        internal abstract Rectangle GetFittingRect(XUnit yPosition, XUnit height);
        internal abstract Area Lower(XUnit verticalOffset);
        internal abstract Area Unite(Area area);

        internal abstract XUnit Height { get; set; }

        internal abstract XUnit Width { get; set; }

        internal abstract XUnit X { get; set; }

        internal abstract XUnit Y { get; set; }
    }
}

