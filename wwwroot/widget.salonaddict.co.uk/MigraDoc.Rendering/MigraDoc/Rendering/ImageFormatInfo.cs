namespace MigraDoc.Rendering
{
    using PdfSharp.Drawing;
    using System;

    internal class ImageFormatInfo : ShapeFormatInfo
    {
        internal int CropHeight;
        internal int CropWidth;
        internal int CropX;
        internal int CropY;
        internal ImageFailure failure;
        internal XUnit Height;
        internal string ImagePath;
        internal XUnit Width;

        internal ImageFormatInfo()
        {
        }
    }
}

