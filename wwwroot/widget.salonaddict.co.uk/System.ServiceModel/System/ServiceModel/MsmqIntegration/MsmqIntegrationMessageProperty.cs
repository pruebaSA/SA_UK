namespace System.ServiceModel.MsmqIntegration
{
    using System;
    using System.ComponentModel;
    using System.Messaging;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public sealed class MsmqIntegrationMessageProperty
    {
        private AcknowledgeTypes? acknowledgeType = null;
        private System.Messaging.Acknowledgment? acknowledgment = null;
        private Uri administrationQueue;
        private int? appSpecific = null;
        private DateTime? arrivedTime = null;
        private bool? authenticated = null;
        private object body;
        private int? bodyType = null;
        private string correlationId;
        private Uri destinationQueue;
        private byte[] extension;
        private string id;
        private string label;
        private System.Messaging.MessageType? messageType = null;
        public const string Name = "MsmqIntegrationMessageProperty";
        private MessagePriority? priority = null;
        private Uri responseQueue;
        private byte[] senderId;
        private DateTime? sentTime = null;
        private TimeSpan? timeToReachQueue = null;

        public static MsmqIntegrationMessageProperty Get(System.ServiceModel.Channels.Message message)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            if (message.Properties == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message.Properties");
            }
            return (message.Properties["MsmqIntegrationMessageProperty"] as MsmqIntegrationMessageProperty);
        }

        internal void InternalSetTimeToReachQueue(TimeSpan timeout)
        {
            this.timeToReachQueue = new TimeSpan?(timeout);
        }

        private static void ValidateMessagePriority(MessagePriority? priority)
        {
            if (priority.HasValue && ((((MessagePriority) priority.Value) < MessagePriority.Lowest) || (((MessagePriority) priority.Value) > MessagePriority.Highest)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("priority", priority.Value, typeof(MessagePriority)));
            }
        }

        private static void ValidateTimeToReachQueue(TimeSpan? timeout)
        {
            if (timeout.HasValue && (timeout.Value < TimeSpan.Zero))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", timeout, System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0")));
            }
            if (timeout.HasValue && TimeoutHelper.IsTooLarge(timeout.Value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", timeout, System.ServiceModel.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
            }
        }

        public AcknowledgeTypes? AcknowledgeType
        {
            get => 
                this.acknowledgeType;
            set
            {
                this.acknowledgeType = value;
            }
        }

        public System.Messaging.Acknowledgment? Acknowledgment
        {
            get => 
                this.acknowledgment;
            internal set
            {
                this.acknowledgment = value;
            }
        }

        public Uri AdministrationQueue
        {
            get => 
                this.administrationQueue;
            set
            {
                this.administrationQueue = value;
            }
        }

        public int? AppSpecific
        {
            get => 
                this.appSpecific;
            set
            {
                this.appSpecific = value;
            }
        }

        public DateTime? ArrivedTime
        {
            get => 
                this.arrivedTime;
            internal set
            {
                this.arrivedTime = value;
            }
        }

        public bool? Authenticated
        {
            get => 
                this.authenticated;
            internal set
            {
                this.authenticated = value;
            }
        }

        public object Body
        {
            get => 
                this.body;
            set
            {
                this.body = value;
            }
        }

        public int? BodyType
        {
            get => 
                this.bodyType;
            set
            {
                this.bodyType = value;
            }
        }

        public string CorrelationId
        {
            get => 
                this.correlationId;
            set
            {
                this.correlationId = value;
            }
        }

        public Uri DestinationQueue
        {
            get => 
                this.destinationQueue;
            internal set
            {
                this.destinationQueue = value;
            }
        }

        public byte[] Extension
        {
            get => 
                this.extension;
            set
            {
                this.extension = value;
            }
        }

        public string Id
        {
            get => 
                this.id;
            internal set
            {
                this.id = value;
            }
        }

        public string Label
        {
            get => 
                this.label;
            set
            {
                this.label = value;
            }
        }

        public System.Messaging.MessageType? MessageType
        {
            get => 
                this.messageType;
            internal set
            {
                this.messageType = value;
            }
        }

        public MessagePriority? Priority
        {
            get => 
                this.priority;
            set
            {
                ValidateMessagePriority(value);
                this.priority = value;
            }
        }

        public Uri ResponseQueue
        {
            get => 
                this.responseQueue;
            set
            {
                this.responseQueue = value;
            }
        }

        public byte[] SenderId
        {
            get => 
                this.senderId;
            internal set
            {
                this.senderId = value;
            }
        }

        public DateTime? SentTime
        {
            get => 
                this.sentTime;
            internal set
            {
                this.sentTime = value;
            }
        }

        public TimeSpan? TimeToReachQueue
        {
            get => 
                this.timeToReachQueue;
            set
            {
                ValidateTimeToReachQueue(value);
                this.timeToReachQueue = value;
            }
        }
    }
}

