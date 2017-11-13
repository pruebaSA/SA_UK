namespace PdfSharp.Drawing
{
    using System;
    using System.Drawing.Drawing2D;

    public sealed class XGraphicsContainer
    {
        internal GraphicsState GdiState;
        internal InternalGraphicsState InternalState;

        internal XGraphicsContainer(GraphicsState state)
        {
            this.GdiState = state;
        }
    }
}

