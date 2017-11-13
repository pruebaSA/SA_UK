namespace System.Net.NetworkInformation
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MibTcpRow
    {
        internal TcpState state;
        internal uint localAddr;
        internal byte localPort1;
        internal byte localPort2;
        internal byte localPort3;
        internal byte localPort4;
        internal uint remoteAddr;
        internal byte remotePort1;
        internal byte remotePort2;
        internal byte remotePort3;
        internal byte remotePort4;
    }
}

