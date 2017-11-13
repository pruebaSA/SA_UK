namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public sealed class LocalServiceSecuritySettings
    {
        private bool detectReplays;
        private TimeSpan inactivityTimeout;
        private TimeSpan issuedCookieLifetime;
        private int maxCachedCookies;
        private TimeSpan maxClockSkew;
        private int maxPendingSessions;
        private int maxStatefulNegotiations;
        private TimeSpan negotiationTimeout;
        private bool reconnectTransportOnFailure;
        private int replayCacheSize;
        private TimeSpan replayWindow;
        private TimeSpan sessionKeyRenewalInterval;
        private TimeSpan sessionKeyRolloverInterval;
        private TimeSpan timestampValidityDuration;

        public LocalServiceSecuritySettings()
        {
            this.DetectReplays = true;
            this.ReplayCacheSize = 0xdbba0;
            this.ReplayWindow = SecurityProtocolFactory.defaultReplayWindow;
            this.MaxClockSkew = SecurityProtocolFactory.defaultMaxClockSkew;
            this.IssuedCookieLifetime = NegotiationTokenAuthenticator<NegotiationTokenAuthenticatorState>.defaultServerIssuedTokenLifetime;
            this.MaxStatefulNegotiations = 0x80;
            this.NegotiationTimeout = NegotiationTokenAuthenticator<NegotiationTokenAuthenticatorState>.defaultServerMaxNegotiationLifetime;
            this.maxPendingSessions = 0x80;
            this.inactivityTimeout = SecuritySessionServerSettings.defaultInactivityTimeout;
            this.sessionKeyRenewalInterval = SecuritySessionServerSettings.defaultKeyRenewalInterval;
            this.sessionKeyRolloverInterval = SecuritySessionServerSettings.defaultKeyRolloverInterval;
            this.reconnectTransportOnFailure = true;
            this.TimestampValidityDuration = SecurityProtocolFactory.defaultTimestampValidityDuration;
            this.maxCachedCookies = 0x3e8;
        }

        private LocalServiceSecuritySettings(LocalServiceSecuritySettings other)
        {
            this.detectReplays = other.detectReplays;
            this.replayCacheSize = other.replayCacheSize;
            this.replayWindow = other.replayWindow;
            this.maxClockSkew = other.maxClockSkew;
            this.issuedCookieLifetime = other.issuedCookieLifetime;
            this.maxStatefulNegotiations = other.maxStatefulNegotiations;
            this.negotiationTimeout = other.negotiationTimeout;
            this.maxPendingSessions = other.maxPendingSessions;
            this.inactivityTimeout = other.inactivityTimeout;
            this.sessionKeyRenewalInterval = other.sessionKeyRenewalInterval;
            this.sessionKeyRolloverInterval = other.sessionKeyRolloverInterval;
            this.reconnectTransportOnFailure = other.reconnectTransportOnFailure;
            this.timestampValidityDuration = other.timestampValidityDuration;
            this.maxCachedCookies = other.maxCachedCookies;
        }

        public LocalServiceSecuritySettings Clone() => 
            new LocalServiceSecuritySettings(this);

        public bool DetectReplays
        {
            get => 
                this.detectReplays;
            set
            {
                this.detectReplays = value;
            }
        }

        public TimeSpan InactivityTimeout
        {
            get => 
                this.inactivityTimeout;
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
                this.inactivityTimeout = value;
            }
        }

        public TimeSpan IssuedCookieLifetime
        {
            get => 
                this.issuedCookieLifetime;
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
                this.issuedCookieLifetime = value;
            }
        }

        public int MaxCachedCookies
        {
            get => 
                this.maxCachedCookies;
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBeNonNegative")));
                }
                this.maxCachedCookies = value;
            }
        }

        public TimeSpan MaxClockSkew
        {
            get => 
                this.maxClockSkew;
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
                this.maxClockSkew = value;
            }
        }

        public int MaxPendingSessions
        {
            get => 
                this.maxPendingSessions;
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBeNonNegative")));
                }
                this.maxPendingSessions = value;
            }
        }

        public int MaxStatefulNegotiations
        {
            get => 
                this.maxStatefulNegotiations;
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBeNonNegative")));
                }
                this.maxStatefulNegotiations = value;
            }
        }

        public TimeSpan NegotiationTimeout
        {
            get => 
                this.negotiationTimeout;
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
                this.negotiationTimeout = value;
            }
        }

        public bool ReconnectTransportOnFailure
        {
            get => 
                this.reconnectTransportOnFailure;
            set
            {
                this.reconnectTransportOnFailure = value;
            }
        }

        public int ReplayCacheSize
        {
            get => 
                this.replayCacheSize;
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBeNonNegative")));
                }
                this.replayCacheSize = value;
            }
        }

        public TimeSpan ReplayWindow
        {
            get => 
                this.replayWindow;
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
                this.replayWindow = value;
            }
        }

        public TimeSpan SessionKeyRenewalInterval
        {
            get => 
                this.sessionKeyRenewalInterval;
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
                this.sessionKeyRenewalInterval = value;
            }
        }

        public TimeSpan SessionKeyRolloverInterval
        {
            get => 
                this.sessionKeyRolloverInterval;
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
                this.sessionKeyRolloverInterval = value;
            }
        }

        public TimeSpan TimestampValidityDuration
        {
            get => 
                this.timestampValidityDuration;
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
                this.timestampValidityDuration = value;
            }
        }
    }
}

