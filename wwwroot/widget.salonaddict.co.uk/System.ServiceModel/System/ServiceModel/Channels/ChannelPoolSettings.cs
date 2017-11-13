namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    public class ChannelPoolSettings
    {
        private TimeSpan idleTimeout;
        private TimeSpan leaseTimeout;
        private int maxOutboundChannelsPerEndpoint;

        public ChannelPoolSettings()
        {
            this.idleTimeout = OneWayDefaults.IdleTimeout;
            this.leaseTimeout = OneWayDefaults.LeaseTimeout;
            this.maxOutboundChannelsPerEndpoint = 10;
        }

        private ChannelPoolSettings(ChannelPoolSettings poolToBeCloned)
        {
            this.idleTimeout = poolToBeCloned.idleTimeout;
            this.leaseTimeout = poolToBeCloned.leaseTimeout;
            this.maxOutboundChannelsPerEndpoint = poolToBeCloned.maxOutboundChannelsPerEndpoint;
        }

        internal ChannelPoolSettings Clone() => 
            new ChannelPoolSettings(this);

        internal bool IsMatch(ChannelPoolSettings channelPoolSettings)
        {
            if (channelPoolSettings == null)
            {
                return false;
            }
            if (this.idleTimeout != channelPoolSettings.idleTimeout)
            {
                return false;
            }
            if (this.leaseTimeout != channelPoolSettings.leaseTimeout)
            {
                return false;
            }
            if (this.maxOutboundChannelsPerEndpoint != channelPoolSettings.maxOutboundChannelsPerEndpoint)
            {
                return false;
            }
            return true;
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

        public TimeSpan LeaseTimeout
        {
            get => 
                this.leaseTimeout;
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
                this.leaseTimeout = value;
            }
        }

        public int MaxOutboundChannelsPerEndpoint
        {
            get => 
                this.maxOutboundChannelsPerEndpoint;
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBePositive")));
                }
                this.maxOutboundChannelsPerEndpoint = value;
            }
        }
    }
}

