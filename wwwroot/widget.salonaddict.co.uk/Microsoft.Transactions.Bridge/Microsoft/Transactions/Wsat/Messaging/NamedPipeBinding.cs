﻿namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel.Channels;

    internal class NamedPipeBinding : CoordinationBinding
    {
        public NamedPipeBinding(ProtocolVersion protocolVersion) : base("NamedPipe", "http://schemas.microsoft.com/ws/2006/02/transactions", protocolVersion)
        {
        }

        protected override void AddBindingElements(BindingElementCollection bindingElements)
        {
            bindingElements.Add(new BinaryMessageEncodingBindingElement());
            base.AddTransactionFlowBindingElement(bindingElements);
            base.AddWindowsStreamSecurityBindingElement(bindingElements);
            base.AddNamedPipeBindingElement(bindingElements);
        }

        public override string Scheme =>
            Uri.UriSchemeNetPipe;
    }
}

