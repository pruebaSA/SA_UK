﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Runtime.CompilerServices;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using System.ServiceModel.Security.Tokens;

    internal abstract class MsmqChannelFactoryBase<TChannel> : TransportChannelFactory<TChannel>
    {
        private MsmqUri.IAddressTranslator addressTranslator;
        private Uri customDeadLetterQueue;
        private System.ServiceModel.DeadLetterQueue deadLetterQueue;
        private string deadLetterQueuePathName;
        private bool durable;
        private bool exactlyOnce;
        private System.ServiceModel.MsmqTransportSecurity msmqTransportSecurity;
        private System.IdentityModel.Selectors.SecurityTokenManager securityTokenManager;
        private TimeSpan timeToLive;
        private bool useMsmqTracing;
        private bool useSourceJournal;

        protected MsmqChannelFactoryBase(MsmqBindingElementBase bindingElement, BindingContext context) : this(bindingElement, context, TransportDefaults.GetDefaultMessageEncoderFactory())
        {
        }

        protected MsmqChannelFactoryBase(MsmqBindingElementBase bindingElement, BindingContext context, MessageEncoderFactory encoderFactory) : base(bindingElement, context)
        {
            this.exactlyOnce = true;
            this.addressTranslator = bindingElement.AddressTranslator;
            this.customDeadLetterQueue = bindingElement.CustomDeadLetterQueue;
            this.durable = bindingElement.Durable;
            this.deadLetterQueue = bindingElement.DeadLetterQueue;
            this.exactlyOnce = bindingElement.ExactlyOnce;
            this.msmqTransportSecurity = new System.ServiceModel.MsmqTransportSecurity(bindingElement.MsmqTransportSecurity);
            this.timeToLive = bindingElement.TimeToLive;
            this.useMsmqTracing = bindingElement.UseMsmqTracing;
            this.useSourceJournal = bindingElement.UseSourceJournal;
            if (this.MsmqTransportSecurity.MsmqAuthenticationMode == MsmqAuthenticationMode.Certificate)
            {
                this.InitializeSecurityTokenManager(context);
            }
            if (null != this.customDeadLetterQueue)
            {
                this.deadLetterQueuePathName = MsmqUri.DeadLetterQueueAddressTranslator.UriToFormatName(this.customDeadLetterQueue);
            }
        }

        internal SecurityTokenProvider CreateTokenProvider(EndpointAddress to, Uri via)
        {
            InitiatorServiceModelSecurityTokenRequirement tokenRequirement = new InitiatorServiceModelSecurityTokenRequirement {
                TokenType = SecurityTokenTypes.X509Certificate,
                TargetAddress = to,
                Via = via,
                KeyUsage = SecurityKeyUsage.Signature,
                TransportScheme = this.Scheme
            };
            return this.SecurityTokenManager.CreateSecurityTokenProvider(tokenRequirement);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal SecurityTokenProviderContainer CreateX509TokenProvider(EndpointAddress to, Uri via)
        {
            if ((MsmqAuthenticationMode.Certificate == this.MsmqTransportSecurity.MsmqAuthenticationMode) && (this.SecurityTokenManager != null))
            {
                return new SecurityTokenProviderContainer(this.CreateTokenProvider(to, via));
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitializeSecurityTokenManager(BindingContext context)
        {
            SecurityCredentialsManager manager = context.BindingParameters.Find<SecurityCredentialsManager>();
            if (manager != null)
            {
                this.securityTokenManager = manager.CreateSecurityTokenManager();
            }
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => 
            new CompletedAsyncResult(callback, state);

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        internal MsmqUri.IAddressTranslator AddressTranslator =>
            this.addressTranslator;

        public Uri CustomDeadLetterQueue =>
            this.customDeadLetterQueue;

        public System.ServiceModel.DeadLetterQueue DeadLetterQueue =>
            this.deadLetterQueue;

        internal string DeadLetterQueuePathName =>
            this.deadLetterQueuePathName;

        public bool Durable =>
            this.durable;

        public bool ExactlyOnce =>
            this.exactlyOnce;

        internal bool IsMsmqX509SecurityConfigured =>
            (MsmqAuthenticationMode.Certificate == this.MsmqTransportSecurity.MsmqAuthenticationMode);

        public System.ServiceModel.MsmqTransportSecurity MsmqTransportSecurity =>
            this.msmqTransportSecurity;

        public override string Scheme =>
            this.addressTranslator.Scheme;

        public System.IdentityModel.Selectors.SecurityTokenManager SecurityTokenManager =>
            this.securityTokenManager;

        public TimeSpan TimeToLive =>
            this.timeToLive;

        public bool UseMsmqTracing =>
            this.useMsmqTracing;

        public bool UseSourceJournal =>
            this.useSourceJournal;
    }
}

