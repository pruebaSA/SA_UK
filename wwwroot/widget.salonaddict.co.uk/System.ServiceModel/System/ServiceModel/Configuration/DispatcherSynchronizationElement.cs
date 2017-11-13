namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Description;

    public sealed class DispatcherSynchronizationElement : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            DispatcherSynchronizationElement element = (DispatcherSynchronizationElement) from;
            this.MaxPendingReceives = element.MaxPendingReceives;
        }

        protected internal override object CreateBehavior() => 
            new DispatcherSynchronizationBehavior(this.MaxPendingReceives);

        public override Type BehaviorType =>
            typeof(DispatcherSynchronizationBehavior);

        [ConfigurationProperty("maxPendingReceives", DefaultValue=1), IntegerValidator(MinValue=1)]
        public int MaxPendingReceives
        {
            get => 
                ((int) base["maxPendingReceives"]);
            set
            {
                base["maxPendingReceives"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("maxPendingReceives", typeof(int), 1, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

