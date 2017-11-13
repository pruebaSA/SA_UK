namespace System.Runtime.Remoting.Lifetime
{
    using System;
    using System.Runtime.Remoting.Messaging;

    internal class LeaseSink : IMessageSink
    {
        private Lease lease;
        private IMessageSink nextSink;

        public LeaseSink(Lease lease, IMessageSink nextSink)
        {
            this.lease = lease;
            this.nextSink = nextSink;
        }

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            this.lease.RenewOnCall();
            return this.nextSink.AsyncProcessMessage(msg, replySink);
        }

        public IMessage SyncProcessMessage(IMessage msg)
        {
            this.lease.RenewOnCall();
            return this.nextSink.SyncProcessMessage(msg);
        }

        public IMessageSink NextSink =>
            this.nextSink;
    }
}

