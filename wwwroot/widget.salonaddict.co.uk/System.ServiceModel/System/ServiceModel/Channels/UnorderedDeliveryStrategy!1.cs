namespace System.ServiceModel.Channels
{
    using System;

    internal class UnorderedDeliveryStrategy<ItemType> : DeliveryStrategy<ItemType> where ItemType: class, IDisposable
    {
        public UnorderedDeliveryStrategy(InputQueueChannel<ItemType> channel, int quota) : base(channel, quota)
        {
        }

        public override bool CanEnqueue(long sequenceNumber) => 
            (this.EnqueuedCount < base.Quota);

        public override bool Enqueue(ItemType item, long sequenceNumber) => 
            base.Channel.EnqueueWithoutDispatch(item, base.DequeueCallback);

        public override int EnqueuedCount =>
            base.Channel.InternalPendingItems;
    }
}

