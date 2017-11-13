namespace System.ServiceModel.MsmqIntegration
{
    using System;
    using System.ServiceModel;

    [MessageContract(IsWrapped=false)]
    public sealed class MsmqMessage<T>
    {
        [MessageProperty(Name="MsmqIntegrationMessageProperty")]
        private MsmqIntegrationMessageProperty property;

        internal MsmqMessage()
        {
        }

        public MsmqMessage(T body)
        {
            if (body == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("body");
            }
            this.property = new MsmqIntegrationMessageProperty();
            this.property.Body = body;
        }

        public AcknowledgeTypes? AcknowledgeType
        {
            get => 
                this.property.AcknowledgeType;
            set
            {
                this.property.AcknowledgeType = value;
            }
        }

        public System.Messaging.Acknowledgment? Acknowledgment =>
            this.property.Acknowledgment;

        public Uri AdministrationQueue
        {
            get => 
                this.property.AdministrationQueue;
            set
            {
                this.property.AdministrationQueue = value;
            }
        }

        public int? AppSpecific
        {
            get => 
                this.property.AppSpecific;
            set
            {
                this.property.AppSpecific = value;
            }
        }

        public DateTime? ArrivedTime =>
            this.property.ArrivedTime;

        public bool? Authenticated =>
            this.property.Authenticated;

        public T Body
        {
            get => 
                ((T) this.property.Body);
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                this.property.Body = value;
            }
        }

        public int? BodyType
        {
            get => 
                this.property.BodyType;
            set
            {
                this.property.BodyType = value;
            }
        }

        public string CorrelationId
        {
            get => 
                this.property.CorrelationId;
            set
            {
                this.property.CorrelationId = value;
            }
        }

        public Uri DestinationQueue =>
            this.property.DestinationQueue;

        public byte[] Extension
        {
            get => 
                this.property.Extension;
            set
            {
                this.property.Extension = value;
            }
        }

        public string Id =>
            this.property.Id;

        public string Label
        {
            get => 
                this.property.Label;
            set
            {
                this.property.Label = value;
            }
        }

        public System.Messaging.MessageType? MessageType =>
            this.property.MessageType;

        public MessagePriority? Priority
        {
            get => 
                this.property.Priority;
            set
            {
                this.property.Priority = value;
            }
        }

        public Uri ResponseQueue
        {
            get => 
                this.property.ResponseQueue;
            set
            {
                this.property.ResponseQueue = value;
            }
        }

        public byte[] SenderId =>
            this.property.SenderId;

        public DateTime? SentTime =>
            this.property.SentTime;

        public TimeSpan? TimeToReachQueue
        {
            get => 
                this.property.TimeToReachQueue;
            set
            {
                this.property.TimeToReachQueue = value;
            }
        }
    }
}

