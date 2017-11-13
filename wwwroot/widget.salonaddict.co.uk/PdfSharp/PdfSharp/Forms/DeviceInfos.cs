namespace PdfSharp.Forms
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceInfos
    {
        private const int HORZSIZE = 4;
        private const int VERTSIZE = 6;
        private const int HORZRES = 8;
        private const int VERTRES = 10;
        private const int LOGPIXELSX = 0x58;
        private const int LOGPIXELSY = 90;
        public int HorizontalSize;
        public int VerticalSize;
        public int HorizontalResolution;
        public int VerticalResolution;
        public int LogicalDpiX;
        public int LogicalDpiY;
        public float PhysicalDpiX;
        public float PhysicalDpiY;
        public float ScaleX;
        public float ScaleY;
        [SuppressUnmanagedCodeSecurity]
        public static DeviceInfos GetInfos(IntPtr hdc)
        {
            DeviceInfos infos;
            infos.HorizontalSize = GetDeviceCaps(hdc, 4);
            infos.VerticalSize = GetDeviceCaps(hdc, 6);
            infos.HorizontalResolution = GetDeviceCaps(hdc, 8);
            infos.VerticalResolution = GetDeviceCaps(hdc, 10);
            infos.LogicalDpiX = GetDeviceCaps(hdc, 0x58);
            infos.LogicalDpiY = GetDeviceCaps(hdc, 90);
            infos.PhysicalDpiX = (infos.HorizontalResolution * 25.4f) / ((float) infos.HorizontalSize);
            infos.PhysicalDpiY = (infos.VerticalResolution * 25.4f) / ((float) infos.VerticalSize);
            infos.ScaleX = ((float) infos.LogicalDpiX) / infos.PhysicalDpiX;
            infos.ScaleY = ((float) infos.LogicalDpiY) / infos.PhysicalDpiY;
            return infos;
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int capability);
    }
}

