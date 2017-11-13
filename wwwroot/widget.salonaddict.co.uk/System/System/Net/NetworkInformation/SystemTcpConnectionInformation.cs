namespace System.Net.NetworkInformation
{
    using System;
    using System.Net;

    internal class SystemTcpConnectionInformation : TcpConnectionInformation
    {
        private IPEndPoint localEndPoint;
        private IPEndPoint remoteEndPoint;
        private TcpState state;

        internal SystemTcpConnectionInformation(MibTcpRow row)
        {
            this.state = row.state;
            int port = (((row.localPort3 << 0x18) | (row.localPort4 << 0x10)) | (row.localPort1 << 8)) | row.localPort2;
            int num2 = (this.state == TcpState.Listen) ? 0 : ((((row.remotePort3 << 0x18) | (row.remotePort4 << 0x10)) | (row.remotePort1 << 8)) | row.remotePort2);
            this.localEndPoint = new IPEndPoint((long) row.localAddr, port);
            this.remoteEndPoint = new IPEndPoint((long) row.remoteAddr, num2);
        }

        public override IPEndPoint LocalEndPoint =>
            this.localEndPoint;

        public override IPEndPoint RemoteEndPoint =>
            this.remoteEndPoint;

        public override TcpState State =>
            this.state;
    }
}

