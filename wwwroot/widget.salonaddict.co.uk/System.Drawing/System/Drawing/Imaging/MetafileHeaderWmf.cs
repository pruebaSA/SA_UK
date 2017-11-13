namespace System.Drawing.Imaging
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack=8)]
    internal class MetafileHeaderWmf
    {
        public MetafileType type;
        public int size = Marshal.SizeOf(typeof(MetafileHeaderWmf));
        public int version;
        public EmfPlusFlags emfPlusFlags;
        public float dpiX;
        public float dpiY;
        public int X;
        public int Y;
        public int Width;
        public int Height;
        [MarshalAs(UnmanagedType.Struct)]
        public MetaHeader WmfHeader = new MetaHeader();
        public int dummy1;
        public int dummy2;
        public int dummy3;
        public int dummy4;
        public int EmfPlusHeaderSize;
        public int LogicalDpiX;
        public int LogicalDpiY;
    }
}

