namespace System.Drawing.Imaging
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    [TypeConverter(typeof(ImageFormatConverter))]
    public sealed class ImageFormat
    {
        private static ImageFormat bmp = new ImageFormat(new System.Guid("{b96b3cab-0728-11d3-9d7b-0000f81ef32e}"));
        private static ImageFormat emf = new ImageFormat(new System.Guid("{b96b3cac-0728-11d3-9d7b-0000f81ef32e}"));
        private static ImageFormat exif = new ImageFormat(new System.Guid("{b96b3cb2-0728-11d3-9d7b-0000f81ef32e}"));
        private static ImageFormat flashPIX = new ImageFormat(new System.Guid("{b96b3cb4-0728-11d3-9d7b-0000f81ef32e}"));
        private static ImageFormat gif = new ImageFormat(new System.Guid("{b96b3cb0-0728-11d3-9d7b-0000f81ef32e}"));
        private System.Guid guid;
        private static ImageFormat icon = new ImageFormat(new System.Guid("{b96b3cb5-0728-11d3-9d7b-0000f81ef32e}"));
        private static ImageFormat jpeg = new ImageFormat(new System.Guid("{b96b3cae-0728-11d3-9d7b-0000f81ef32e}"));
        private static ImageFormat memoryBMP = new ImageFormat(new System.Guid("{b96b3caa-0728-11d3-9d7b-0000f81ef32e}"));
        private static ImageFormat photoCD = new ImageFormat(new System.Guid("{b96b3cb3-0728-11d3-9d7b-0000f81ef32e}"));
        private static ImageFormat png = new ImageFormat(new System.Guid("{b96b3caf-0728-11d3-9d7b-0000f81ef32e}"));
        private static ImageFormat tiff = new ImageFormat(new System.Guid("{b96b3cb1-0728-11d3-9d7b-0000f81ef32e}"));
        private static ImageFormat wmf = new ImageFormat(new System.Guid("{b96b3cad-0728-11d3-9d7b-0000f81ef32e}"));

        public ImageFormat(System.Guid guid)
        {
            this.guid = guid;
        }

        public override bool Equals(object o)
        {
            ImageFormat format = o as ImageFormat;
            if (format == null)
            {
                return false;
            }
            return (this.guid == format.guid);
        }

        internal ImageCodecInfo FindEncoder()
        {
            foreach (ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
            {
                if (info.FormatID.Equals(this.guid))
                {
                    return info;
                }
            }
            return null;
        }

        public override int GetHashCode() => 
            this.guid.GetHashCode();

        public override string ToString()
        {
            if (this == memoryBMP)
            {
                return "MemoryBMP";
            }
            if (this == bmp)
            {
                return "Bmp";
            }
            if (this == emf)
            {
                return "Emf";
            }
            if (this == wmf)
            {
                return "Wmf";
            }
            if (this == gif)
            {
                return "Gif";
            }
            if (this == jpeg)
            {
                return "Jpeg";
            }
            if (this == png)
            {
                return "Png";
            }
            if (this == tiff)
            {
                return "Tiff";
            }
            if (this == exif)
            {
                return "Exif";
            }
            if (this == icon)
            {
                return "Icon";
            }
            return ("[ImageFormat: " + this.guid + "]");
        }

        public static ImageFormat Bmp =>
            bmp;

        public static ImageFormat Emf =>
            emf;

        public static ImageFormat Exif =>
            exif;

        public static ImageFormat Gif =>
            gif;

        public System.Guid Guid =>
            this.guid;

        public static ImageFormat Icon =>
            icon;

        public static ImageFormat Jpeg =>
            jpeg;

        public static ImageFormat MemoryBmp =>
            memoryBMP;

        public static ImageFormat Png =>
            png;

        public static ImageFormat Tiff =>
            tiff;

        public static ImageFormat Wmf =>
            wmf;
    }
}

