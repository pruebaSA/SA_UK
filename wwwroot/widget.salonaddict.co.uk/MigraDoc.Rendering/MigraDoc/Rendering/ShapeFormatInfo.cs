namespace MigraDoc.Rendering
{
    using System;

    internal class ShapeFormatInfo : FormatInfo
    {
        internal bool fits;

        internal ShapeFormatInfo()
        {
        }

        internal override bool EndingIsComplete =>
            this.fits;

        internal override bool IsComplete =>
            this.fits;

        internal override bool IsEmpty =>
            !this.fits;

        internal override bool IsEnding =>
            this.fits;

        internal override bool IsStarting =>
            this.fits;

        internal override bool StartingIsComplete =>
            this.fits;
    }
}

