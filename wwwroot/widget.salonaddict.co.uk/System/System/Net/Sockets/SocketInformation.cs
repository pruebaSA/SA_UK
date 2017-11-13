namespace System.Net.Sockets
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SocketInformation
    {
        private byte[] protocolInformation;
        private SocketInformationOptions options;
        public byte[] ProtocolInformation
        {
            get => 
                this.protocolInformation;
            set
            {
                this.protocolInformation = value;
            }
        }
        public SocketInformationOptions Options
        {
            get => 
                this.options;
            set
            {
                this.options = value;
            }
        }
        internal bool IsNonBlocking
        {
            get => 
                ((this.options & SocketInformationOptions.NonBlocking) != 0);
            set
            {
                if (value)
                {
                    this.options |= SocketInformationOptions.NonBlocking;
                }
                else
                {
                    this.options &= ~SocketInformationOptions.NonBlocking;
                }
            }
        }
        internal bool IsConnected
        {
            get => 
                ((this.options & SocketInformationOptions.Connected) != 0);
            set
            {
                if (value)
                {
                    this.options |= SocketInformationOptions.Connected;
                }
                else
                {
                    this.options &= ~SocketInformationOptions.Connected;
                }
            }
        }
        internal bool IsListening
        {
            get => 
                ((this.options & SocketInformationOptions.Listening) != 0);
            set
            {
                if (value)
                {
                    this.options |= SocketInformationOptions.Listening;
                }
                else
                {
                    this.options &= ~SocketInformationOptions.Listening;
                }
            }
        }
        internal bool UseOnlyOverlappedIO
        {
            get => 
                ((this.options & SocketInformationOptions.UseOnlyOverlappedIO) != 0);
            set
            {
                if (value)
                {
                    this.options |= SocketInformationOptions.UseOnlyOverlappedIO;
                }
                else
                {
                    this.options &= ~SocketInformationOptions.UseOnlyOverlappedIO;
                }
            }
        }
    }
}

