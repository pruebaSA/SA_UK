namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using PdfSharp.Drawing;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct TabOffset
    {
        internal TabLeader leader;
        internal XUnit offset;
        internal TabOffset(TabLeader leader, XUnit offset)
        {
            this.leader = leader;
            this.offset = offset;
        }
    }
}

