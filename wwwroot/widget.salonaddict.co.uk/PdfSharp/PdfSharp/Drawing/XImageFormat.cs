namespace PdfSharp.Drawing
{
    using System;

    public sealed class XImageFormat
    {
        private static XImageFormat gif = new XImageFormat(new System.Guid("{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}"));
        private System.Guid guid;
        private static XImageFormat icon = new XImageFormat(new System.Guid("{B96B3CB5-0728-11D3-9D7B-0000F81EF32E}"));
        private static XImageFormat jpeg = new XImageFormat(new System.Guid("{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}"));
        private static XImageFormat pdf = new XImageFormat(new System.Guid("{84570158-DBF0-4C6B-8368-62D6A3CA76E0}"));
        private static XImageFormat png = new XImageFormat(new System.Guid("{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}"));
        private static XImageFormat tiff = new XImageFormat(new System.Guid("{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}"));

        private XImageFormat(System.Guid guid)
        {
            this.guid = guid;
        }

        public override bool Equals(object obj)
        {
            XImageFormat format = obj as XImageFormat;
            if (format == null)
            {
                return false;
            }
            return (this.guid == format.guid);
        }

        public override int GetHashCode() => 
            this.guid.GetHashCode();

        public static XImageFormat Gif =>
            gif;

        internal System.Guid Guid =>
            this.guid;

        public static XImageFormat Icon =>
            icon;

        public static XImageFormat Jpeg =>
            jpeg;

        public static XImageFormat Pdf =>
            pdf;

        public static XImageFormat Png =>
            png;

        public static XImageFormat Tiff =>
            tiff;
    }
}

