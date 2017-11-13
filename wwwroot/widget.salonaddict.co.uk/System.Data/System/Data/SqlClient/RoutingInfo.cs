namespace System.Data.SqlClient
{
    using System;

    internal class RoutingInfo
    {
        private ushort _port;
        private byte _protocol;
        private string _serverName;

        internal RoutingInfo(byte protocol, ushort port, string servername)
        {
            this._protocol = protocol;
            this._port = port;
            this._serverName = servername;
        }

        internal ushort Port =>
            this._port;

        internal byte Protocol =>
            this._protocol;

        internal string ServerName =>
            this._serverName;
    }
}

