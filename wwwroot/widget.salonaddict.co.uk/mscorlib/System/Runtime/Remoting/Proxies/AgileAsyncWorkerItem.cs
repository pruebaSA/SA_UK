namespace System.Runtime.Remoting.Proxies
{
    using System;
    using System.Runtime.Remoting.Messaging;

    internal class AgileAsyncWorkerItem
    {
        private AsyncResult _ar;
        private IMethodCallMessage _message;
        private object _target;

        public AgileAsyncWorkerItem(IMethodCallMessage message, AsyncResult ar, object target)
        {
            this._message = new MethodCall(message);
            this._ar = ar;
            this._target = target;
        }

        public void DoAsyncCall()
        {
            new StackBuilderSink(this._target).AsyncProcessMessage(this._message, this._ar);
        }

        public static void ThreadPoolCallBack(object o)
        {
            ((AgileAsyncWorkerItem) o).DoAsyncCall();
        }
    }
}

