namespace System.ServiceModel
{
    using System;

    internal class CompletedAsyncResult : AsyncResult
    {
        public CompletedAsyncResult(AsyncCallback callback, object state) : base(callback, state)
        {
            base.Complete(true);
        }

        public CompletedAsyncResult(Exception exception, AsyncCallback callback, object state) : base(callback, state)
        {
            base.Complete(true, exception);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<CompletedAsyncResult>(result);
        }
    }
}

