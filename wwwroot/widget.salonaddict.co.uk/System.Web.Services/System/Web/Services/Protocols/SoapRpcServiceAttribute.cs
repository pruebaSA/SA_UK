namespace System.Web.Services.Protocols
{
    using System;
    using System.Runtime.InteropServices;
    using System.Web.Services.Description;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SoapRpcServiceAttribute : Attribute
    {
        private SoapServiceRoutingStyle routingStyle;
        private SoapBindingUse use = SoapBindingUse.Encoded;

        public SoapServiceRoutingStyle RoutingStyle
        {
            get => 
                this.routingStyle;
            set
            {
                this.routingStyle = value;
            }
        }

        [ComVisible(false)]
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

