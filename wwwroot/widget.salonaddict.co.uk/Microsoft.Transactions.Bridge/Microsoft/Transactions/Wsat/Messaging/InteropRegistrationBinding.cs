namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel.Channels;

    internal class InteropRegistrationBinding : InteropRequestReplyBinding
    {
        private Microsoft.Transactions.Wsat.Messaging.SupportingTokenBindingElement supportingTokenBE;

        public InteropRegistrationBinding(Uri clientBaseAddress, bool acceptSupportingTokens, ProtocolVersion protocolVersion) : base(clientBaseAddress, protocolVersion)
        {
            if (acceptSupportingTokens)
            {
                this.supportingTokenBE = new Microsoft.Transactions.Wsat.Messaging.SupportingTokenBindingElement(protocolVersion);
            }
        }

        protected override void AddBindingElements(BindingElementCollection bindingElements)
        {
            if (this.supportingTokenBE != null)
            {
                base.AddTransportSecurityBindingElement(bindingElements);
                bindingElements.Add(this.supportingTokenBE);
            }
            base.AddBindingElements(bindingElements);
        }

        public Microsoft.Transactions.Wsat.Messaging.SupportingTokenBindingElement SupportingTokenBindingElement =>
            this.supportingTokenBE;
    }
}

