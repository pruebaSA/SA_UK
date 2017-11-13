namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Description;

    public sealed class ServiceThrottlingElement : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            ServiceThrottlingElement element = (ServiceThrottlingElement) from;
            this.MaxConcurrentCalls = element.MaxConcurrentCalls;
            this.MaxConcurrentSessions = element.MaxConcurrentSessions;
            this.MaxConcurrentInstances = element.MaxConcurrentInstances;
        }

        protected internal override object CreateBehavior()
        {
            ServiceThrottlingBehavior behavior = new ServiceThrottlingBehavior {
                MaxConcurrentCalls = this.MaxConcurrentCalls,
                MaxConcurrentSessions = this.MaxConcurrentSessions
            };
            if (base.ElementInformation.Properties["maxConcurrentInstances"].ValueOrigin != PropertyValueOrigin.Default)
            {
                behavior.MaxConcurrentInstances = this.MaxConcurrentInstances;
            }
            return behavior;
        }

        public override Type BehaviorType =>
            typeof(ServiceThrottlingBehavior);

        [IntegerValidator(MinValue=1), ConfigurationProperty("maxConcurrentCalls", DefaultValue=0x10)]
        public int MaxConcurrentCalls
        {
            get => 
                ((int) base["maxConcurrentCalls"]);
            set
            {
                base["maxConcurrentCalls"] = value;
            }
        }

        [IntegerValidator(MinValue=1), ConfigurationProperty("maxConcurrentInstances", DefaultValue=0x1a)]
        public int MaxConcurrentInstances
        {
            get => 
                ((int) base["maxConcurrentInstances"]);
            set
            {
                base["maxConcurrentInstances"] = value;
            }
        }

        [ConfigurationProperty("maxConcurrentSessions", DefaultValue=10), IntegerValidator(MinValue=1)]
        public int MaxConcurrentSessions
        {
            get => 
                ((int) base["maxConcurrentSessions"]);
            set
            {
                base["maxConcurrentSessions"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("maxConcurrentCalls", typeof(int), 0x10, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxConcurrentSessions", typeof(int), 10, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxConcurrentInstances", typeof(int), 0x1a, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

