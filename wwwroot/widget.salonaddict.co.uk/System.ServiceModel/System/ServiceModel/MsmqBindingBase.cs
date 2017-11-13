namespace System.ServiceModel
{
    using System;
    using System.ServiceModel.Channels;

    public abstract class MsmqBindingBase : Binding, IBindingRuntimePreferences
    {
        internal MsmqBindingElementBase transport;

        protected MsmqBindingBase()
        {
        }

        public Uri CustomDeadLetterQueue
        {
            get => 
                this.transport.CustomDeadLetterQueue;
            set
            {
                this.transport.CustomDeadLetterQueue = value;
            }
        }

        public System.ServiceModel.DeadLetterQueue DeadLetterQueue
        {
            get => 
                this.transport.DeadLetterQueue;
            set
            {
                this.transport.DeadLetterQueue = value;
            }
        }

        public bool Durable
        {
            get => 
                this.transport.Durable;
            set
            {
                this.transport.Durable = value;
            }
        }

        public bool ExactlyOnce
        {
            get => 
                this.transport.ExactlyOnce;
            set
            {
                this.transport.ExactlyOnce = value;
            }
        }

        public long MaxReceivedMessageSize
        {
            get => 
                this.transport.MaxReceivedMessageSize;
            set
            {
                this.transport.MaxReceivedMessageSize = value;
            }
        }

        public int MaxRetryCycles
        {
            get => 
                this.transport.MaxRetryCycles;
            set
            {
                this.transport.MaxRetryCycles = value;
            }
        }

        public System.ServiceModel.ReceiveErrorHandling ReceiveErrorHandling
        {
            get => 
                this.transport.ReceiveErrorHandling;
            set
            {
                this.transport.ReceiveErrorHandling = value;
            }
        }

        public int ReceiveRetryCount
        {
            get => 
                this.transport.ReceiveRetryCount;
            set
            {
                this.transport.ReceiveRetryCount = value;
            }
        }

        public TimeSpan RetryCycleDelay
        {
            get => 
                this.transport.RetryCycleDelay;
            set
            {
                this.transport.RetryCycleDelay = value;
            }
        }

        public override string Scheme =>
            this.transport.Scheme;

        bool IBindingRuntimePreferences.ReceiveSynchronously =>
            this.ExactlyOnce;

        public TimeSpan TimeToLive
        {
            get => 
                this.transport.TimeToLive;
            set
            {
                this.transport.TimeToLive = value;
            }
        }

        public bool UseMsmqTracing
        {
            get => 
                this.transport.UseMsmqTracing;
            set
            {
                this.transport.UseMsmqTracing = value;
            }
        }

        public bool UseSourceJournal
        {
            get => 
                this.transport.UseSourceJournal;
            set
            {
                this.transport.UseSourceJournal = value;
            }
        }
    }
}

