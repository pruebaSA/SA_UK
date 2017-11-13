namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel.Channels;

    internal class InteropActivationBinding : InteropRequestReplyBinding
    {
        public InteropActivationBinding(Uri clientBaseAddress, ProtocolVersion protocolVersion) : base(clientBaseAddress, protocolVersion)
        {
        }

        protected override void AddBindingElements(BindingElementCollection bindingElements)
        {
            base.AddTransactionFlowBindingElement(bindingElements);
            base.AddBindingElements(bindingElements);
        }
    }
}

