namespace System.Web.Services.Protocols
{
    using System;
    using System.Web.Services.Description;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SoapDocumentServiceAttribute : Attribute
    {
        private SoapParameterStyle paramStyle;
        private SoapServiceRoutingStyle routingStyle;
        private SoapBindingUse use;

        public SoapDocumentServiceAttribute()
        {
        }

        public SoapDocumentServiceAttribute(SoapBindingUse use)
        {
            this.use = use;
        }

        public SoapDocumentServiceAttribute(SoapBindingUse use, SoapParameterStyle paramStyle)
        {
            this.use = use;
            this.paramStyle = paramStyle;
        }

        public SoapParameterStyle ParameterStyle
        {
            get => 
                this.paramStyle;
            set
            {
                this.paramStyle = value;
            }
        }

        public SoapServiceRoutingStyle RoutingStyle
        {
            get => 
                this.routingStyle;
            set
            {
                this.routingStyle = value;
            }
        }

        public SoapBindingUse Use
        {
            get => 
                this.use;
            set
            {
                this.use = value;
            }
        }
    }
}

