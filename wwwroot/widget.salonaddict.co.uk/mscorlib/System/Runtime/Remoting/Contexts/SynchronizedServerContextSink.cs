namespace System.Runtime.Remoting.Contexts
{
    using System;
    using System.Runtime.Remoting.Messaging;

    internal class SynchronizedServerContextSink : InternalSink, IMessageSink
    {
        internal IMessageSink _nextSink;
        internal SynchronizationAttribute _property;

        internal SynchronizedServerContextSink(SynchronizationAttribute prop, IMessageSink nextSink)
        {
            this._property = prop;
            this._nextSink = nextSink;
        }

        public virtual IMessageCtrl AsyncProcessMessage(IMessage reqMsg, IMessageSink replySink)
        {
            WorkItem work = new WorkItem(reqMsg, this._nextSink, replySink);
            work.SetAsync();
            this._property.HandleWorkRequest(work);
            return null;
        }

        ~SynchronizedServerContextSink()
        {
            this._property.Dispose();
        }

        public virtual IMessage SyncProcessMessage(IMessage reqMsg)
        {
            WorkItem work = new WorkItem(reqMsg, this._nextSink, null);
            this._property.HandleWorkRequest(work);
            return work.ReplyMessage;
        }

        public IMessageSink NextSink =>
            this._nextSink;
    }
}

