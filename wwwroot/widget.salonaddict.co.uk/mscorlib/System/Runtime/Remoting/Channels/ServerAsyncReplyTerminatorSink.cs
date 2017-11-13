namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Messaging;

    internal class ServerAsyncReplyTerminatorSink : IMessageSink
    {
        internal IMessageSink _nextSink;

        internal ServerAsyncReplyTerminatorSink(IMessageSink nextSink)
        {
            this._nextSink = nextSink;
        }

        public virtual IMessageCtrl AsyncProcessMessage(IMessage replyMsg, IMessageSink replySink) => 
            null;

        public virtual IMessage SyncProcessMessage(IMessage replyMsg)
        {
            Guid guid;
            RemotingServices.CORProfilerRemotingServerSendingReply(out guid, true);
            if (RemotingServices.CORProfilerTrackRemotingCookie())
            {
                replyMsg.Properties["CORProfilerCookie"] = guid;
            }
            return this._nextSink.SyncProcessMessage(replyMsg);
        }

        public IMessageSink NextSink =>
            this._nextSink;
    }
}

