namespace MigraDoc.Rendering
{
    using PdfSharp.Drawing;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct VerticalLineInfo
    {
        internal XUnit height;
        internal XUnit descent;
        internal XUnit inherentlineSpace;
        internal VerticalLineInfo(XUnit height, XUnit descent, XUnit inherentlineSpace)
        {
            this.height = height;
            this.descent = descent;
            this.inherentlineSpace = inherentlineSpace;
        }
    }
}

