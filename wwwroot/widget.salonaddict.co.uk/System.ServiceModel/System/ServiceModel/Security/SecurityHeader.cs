namespace System.ServiceModel.Security
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    internal abstract class SecurityHeader : MessageHeader
    {
        private readonly string actor;
        private readonly SecurityAlgorithmSuite algorithmSuite;
        private bool encryptedKeyContainsReferenceList = true;
        private SecurityHeaderLayout layout;
        private bool maintainSignatureConfirmationState;
        private System.ServiceModel.Channels.Message message;
        private readonly bool mustUnderstand;
        private bool processingStarted;
        private readonly bool relay;
        private bool requireMessageProtection = true;
        private readonly SecurityStandardsManager standardsManager;
        private System.ServiceModel.Description.MessageDirection transferDirection;

        public SecurityHeader(System.ServiceModel.Channels.Message message, string actor, bool mustUnderstand, bool relay, SecurityStandardsManager standardsManager, SecurityAlgorithmSuite algorithmSuite, System.ServiceModel.Description.MessageDirection transferDirection)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            if (actor == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("actor");
            }
            if (standardsManager == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("standardsManager");
            }
            if (algorithmSuite == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithmSuite");
            }
            this.message = message;
            this.actor = actor;
            this.mustUnderstand = mustUnderstand;
            this.relay = relay;
            this.standardsManager = standardsManager;
            this.algorithmSuite = algorithmSuite;
            this.transferDirection = transferDirection;
        }

        protected void SetProcessingStarted()
        {
            this.processingStarted = true;
        }

        protected void ThrowIfProcessingStarted()
        {
            if (this.processingStarted)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("OperationCannotBeDoneAfterProcessingIsStarted")));
            }
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "{0}(Actor = '{1}')", new object[] { base.GetType().Name, this.Actor });

        public override string Actor =>
            this.actor;

        public SecurityAlgorithmSuite AlgorithmSuite =>
            this.algorithmSuite;

        public bool EncryptedKeyContainsReferenceList
        {
            get => 
                this.encryptedKeyContainsReferenceList;
            set
            {
                this.ThrowIfProcessingStarted();
                this.encryptedKeyContainsReferenceList = value;
            }
        }

        public SecurityHeaderLayout Layout
        {
            get => 
                this.layout;
            set
            {
                this.ThrowIfProcessingStarted();
                this.layout = value;
            }
        }

        public bool MaintainSignatureConfirmationState
        {
            get => 
                this.maintainSignatureConfirmationState;
            set
            {
                this.ThrowIfProcessingStarted();
                this.maintainSignatureConfirmationState = value;
            }
        }

        protected System.ServiceModel.Channels.Message Message
        {
            get => 
                this.message;
            set
            {
                this.message = value;
            }
        }

        public System.ServiceModel.Description.MessageDirection MessageDirection =>
            this.transferDirection;

        public override bool MustUnderstand =>
            this.mustUnderstand;

        public override bool Relay =>
            this.relay;

        public bool RequireMessageProtection
        {
            get => 
                this.requireMessageProtection;
            set
            {
                this.ThrowIfProcessingStarted();
                this.requireMessageProtection = value;
            }
        }

        public SecurityStandardsManager StandardsManager =>
            this.standardsManager;

        protected MessageVersion Version =>
            this.message.Version;
    }
}

