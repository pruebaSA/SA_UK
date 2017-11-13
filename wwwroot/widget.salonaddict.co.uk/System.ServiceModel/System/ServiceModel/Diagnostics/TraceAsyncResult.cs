namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.ServiceModel;

    internal abstract class TraceAsyncResult : AsyncResult
    {
        protected TraceAsyncResult(AsyncCallback callback, object state) : base(callback, state)
        {
            if (DiagnosticUtility.ShouldUseActivity)
            {
                base.CallbackActivity = ServiceModelActivity.Current;
            }
        }
    }
}

