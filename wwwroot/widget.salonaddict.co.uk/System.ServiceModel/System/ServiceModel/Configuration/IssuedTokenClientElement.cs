﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;

    public sealed class IssuedTokenClientElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(IssuedTokenClientCredential issuedToken)
        {
            if (issuedToken == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("issuedToken");
            }
            issuedToken.CacheIssuedTokens = this.CacheIssuedTokens;
            issuedToken.DefaultKeyEntropyMode = this.DefaultKeyEntropyMode;
            issuedToken.MaxIssuedTokenCachingTime = this.MaxIssuedTokenCachingTime;
            issuedToken.IssuedTokenRenewalThresholdPercentage = this.IssuedTokenRenewalThresholdPercentage;
            if (base.ElementInformation.Properties["localIssuer"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.LocalIssuer.Validate();
                issuedToken.LocalIssuerAddress = ConfigLoader.LoadEndpointAddress(this.LocalIssuer);
                if (!string.IsNullOrEmpty(this.LocalIssuer.Binding))
                {
                    issuedToken.LocalIssuerBinding = ConfigLoader.LookupBinding(this.LocalIssuer.Binding, this.LocalIssuer.BindingConfiguration, base.EvaluationContext);
                }
            }
            if (!string.IsNullOrEmpty(this.LocalIssuerChannelBehaviors))
            {
                ConfigLoader.LoadChannelBehaviors(this.LocalIssuerChannelBehaviors, base.EvaluationContext, issuedToken.LocalIssuerChannelBehaviors);
            }
            if (base.ElementInformation.Properties["issuerChannelBehaviors"].ValueOrigin != PropertyValueOrigin.Default)
            {
                foreach (IssuedTokenClientBehaviorsElement element in this.IssuerChannelBehaviors)
                {
                    if (!string.IsNullOrEmpty(element.BehaviorConfiguration))
                    {
                        KeyedByTypeCollection<IEndpointBehavior> channelBehaviors = new KeyedByTypeCollection<IEndpointBehavior>();
                        ConfigLoader.LoadChannelBehaviors(element.BehaviorConfiguration, base.EvaluationContext, channelBehaviors);
                        issuedToken.IssuerChannelBehaviors.Add(new Uri(element.IssuerAddress), channelBehaviors);
                    }
                }
            }
        }

        public void Copy(IssuedTokenClientElement from)
        {
            if (this.IsReadOnly())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigReadOnly")));
            }
            if (from == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("from");
            }
            this.DefaultKeyEntropyMode = from.DefaultKeyEntropyMode;
            this.CacheIssuedTokens = from.CacheIssuedTokens;
            this.MaxIssuedTokenCachingTime = from.MaxIssuedTokenCachingTime;
            this.IssuedTokenRenewalThresholdPercentage = from.IssuedTokenRenewalThresholdPercentage;
            if (from.ElementInformation.Properties["localIssuer"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.LocalIssuer.Copy(from.LocalIssuer);
            }
            if (from.ElementInformation.Properties["localIssuerChannelBehaviors"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.LocalIssuerChannelBehaviors = from.LocalIssuerChannelBehaviors;
            }
            if (from.ElementInformation.Properties["issuerChannelBehaviors"].ValueOrigin != PropertyValueOrigin.Default)
            {
                foreach (IssuedTokenClientBehaviorsElement element in from.IssuerChannelBehaviors)
                {
                    this.IssuerChannelBehaviors.Add(element);
                }
            }
        }

        [ConfigurationProperty("cacheIssuedTokens", DefaultValue=true)]
        public bool CacheIssuedTokens
        {
            get => 
                ((bool) base["cacheIssuedTokens"]);
            set
            {
                base["cacheIssuedTokens"] = value;
            }
        }

        [ConfigurationProperty("defaultKeyEntropyMode", DefaultValue=2), ServiceModelEnumValidator(typeof(SecurityKeyEntropyModeHelper))]
        public SecurityKeyEntropyMode DefaultKeyEntropyMode
        {
            get => 
                ((SecurityKeyEntropyMode) base["defaultKeyEntropyMode"]);
            set
            {
                base["defaultKeyEntropyMode"] = value;
            }
        }

        [ConfigurationProperty("issuedTokenRenewalThresholdPercentage", DefaultValue=60), IntegerValidator(MinValue=0, MaxValue=100)]
        public int IssuedTokenRenewalThresholdPercentage
        {
            get => 
                ((int) base["issuedTokenRenewalThresholdPercentage"]);
            set
            {
                base["issuedTokenRenewalThresholdPercentage"] = value;
            }
        }

        [ConfigurationProperty("issuerChannelBehaviors")]
        public IssuedTokenClientBehaviorsElementCollection IssuerChannelBehaviors =>
            ((IssuedTokenClientBehaviorsElementCollection) base["issuerChannelBehaviors"]);

        [ConfigurationProperty("localIssuer")]
        public IssuedTokenParametersEndpointAddressElement LocalIssuer =>
            ((IssuedTokenParametersEndpointAddressElement) base["localIssuer"]);

        [ConfigurationProperty("localIssuerChannelBehaviors", DefaultValue=""), StringValidator(MinLength=0)]
        public string LocalIssuerChannelBehaviors
        {
            get => 
                ((string) base["localIssuerChannelBehaviors"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["localIssuerChannelBehaviors"] = value;
            }
        }

        [ConfigurationProperty("maxIssuedTokenCachingTime", DefaultValue="10675199.02:48:05.4775807"), TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan MaxIssuedTokenCachingTime
        {
            get => 
                ((TimeSpan) base["maxIssuedTokenCachingTime"]);
            set
            {
                base["maxIssuedTokenCachingTime"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("localIssuer", typeof(IssuedTokenParametersEndpointAddressElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("localIssuerChannelBehaviors", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("issuerChannelBehaviors", typeof(IssuedTokenClientBehaviorsElementCollection), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("cacheIssuedTokens", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxIssuedTokenCachingTime", typeof(TimeSpan), TimeSpan.Parse("10675199.02:48:05.4775807"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("defaultKeyEntropyMode", typeof(SecurityKeyEntropyMode), SecurityKeyEntropyMode.CombinedEntropy, null, new ServiceModelEnumValidator(typeof(SecurityKeyEntropyModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("issuedTokenRenewalThresholdPercentage", typeof(int), 60, null, new IntegerValidator(0, 100, false), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

