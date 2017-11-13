﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Dispatcher;

    public sealed class DataContractSerializerElement : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            DataContractSerializerElement element = (DataContractSerializerElement) from;
            this.IgnoreExtensionDataObject = element.IgnoreExtensionDataObject;
            this.MaxItemsInObjectGraph = element.MaxItemsInObjectGraph;
        }

        protected internal override object CreateBehavior() => 
            new DataContractSerializerServiceBehavior(this.IgnoreExtensionDataObject, this.MaxItemsInObjectGraph);

        public override Type BehaviorType =>
            typeof(DataContractSerializerServiceBehavior);

        [ConfigurationProperty("ignoreExtensionDataObject", DefaultValue=false)]
        public bool IgnoreExtensionDataObject
        {
            get => 
                ((bool) base["ignoreExtensionDataObject"]);
            set
            {
                base["ignoreExtensionDataObject"] = value;
            }
        }

        [ConfigurationProperty("maxItemsInObjectGraph", DefaultValue=0x10000), IntegerValidator(MinValue=0)]
        public int MaxItemsInObjectGraph
        {
            get => 
                ((int) base["maxItemsInObjectGraph"]);
            set
            {
                base["maxItemsInObjectGraph"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("ignoreExtensionDataObject", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxItemsInObjectGraph", typeof(int), 0x10000, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

