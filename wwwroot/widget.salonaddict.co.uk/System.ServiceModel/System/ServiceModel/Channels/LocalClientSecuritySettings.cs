namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public sealed class LocalClientSecuritySettings
    {
        private bool cacheCookies;
        private int cookieRenewalThresholdPercentage;
        private bool detectReplays;
        private System.ServiceModel.Security.IdentityVerifier identityVerifier;
        private TimeSpan maxClockSkew;
        private TimeSpan maxCookieCachingTime;
        private bool reconnectTransportOnFailure;
        private int replayCacheSize;
        private TimeSpan replayWindow;
        private TimeSpan sessionKeyRenewalInterval;
        private TimeSpan sessionKeyRolloverInterval;
        private TimeSpan timestampValidityDuration;

        public LocalClientSecuritySettings()
        {
            this.DetectReplays = true;
            this.ReplayCacheSize = 0xdbba0;
            this.ReplayWindow = SecurityProtocolFactory.defaultReplayWindow;
            this.MaxClockSkew = SecurityProtocolFactory.defaultMaxClockSkew;
            this.TimestampValidityDuration = SecurityProtocolFactory.defaultTimestampValidityDuration;
            this.CacheCookies = true;
            this.MaxCookieCachingTime = IssuanceTokenProviderBase<IssuanceTokenProviderState>.DefaultClientMaxTokenCachingTime;
            this.SessionKeyRenewalInterval = SecuritySessionClientSettings.defaultKeyRenewalInterval;
            this.SessionKeyRolloverInterval = SecuritySessionClientSettings.defaultKeyRolloverInterval;
            this.ReconnectTransportOnFailure = true;
            this.CookieRenewalThresholdPercentage = 60;
            this.IdentityVerifier = System.ServiceModel.Security.IdentityVerifier.CreateDefault();
        }

        private LocalClientSecuritySettings(LocalClientSecuritySettings other)
        {
            this.detectReplays = other.detectReplays;
            this.replayCacheSize = other.replayCacheSize;
            this.replayWindow = other.replayWindow;
            this.maxClockSkew = other.maxClockSkew;
            this.cacheCookies = other.cacheCookies;
            this.maxCookieCachingTime = other.maxCookieCachingTime;
            this.sessionKeyRenewalInterval = other.sessionKeyRenewalInterval;
            this.sessionKeyRolloverInterval = other.sessionKeyRolloverInterval;
            this.reconnectTransportOnFailure = other.reconnectTransportOnFailure;
            this.timestampValidityDuration = other.timestampValidityDuration;
            this.identityVerifier = other.identityVerifier;
            this.cookieRenewalThresholdPercentage = other.cookieRenewalThresholdPercentage;
        }

        public LocalClientSecuritySettings Clone() => 
            new LocalClientSecuritySettings(this);

        public bool CacheCookies
        {
            get => 
                this.cacheCookies;
            set
            {
                this.cacheCookies = value;
            }
        }

        public int CookieRenewalThresholdPercentage
        {
            get => 
                this.cookieRenewalThresholdPercentage;
            set
            {
                if ((value < 0) || (value > 100))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBeInRange", new object[] { 0, 100 })));
                }
                this.cookieRenewalThresholdPercentage = value;
            }
        }

        public bool DetectReplays
        {
            get => 
                this.detectReplays;
            set
            {
                this.detectReplays = value;
            }
        }

        public System.ServiceModel.Security.IdentityVerifier IdentityVerifier
        {
            get => 
                this.identityVerifier;
            set
            {
                this.identityVerifier = value;
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

        public TimeSpan MaxCookieCachingTime
        {
            get => 
                this.maxCookieCachingTime;
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
                this.maxCookieCachingTime = value;
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

