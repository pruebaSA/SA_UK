namespace PdfSharp.Drawing
{
    using System;
    using System.Drawing.Drawing2D;

    internal static class XConvert
    {
        private static LineCap[] gdiLineCap;
        private static LineJoin[] gdiLineJoin;

        static XConvert()
        {
            LineJoin[] joinArray = new LineJoin[3];
            joinArray[1] = LineJoin.Round;
            joinArray[2] = LineJoin.Bevel;
            gdiLineJoin = joinArray;
            LineCap[] capArray = new LineCap[3];
            capArray[1] = LineCap.Round;
            capArray[2] = LineCap.Square;
            gdiLineCap = capArray;
        }

        public static LineCap ToLineCap(XLineCap lineCap) => 
            gdiLineCap[(int) lineCap];

        public static LineJoin ToLineJoin(XLineJoin lineJoin) => 
            gdiLineJoin[(int) lineJoin];
    }
}

