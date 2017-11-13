namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel.Channels;

    internal class SupportingTokenBindingElement : BindingElement
    {
        private Microsoft.Transactions.Wsat.Protocol.ProtocolVersion protocolVersion;
        private SupportingTokenServiceCredentials serverCreds;

        private SupportingTokenBindingElement(SupportingTokenBindingElement other) : base(other)
        {
            this.serverCreds = new SupportingTokenServiceCredentials();
            this.protocolVersion = other.ProtocolVersion;
        }

        public SupportingTokenBindingElement(Microsoft.Transactions.Wsat.Protocol.ProtocolVersion protocolVersion)
        {
            this.serverCreds = new SupportingTokenServiceCredentials();
            this.protocolVersion = protocolVersion;
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            new SupportingTokenChannelListener<TChannel>(this, context, this.serverCreds.TokenResolver);

        public override bool CanBuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            context.CanBuildInnerChannelListener<TChannel>();

        public override BindingElement Clone() => 
            new SupportingTokenBindingElement(this);

        public override T GetProperty<T>(BindingContext context) where T: class => 
            context.GetInnerProperty<T>();

        public Microsoft.Transactions.Wsat.Protocol.ProtocolVersion ProtocolVersion =>
            this.protocolVersion;

        public SupportingTokenServiceCredentials ServiceCredentials =>
            this.serverCreds;
    }
}

