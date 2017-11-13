namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Contexts;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;

    internal class AsyncWorkItem : IMessageSink
    {
        private LogicalCallContext _callCtx;
        private Context _oldCtx;
        private IMessageSink _replySink;
        private IMessage _reqMsg;
        private ServerIdentity _srvID;

        internal AsyncWorkItem(IMessageSink replySink, Context oldCtx) : this(null, replySink, oldCtx, null)
        {
        }

        internal AsyncWorkItem(IMessage reqMsg, IMessageSink replySink, Context oldCtx, ServerIdentity srvID)
        {
            this._reqMsg = reqMsg;
            this._replySink = replySink;
            this._oldCtx = oldCtx;
            this._callCtx = CallContext.GetLogicalCallContext();
            this._srvID = srvID;
        }

        public virtual IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_Method"));
        }

        internal virtual void FinishAsyncWork(object stateIgnored)
        {
            InternalCrossContextDelegate ftnToCall = new InternalCrossContextDelegate(AsyncWorkItem.FinishAsyncWorkCallback);
            object[] args = new object[] { this };
            Thread.CurrentThread.InternalCrossContextCallback(this._srvID.ServerContext, ftnToCall, args);
        }

        internal static object FinishAsyncWorkCallback(object[] args)
        {
            AsyncWorkItem replySink = (AsyncWorkItem) args[0];
            Context serverContext = replySink._srvID.ServerContext;
            LogicalCallContext callCtx = CallContext.SetLogicalCallContext(replySink._callCtx);
            serverContext.NotifyDynamicSinks(replySink._reqMsg, false, true, true, true);
            serverContext.GetServerContextChain().AsyncProcessMessage(replySink._reqMsg, replySink);
            CallContext.SetLogicalCallContext(callCtx);
            return null;
        }

        public virtual IMessage SyncProcessMessage(IMessage msg)
        {
            IMessage message = null;
            if (this._replySink != null)
            {
                Thread.CurrentContext.NotifyDynamicSinks(msg, false, false, true, true);
                object[] args = new object[] { this._replySink, msg };
                InternalCrossContextDelegate ftnToCall = new InternalCrossContextDelegate(AsyncWorkItem.SyncProcessMessageCallback);
                message = (IMessage) Thread.CurrentThread.InternalCrossContextCallback(this._oldCtx, ftnToCall, args);
            }
            return message;
        }

        internal static object SyncProcessMessageCallback(object[] args)
        {
            IMessageSink sink = (IMessageSink) args[0];
            IMessage msg = (IMessage) args[1];
            return sink.SyncProcessMessage(msg);
        }

        public IMessageSink NextSink =>
            this._replySink;
    }
}

