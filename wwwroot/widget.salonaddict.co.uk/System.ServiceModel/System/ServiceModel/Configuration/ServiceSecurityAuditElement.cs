namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    public sealed class ServiceSecurityAuditElement : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            ServiceSecurityAuditElement element = (ServiceSecurityAuditElement) from;
            this.AuditLogLocation = element.AuditLogLocation;
            this.SuppressAuditFailure = element.SuppressAuditFailure;
            this.ServiceAuthorizationAuditLevel = element.ServiceAuthorizationAuditLevel;
            this.MessageAuthenticationAuditLevel = element.MessageAuthenticationAuditLevel;
        }

        protected internal override object CreateBehavior() => 
            new ServiceSecurityAuditBehavior { 
                AuditLogLocation = this.AuditLogLocation,
                SuppressAuditFailure = this.SuppressAuditFailure,
                ServiceAuthorizationAuditLevel = this.ServiceAuthorizationAuditLevel,
                MessageAuthenticationAuditLevel = this.MessageAuthenticationAuditLevel
            };

        [ServiceModelEnumValidator(typeof(AuditLogLocationHelper)), ConfigurationProperty("auditLogLocation", DefaultValue=0)]
        public System.ServiceModel.AuditLogLocation AuditLogLocation
        {
            get => 
                ((System.ServiceModel.AuditLogLocation) base["auditLogLocation"]);
            set
            {
                base["auditLogLocation"] = value;
            }
        }

        public override Type BehaviorType =>
            typeof(ServiceSecurityAuditBehavior);

        [ConfigurationProperty("messageAuthenticationAuditLevel", DefaultValue=0), ServiceModelEnumValidator(typeof(AuditLevelHelper))]
        public AuditLevel MessageAuthenticationAuditLevel
        {
            get => 
                ((AuditLevel) base["messageAuthenticationAuditLevel"]);
            set
            {
                base["messageAuthenticationAuditLevel"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("auditLogLocation", typeof(System.ServiceModel.AuditLogLocation), System.ServiceModel.AuditLogLocation.Default, null, new ServiceModelEnumValidator(typeof(AuditLogLocationHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("suppressAuditFailure", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("serviceAuthorizationAuditLevel", typeof(AuditLevel), AuditLevel.None, null, new ServiceModelEnumValidator(typeof(AuditLevelHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("messageAuthenticationAuditLevel", typeof(AuditLevel), AuditLevel.None, null, new ServiceModelEnumValidator(typeof(AuditLevelHelper)), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ServiceModelEnumValidator(typeof(AuditLevelHelper)), ConfigurationProperty("serviceAuthorizationAuditLevel", DefaultValue=0)]
        public AuditLevel ServiceAuthorizationAuditLevel
        {
            get => 
                ((AuditLevel) base["serviceAuthorizationAuditLevel"]);
            set
            {
                base["serviceAuthorizationAuditLevel"] = value;
            }
        }

        [ConfigurationProperty("suppressAuditFailure", DefaultValue=true)]
        public bool SuppressAuditFailure
        {
            get => 
                ((bool) base["suppressAuditFailure"]);
            set
            {
                base["suppressAuditFailure"] = value;
            }
        }
    }
}

