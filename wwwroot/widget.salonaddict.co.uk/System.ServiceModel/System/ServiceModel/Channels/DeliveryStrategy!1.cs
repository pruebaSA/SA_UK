namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    internal abstract class DeliveryStrategy<ItemType> : IDisposable where ItemType: class, IDisposable
    {
        private InputQueueChannel<ItemType> channel;
        private ItemDequeuedCallback dequeueCallback;
        private int quota;

        public DeliveryStrategy(InputQueueChannel<ItemType> channel, int quota)
        {
            if (quota <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            this.channel = channel;
            this.quota = quota;
        }

        public abstract bool CanEnqueue(long sequenceNumber);
        public virtual void Dispose()
        {
        }

        public abstract bool Enqueue(ItemType item, long sequenceNumber);

        protected InputQueueChannel<ItemType> Channel =>
            this.channel;

        public ItemDequeuedCallback DequeueCallback
        {
            get => 
                this.dequeueCallback;
            set
            {
                this.dequeueCallback = value;
            }
        }

        public abstract int EnqueuedCount { get; }

        protected int Quota =>
            this.quota;
    }
}

