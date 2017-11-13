namespace System.Runtime.Remoting.Messaging
{
    using System;

    internal class DisposeSink : IMessageSink
    {
        private IDisposable _iDis;
        private IMessageSink _replySink;

        internal DisposeSink(IDisposable iDis, IMessageSink replySink)
        {
            this._iDis = iDis;
            this._replySink = replySink;
        }

        public virtual IMessageCtrl AsyncProcessMessage(IMessage reqMsg, IMessageSink replySink)
        {
            throw new NotSupportedException();
        }

        public virtual IMessage SyncProcessMessage(IMessage reqMsg)
        {
            IMessage message = null;
            try
            {
                if (this._replySink != null)
                {
                    message = this._replySink.SyncProcessMessage(reqMsg);
                }
            }
            finally
            {
                this._iDis.Dispose();
            }
            return message;
        }

        public IMessageSink NextSink =>
            this._replySink;
    }
}

