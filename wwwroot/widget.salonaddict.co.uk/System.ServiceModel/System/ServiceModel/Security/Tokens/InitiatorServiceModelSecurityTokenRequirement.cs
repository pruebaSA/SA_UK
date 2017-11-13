namespace System.ServiceModel.Security.Tokens
{
    using System;
    using System.Net;
    using System.ServiceModel;

    public sealed class InitiatorServiceModelSecurityTokenRequirement : ServiceModelSecurityTokenRequirement
    {
        private WebHeaderCollection webHeaderCollection;

        public InitiatorServiceModelSecurityTokenRequirement()
        {
            base.Properties.Add(ServiceModelSecurityTokenRequirement.IsInitiatorProperty, true);
        }

        public override string ToString() => 
            base.InternalToString();

        internal bool IsOutOfBandToken
        {
            get => 
                base.GetPropertyOrDefault<bool>(ServiceModelSecurityTokenRequirement.IsOutOfBandTokenProperty, false);
            set
            {
                base.Properties[ServiceModelSecurityTokenRequirement.IsOutOfBandTokenProperty] = value;
            }
        }

        public EndpointAddress TargetAddress
        {
            get => 
                base.GetPropertyOrDefault<EndpointAddress>(ServiceModelSecurityTokenRequirement.TargetAddressProperty, null);
            set
            {
                base.Properties[ServiceModelSecurityTokenRequirement.TargetAddressProperty] = value;
            }
        }

        public Uri Via
        {
            get => 
                base.GetPropertyOrDefault<Uri>(ServiceModelSecurityTokenRequirement.ViaProperty, null);
            set
            {
                base.Properties[ServiceModelSecurityTokenRequirement.ViaProperty] = value;
            }
        }

        internal WebHeaderCollection WebHeaders
        {
            get => 
                this.webHeaderCollection;
            set
            {
                this.webHeaderCollection = value;
            }
        }
    }
}

