namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    public sealed class CallbackTimeoutsElement : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            CallbackTimeoutsElement element = (CallbackTimeoutsElement) from;
            this.TransactionTimeout = element.TransactionTimeout;
        }

        protected internal override object CreateBehavior() => 
            new CallbackTimeoutsBehavior { TransactionTimeout = this.TransactionTimeout };

        public override Type BehaviorType =>
            typeof(CallbackTimeoutsBehavior);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("transactionTimeout", typeof(TimeSpan), TimeSpan.Parse("00:00:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00"), ConfigurationProperty("transactionTimeout", DefaultValue="00:00:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan TransactionTimeout
        {
            get => 
                ((TimeSpan) base["transactionTimeout"]);
            set
            {
                base["transactionTimeout"] = value;
            }
        }
    }
}

