namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class TransactionFlowElement : BindingElementExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            TransactionFlowBindingElement element = (TransactionFlowBindingElement) bindingElement;
            element.Transactions = true;
            element.TransactionProtocol = this.TransactionProtocol;
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            TransactionFlowElement element = (TransactionFlowElement) from;
            this.TransactionProtocol = element.TransactionProtocol;
        }

        protected internal override BindingElement CreateBindingElement() => 
            new TransactionFlowBindingElement(true, this.TransactionProtocol);

        protected internal override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);
            TransactionFlowBindingElement element = (TransactionFlowBindingElement) bindingElement;
            this.TransactionProtocol = element.TransactionProtocol;
        }

        public override Type BindingElementType =>
            typeof(TransactionFlowBindingElement);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("transactionProtocol", typeof(System.ServiceModel.TransactionProtocol), "OleTransactions", new TransactionProtocolConverter(), null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("transactionProtocol", DefaultValue="OleTransactions"), TypeConverter(typeof(TransactionProtocolConverter))]
        public System.ServiceModel.TransactionProtocol TransactionProtocol
        {
            get => 
                ((System.ServiceModel.TransactionProtocol) base["transactionProtocol"]);
            set
            {
                base["transactionProtocol"] = value;
            }
        }
    }
}

