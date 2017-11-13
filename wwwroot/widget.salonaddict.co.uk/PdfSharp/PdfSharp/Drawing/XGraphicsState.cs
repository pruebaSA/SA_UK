namespace PdfSharp.Drawing
{
    using System;
    using System.Drawing.Drawing2D;

    public sealed class XGraphicsState
    {
        internal GraphicsState GdiState;
        internal InternalGraphicsState InternalState;

        internal XGraphicsState(GraphicsState state)
        {
            this.GdiState = state;
        }
    }
}

