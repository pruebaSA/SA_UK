﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    internal class ReceiveTimeoutAsyncResult : AsyncResult
    {
        private static AsyncCallback innerCallback = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(ReceiveTimeoutAsyncResult.Callback));
        private IAsyncResult innerResult;
        private System.ServiceModel.TimeoutHelper timeoutHelper;

        internal ReceiveTimeoutAsyncResult(TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
        {
            this.timeoutHelper = new System.ServiceModel.TimeoutHelper(timeout);
        }

        private static void Callback(IAsyncResult result)
        {
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
            }
            ReceiveTimeoutAsyncResult asyncState = (ReceiveTimeoutAsyncResult) result.AsyncState;
            asyncState.InnerResult = result;
            asyncState.Complete(result.CompletedSynchronously);
        }

        internal AsyncCallback InnerCallback
        {
            get
            {
                if (innerCallback == null)
                {
                    innerCallback = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(ReceiveTimeoutAsyncResult.Callback));
                }
                return innerCallback;
            }
        }

        internal IAsyncResult InnerResult
        {
            get
            {
                if (this.innerResult == null)
                {
                    DiagnosticUtility.FailFast("ReceiveTimeoutAsyncResult.InnerResult: (this.innerResult != null)");
                }
                return this.innerResult;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (this.innerResult == null)
                {
                    this.innerResult = value;
                }
                else if (this.innerResult != value)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxAsyncResultsDontMatch0")));
                }
            }
        }

        internal object InnerState =>
            this;

        internal System.ServiceModel.TimeoutHelper TimeoutHelper =>
            this.timeoutHelper;
    }
}

