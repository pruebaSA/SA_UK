namespace System.Messaging
{
    using System;
    using System.ComponentModel;
    using System.Messaging.Design;

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DefaultPropertiesToSend
    {
        private MessageQueue cachedAdminQueue;
        private Message cachedMessage;
        private MessageQueue cachedResponseQueue;
        private MessageQueue cachedTransactionStatusQueue;
        private bool designMode;

        public DefaultPropertiesToSend()
        {
            this.cachedMessage = new Message();
        }

        internal DefaultPropertiesToSend(bool designMode)
        {
            this.cachedMessage = new Message();
            this.designMode = designMode;
        }

        private bool ShouldSerializeExtension() => 
            ((this.Extension != null) && (this.Extension.Length > 0));

        private bool ShouldSerializeTimeToBeReceived()
        {
            if (this.TimeToBeReceived == Message.InfiniteTimeout)
            {
                return false;
            }
            return true;
        }

        private bool ShouldSerializeTimeToReachQueue()
        {
            if (this.TimeToReachQueue == Message.InfiniteTimeout)
            {
                return false;
            }
            return true;
        }

        [DefaultValue(0), MessagingDescription("MsgAcknowledgeType")]
        public AcknowledgeTypes AcknowledgeType
        {
            get => 
                this.cachedMessage.AcknowledgeType;
            set
            {
                this.cachedMessage.AcknowledgeType = value;
            }
        }

        [DefaultValue((string) null), MessagingDescription("MsgAdministrationQueue")]
        public MessageQueue AdministrationQueue
        {
            get
            {
                if (!this.designMode)
                {
                    return this.cachedMessage.AdministrationQueue;
                }
                if ((this.cachedAdminQueue != null) && (this.cachedAdminQueue.Site == null))
                {
                    this.cachedAdminQueue = null;
                }
                return this.cachedAdminQueue;
            }
            set
            {
                if (this.designMode)
                {
                    this.cachedAdminQueue = value;
                }
                else
                {
                    this.cachedMessage.AdministrationQueue = value;
                }
            }
        }

        [MessagingDescription("MsgAppSpecific"), DefaultValue(0)]
        public int AppSpecific
        {
            get => 
                this.cachedMessage.AppSpecific;
            set
            {
                this.cachedMessage.AppSpecific = value;
            }
        }

        [MessagingDescription("MsgAttachSenderId"), DefaultValue(true)]
        public bool AttachSenderId
        {
            get => 
                this.cachedMessage.AttachSenderId;
            set
            {
                this.cachedMessage.AttachSenderId = value;
            }
        }

        internal Message CachedMessage =>
            this.cachedMessage;

        [DefaultValue(0x6602), MessagingDescription("MsgEncryptionAlgorithm")]
        public System.Messaging.EncryptionAlgorithm EncryptionAlgorithm
        {
            get => 
                this.cachedMessage.EncryptionAlgorithm;
            set
            {
                this.cachedMessage.EncryptionAlgorithm = value;
            }
        }

        [Editor("System.ComponentModel.Design.ArrayEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), MessagingDescription("MsgExtension")]
        public byte[] Extension
        {
            get => 
                this.cachedMessage.Extension;
            set
            {
                this.cachedMessage.Extension = value;
            }
        }

        [DefaultValue(0x8003), MessagingDescription("MsgHashAlgorithm")]
        public System.Messaging.HashAlgorithm HashAlgorithm
        {
            get => 
                this.cachedMessage.HashAlgorithm;
            set
            {
                this.cachedMessage.HashAlgorithm = value;
            }
        }

        [DefaultValue(""), MessagingDescription("MsgLabel")]
        public string Label
        {
            get => 
                this.cachedMessage.Label;
            set
            {
                this.cachedMessage.Label = value;
            }
        }

        [MessagingDescription("MsgPriority"), DefaultValue(3)]
        public MessagePriority Priority
        {
            get => 
                this.cachedMessage.Priority;
            set
            {
                this.cachedMessage.Priority = value;
            }
        }

        [DefaultValue(false), MessagingDescription("MsgRecoverable")]
        public bool Recoverable
        {
            get => 
                this.cachedMessage.Recoverable;
            set
            {
                this.cachedMessage.Recoverable = value;
            }
        }

        [DefaultValue((string) null), MessagingDescription("MsgResponseQueue")]
        public MessageQueue ResponseQueue
        {
            get
            {
                if (this.designMode)
                {
                    return this.cachedResponseQueue;
                }
                return this.cachedMessage.ResponseQueue;
            }
            set
            {
                if (this.designMode)
                {
                    this.cachedResponseQueue = value;
                }
                else
                {
                    this.cachedMessage.ResponseQueue = value;
                }
            }
        }

        [MessagingDescription("MsgTimeToBeReceived"), TypeConverter(typeof(TimeoutConverter))]
        public TimeSpan TimeToBeReceived
        {
            get => 
                this.cachedMessage.TimeToBeReceived;
            set
            {
                this.cachedMessage.TimeToBeReceived = value;
            }
        }

        [TypeConverter(typeof(TimeoutConverter)), MessagingDescription("MsgTimeToReachQueue")]
        public TimeSpan TimeToReachQueue
        {
            get => 
                this.cachedMessage.TimeToReachQueue;
            set
            {
                this.cachedMessage.TimeToReachQueue = value;
            }
        }

        [DefaultValue((string) null), MessagingDescription("MsgTransactionStatusQueue")]
        public MessageQueue TransactionStatusQueue
        {
            get
            {
                if (this.designMode)
                {
                    return this.cachedTransactionStatusQueue;
                }
                return this.cachedMessage.TransactionStatusQueue;
            }
            set
            {
                if (this.designMode)
                {
                    this.cachedTransactionStatusQueue = value;
                }
                else
                {
                    this.cachedMessage.TransactionStatusQueue = value;
                }
            }
        }

        [DefaultValue(false), MessagingDescription("MsgUseAuthentication")]
        public bool UseAuthentication
        {
            get => 
                this.cachedMessage.UseAuthentication;
            set
            {
                this.cachedMessage.UseAuthentication = value;
            }
        }

        [MessagingDescription("MsgUseDeadLetterQueue"), DefaultValue(false)]
        public bool UseDeadLetterQueue
        {
            get => 
                this.cachedMessage.UseDeadLetterQueue;
            set
            {
                this.cachedMessage.UseDeadLetterQueue = value;
            }
        }

        [MessagingDescription("MsgUseEncryption"), DefaultValue(false)]
        public bool UseEncryption
        {
            get => 
                this.cachedMessage.UseEncryption;
            set
            {
                this.cachedMessage.UseEncryption = value;
            }
        }

        [MessagingDescription("MsgUseJournalQueue"), DefaultValue(false)]
        public bool UseJournalQueue
        {
            get => 
                this.cachedMessage.UseJournalQueue;
            set
            {
                this.cachedMessage.UseJournalQueue = value;
            }
        }

        [DefaultValue(false), MessagingDescription("MsgUseTracing")]
        public bool UseTracing
        {
            get => 
                this.cachedMessage.UseTracing;
            set
            {
                this.cachedMessage.UseTracing = value;
            }
        }
    }
}

