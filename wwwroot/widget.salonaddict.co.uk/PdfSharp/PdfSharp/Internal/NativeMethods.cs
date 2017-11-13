namespace PdfSharp.Internal
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        public const int HORZRES = 8;
        public const int HORZSIZE = 4;
        public const int LOGPIXELSX = 0x58;
        public const int LOGPIXELSY = 90;
        public const int VERTRES = 10;
        public const int VERTSIZE = 6;

        [DllImport("gdi32.dll", EntryPoint="CreateFontIndirectW")]
        public static extern IntPtr CreateFontIndirect(LOGFONT lpLogFont);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hgdiobj);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        [DllImport("gdi32.dll", SetLastError=true)]
        public static extern int GetFontData(IntPtr hdc, uint dwTable, uint dwOffset, byte[] lpvBuffer, int cbData);
        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
        public class LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x20)]
            public string lfFaceName;
            private LOGFONT(int dummy)
            {
                this.lfHeight = 0;
                this.lfWidth = 0;
                this.lfEscapement = 0;
                this.lfOrientation = 0;
                this.lfWeight = 0;
                this.lfItalic = 0;
                this.lfUnderline = 0;
                this.lfStrikeOut = 0;
                this.lfCharSet = 0;
                this.lfOutPrecision = 0;
                this.lfClipPrecision = 0;
                this.lfQuality = 0;
                this.lfPitchAndFamily = 0;
                this.lfFaceName = "";
            }

            public override string ToString() => 
                string.Concat(new object[] { 
                    "lfHeight=", this.lfHeight, ", lfWidth=", this.lfWidth, ", lfEscapement=", this.lfEscapement, ", lfOrientation=", this.lfOrientation, ", lfWeight=", this.lfWeight, ", lfItalic=", this.lfItalic, ", lfUnderline=", this.lfUnderline, ", lfStrikeOut=", this.lfStrikeOut,
                    ", lfCharSet=", this.lfCharSet, ", lfOutPrecision=", this.lfOutPrecision, ", lfClipPrecision=", this.lfClipPrecision, ", lfQuality=", this.lfQuality, ", lfPitchAndFamily=", this.lfPitchAndFamily, ", lfFaceName=", this.lfFaceName
                });

            public LOGFONT()
            {
            }
        }
    }
}

