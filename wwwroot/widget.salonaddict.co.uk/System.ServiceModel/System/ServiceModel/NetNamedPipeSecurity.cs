namespace System.ServiceModel
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;

    public sealed class NetNamedPipeSecurity
    {
        internal const NetNamedPipeSecurityMode DefaultMode = NetNamedPipeSecurityMode.Transport;
        private NetNamedPipeSecurityMode mode;
        private NamedPipeTransportSecurity transport;

        internal NetNamedPipeSecurity()
        {
            this.transport = new NamedPipeTransportSecurity();
            this.mode = NetNamedPipeSecurityMode.Transport;
        }

        private NetNamedPipeSecurity(NetNamedPipeSecurityMode mode, NamedPipeTransportSecurity transport)
        {
            this.transport = new NamedPipeTransportSecurity();
            this.mode = mode;
            this.transport = (transport == null) ? new NamedPipeTransportSecurity() : transport;
        }

        internal WindowsStreamSecurityBindingElement CreateTransportSecurity()
        {
            if (this.mode == NetNamedPipeSecurityMode.Transport)
            {
                return this.transport.CreateTransportProtectionAndAuthentication();
            }
            return null;
        }

        internal static bool TryCreate(WindowsStreamSecurityBindingElement wssbe, NetNamedPipeSecurityMode mode, out NetNamedPipeSecurity security)
        {
            security = null;
            NamedPipeTransportSecurity transportSecurity = new NamedPipeTransportSecurity();
            if ((mode == NetNamedPipeSecurityMode.Transport) && !NamedPipeTransportSecurity.IsTransportProtectionAndAuthentication(wssbe, transportSecurity))
            {
                return false;
            }
            security = new NetNamedPipeSecurity(mode, transportSecurity);
            return true;
        }

        public NetNamedPipeSecurityMode Mode
        {
            get => 
                this.mode;
            set
            {
                if (!NetNamedPipeSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.mode = value;
            }
        }

        public NamedPipeTransportSecurity Transport =>
            this.transport;
    }
}

