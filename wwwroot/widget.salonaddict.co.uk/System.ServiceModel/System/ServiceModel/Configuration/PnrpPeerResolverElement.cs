namespace System.ServiceModel.Configuration
{
    using System;
    using System.ServiceModel.Channels;

    public class PnrpPeerResolverElement : BindingElementExtensionElement
    {
        protected internal override BindingElement CreateBindingElement() => 
            new PnrpPeerResolverBindingElement();

        public override Type BindingElementType =>
            typeof(PnrpPeerResolverBindingElement);
    }
}

