namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security.Permissions;
    using System.Threading;

    [ComVisible(true)]
    public class AsyncResult : IAsyncResult, IMessageSink
    {
        private AsyncCallback _acbd;
        private Delegate _asyncDelegate;
        private object _asyncState;
        private ManualResetEvent _AsyncWaitHandle;
        private bool _endInvokeCalled;
        private bool _isCompleted;
        private IMessageCtrl _mc;
        private IMessage _replyMsg;

        internal AsyncResult(Message m)
        {
            m.GetAsyncBeginInfo(out this._acbd, out this._asyncState);
            this._asyncDelegate = (Delegate) m.GetThisPtr();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public virtual IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_Method"));
        }

        private void FaultInWaitHandle()
        {
            lock (this)
            {
                if (this._AsyncWaitHandle == null)
                {
                    this._AsyncWaitHandle = new ManualResetEvent(this._isCompleted);
                }
            }
        }

        public virtual IMessage GetReplyMessage() => 
            this._replyMsg;

        public virtual void SetMessageCtrl(IMessageCtrl mc)
        {
            this._mc = mc;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public virtual IMessage SyncProcessMessage(IMessage msg)
        {
            if (msg == null)
            {
                this._replyMsg = new ReturnMessage(new RemotingException(Environment.GetResourceString("Remoting_NullMessage")), new ErrorMessage());
            }
            else if (!(msg is IMethodReturnMessage))
            {
                this._replyMsg = new ReturnMessage(new RemotingException(Environment.GetResourceString("Remoting_Message_BadType")), new ErrorMessage());
            }
            else
            {
                this._replyMsg = msg;
            }
            lock (this)
            {
                this._isCompleted = true;
                if (this._AsyncWaitHandle != null)
                {
                    this._AsyncWaitHandle.Set();
                }
            }
            if (this._acbd != null)
            {
                this._acbd(this);
            }
            return null;
        }

        public virtual object AsyncDelegate =>
            this._asyncDelegate;

        public virtual object AsyncState =>
            this._asyncState;

        public virtual WaitHandle AsyncWaitHandle
        {
            get
            {
                this.FaultInWaitHandle();
                return this._AsyncWaitHandle;
            }
        }

        public virtual bool CompletedSynchronously =>
            false;

        public bool EndInvokeCalled
        {
            get => 
                this._endInvokeCalled;
            set
            {
                this._endInvokeCalled = value;
            }
        }

        public virtual bool IsCompleted =>
            this._isCompleted;

        public IMessageSink NextSink =>
            null;
    }
}

