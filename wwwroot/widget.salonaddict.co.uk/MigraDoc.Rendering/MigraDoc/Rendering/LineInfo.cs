namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using PdfSharp.Drawing;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct LineInfo
    {
        internal ParagraphIterator startIter;
        internal ParagraphIterator endIter;
        internal XUnit wordsWidth;
        internal XUnit lineWidth;
        internal int blankCount;
        internal VerticalLineInfo vertical;
        internal ArrayList tabOffsets;
        internal bool reMeasureLine;
        internal DocumentObject lastTab;
    }
}

