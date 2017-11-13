namespace System.DirectoryServices.Protocols
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    internal class Luid
    {
        internal int lowPart;
        internal int highPart;
        internal Luid()
        {
        }

        public int LowPart =>
            this.lowPart;
        public int HighPart =>
            this.highPart;
    }
}

