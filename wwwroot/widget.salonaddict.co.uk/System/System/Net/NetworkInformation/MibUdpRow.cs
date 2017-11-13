namespace System.Net.NetworkInformation
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MibUdpRow
    {
        internal uint localAddr;
        internal byte localPort1;
        internal byte localPort2;
        internal byte localPort3;
        internal byte localPort4;
    }
}

