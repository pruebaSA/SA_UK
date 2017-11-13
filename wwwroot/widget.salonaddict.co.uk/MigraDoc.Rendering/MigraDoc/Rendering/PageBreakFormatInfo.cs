namespace MigraDoc.Rendering
{
    using System;

    internal class PageBreakFormatInfo : FormatInfo
    {
        internal override bool EndingIsComplete =>
            true;

        internal override bool IsComplete =>
            true;

        internal override bool IsEmpty =>
            false;

        internal override bool IsEnding =>
            true;

        internal override bool IsStarting =>
            true;

        internal override bool StartingIsComplete =>
            true;
    }
}

