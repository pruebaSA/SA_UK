namespace System.ServiceModel.Channels
{
    using System;

    internal class ConnectionMessageProperty
    {
        private IConnection connection;

        public ConnectionMessageProperty(IConnection connection)
        {
            this.connection = connection;
        }

        public IConnection Connection =>
            this.connection;

        public static string Name =>
            "iconnection";
    }
}

