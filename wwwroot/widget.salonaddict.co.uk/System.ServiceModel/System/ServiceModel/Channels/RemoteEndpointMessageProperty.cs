namespace System.ServiceModel.Channels
{
    using System;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    public sealed class RemoteEndpointMessageProperty
    {
        private string address;
        private HostedRequestContainer hostedRequestContainer;
        private int port;
        private IPEndPoint remoteEndPoint;
        private InitializationState state;
        private object thisLock;

        internal RemoteEndpointMessageProperty(IPEndPoint remoteEndPoint)
        {
            this.thisLock = new object();
            this.remoteEndPoint = remoteEndPoint;
        }

        internal RemoteEndpointMessageProperty(HostedRequestContainer hostedRequestContainer)
        {
            this.thisLock = new object();
            this.hostedRequestContainer = hostedRequestContainer;
        }

        public RemoteEndpointMessageProperty(string address, int port)
        {
            this.thisLock = new object();
            if (string.IsNullOrEmpty(address))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
            }
            if ((port < 0) || (port > 0xffff))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("port", System.ServiceModel.SR.GetString("ValueMustBeInRange", new object[] { 0, 0xffff }));
            }
            this.port = port;
            this.address = address;
            this.state = InitializationState.All;
        }

        private void Initialize(bool getHostedPort)
        {
            if (this.remoteEndPoint != null)
            {
                this.address = this.remoteEndPoint.Address.ToString();
                this.port = this.remoteEndPoint.Port;
                this.state = InitializationState.All;
                this.remoteEndPoint = null;
            }
            else
            {
                if ((this.state & InitializationState.Address) != InitializationState.Address)
                {
                    this.address = this.hostedRequestContainer.GetRemoteAddress();
                    this.state |= InitializationState.Address;
                }
                if (getHostedPort)
                {
                    this.port = this.hostedRequestContainer.GetRemotePort();
                    this.state |= InitializationState.Port;
                    this.hostedRequestContainer = null;
                }
            }
        }

        public string Address
        {
            get
            {
                if ((this.state & InitializationState.Address) != InitializationState.Address)
                {
                    lock (this.ThisLock)
                    {
                        if ((this.state & InitializationState.Address) != InitializationState.Address)
                        {
                            this.Initialize(false);
                        }
                    }
                }
                return this.address;
            }
        }

        public static string Name =>
            "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        public int Port
        {
            get
            {
                if ((this.state & InitializationState.Port) != InitializationState.Port)
                {
                    lock (this.ThisLock)
                    {
                        if ((this.state & InitializationState.Port) != InitializationState.Port)
                        {
                            this.Initialize(true);
                        }
                    }
                }
                return this.port;
            }
        }

        private object ThisLock =>
            this.thisLock;

        [Flags]
        private enum InitializationState
        {
            None,
            Address,
            Port,
            All
        }
    }
}

