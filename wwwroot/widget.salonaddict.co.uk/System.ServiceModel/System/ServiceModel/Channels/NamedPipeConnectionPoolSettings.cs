namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    public sealed class NamedPipeConnectionPoolSettings
    {
        private string groupName;
        private TimeSpan idleTimeout;
        private int maxOutputConnectionsPerEndpoint;

        internal NamedPipeConnectionPoolSettings()
        {
            this.groupName = "default";
            this.idleTimeout = ConnectionOrientedTransportDefaults.IdleTimeout;
            this.maxOutputConnectionsPerEndpoint = 10;
        }

        internal NamedPipeConnectionPoolSettings(NamedPipeConnectionPoolSettings namedPipe)
        {
            this.groupName = namedPipe.groupName;
            this.idleTimeout = namedPipe.idleTimeout;
            this.maxOutputConnectionsPerEndpoint = namedPipe.maxOutputConnectionsPerEndpoint;
        }

        internal NamedPipeConnectionPoolSettings Clone() => 
            new NamedPipeConnectionPoolSettings(this);

        internal bool IsMatch(NamedPipeConnectionPoolSettings namedPipe)
        {
            if (this.groupName != namedPipe.groupName)
            {
                return false;
            }
            if (this.idleTimeout != namedPipe.idleTimeout)
            {
                return false;
            }
            if (this.maxOutputConnectionsPerEndpoint != namedPipe.maxOutputConnectionsPerEndpoint)
            {
                return false;
            }
            return true;
        }

        public string GroupName
        {
            get => 
                this.groupName;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                this.groupName = value;
            }
        }

        public TimeSpan IdleTimeout
        {
            get => 
                this.idleTimeout;
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0")));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
                }
                this.idleTimeout = value;
            }
        }

        public int MaxOutboundConnectionsPerEndpoint
        {
            get => 
                this.maxOutputConnectionsPerEndpoint;
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBeNonNegative")));
                }
                this.maxOutputConnectionsPerEndpoint = value;
            }
        }
    }
}

