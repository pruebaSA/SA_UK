namespace System.Web.Services.Protocols
{
    using System;
    using System.Runtime.InteropServices;

    public sealed class SoapClientMessage : SoapMessage
    {
        internal SoapExtension[] initializedExtensions;
        private SoapClientMethod method;
        private SoapHttpClientProtocol protocol;
        private string url;

        internal SoapClientMessage(SoapHttpClientProtocol protocol, SoapClientMethod method, string url)
        {
            this.method = method;
            this.protocol = protocol;
            this.url = url;
        }

        protected override void EnsureInStage()
        {
            base.EnsureStage(SoapMessageStage.BeforeSerialize);
        }

        protected override void EnsureOutStage()
        {
            base.EnsureStage(SoapMessageStage.AfterDeserialize);
        }

        public override string Action =>
            this.method.action;

        public SoapHttpClientProtocol Client =>
            this.protocol;

        internal SoapClientMethod Method =>
            this.method;

        public override LogicalMethodInfo MethodInfo =>
            this.method.methodInfo;

        public override bool OneWay =>
            this.method.oneWay;

        [ComVisible(false)]
        public override SoapProtocolVersion SoapVersion
        {
            get
            {
                if (this.protocol.SoapVersion != SoapProtocolVersion.Default)
                {
                    return this.protocol.SoapVersion;
                }
                return SoapProtocolVersion.Soap11;
            }
        }

        public override string Url =>
            this.url;
    }
}

