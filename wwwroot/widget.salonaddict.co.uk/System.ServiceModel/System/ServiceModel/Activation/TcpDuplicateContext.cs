namespace System.ServiceModel.Activation
{
    using System;
    using System.Net.Sockets;
    using System.Runtime.Serialization;

    [DataContract]
    internal class TcpDuplicateContext : DuplicateContext
    {
        [DataMember]
        private System.Net.Sockets.SocketInformation socketInformation;

        public TcpDuplicateContext(System.Net.Sockets.SocketInformation socketInformation, Uri via, byte[] readData) : base(via, readData)
        {
            this.socketInformation = socketInformation;
        }

        public System.Net.Sockets.SocketInformation SocketInformation =>
            this.socketInformation;
    }
}

