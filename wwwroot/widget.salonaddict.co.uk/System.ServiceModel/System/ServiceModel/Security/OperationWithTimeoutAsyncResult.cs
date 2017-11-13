﻿namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.Threading;

    internal class OperationWithTimeoutAsyncResult : TraceAsyncResult
    {
        private OperationWithTimeoutCallback operationWithTimeout;
        private static readonly WaitCallback scheduledCallback = new WaitCallback(OperationWithTimeoutAsyncResult.OnScheduled);
        private TimeoutHelper timeoutHelper;

        public OperationWithTimeoutAsyncResult(OperationWithTimeoutCallback operationWithTimeout, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
        {
            this.operationWithTimeout = operationWithTimeout;
            this.timeoutHelper = new TimeoutHelper(timeout);
            IOThreadScheduler.ScheduleCallback(scheduledCallback, this);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<OperationWithTimeoutAsyncResult>(result);
        }

        private static void OnScheduled(object state)
        {
            OperationWithTimeoutAsyncResult result = (OperationWithTimeoutAsyncResult) state;
            Exception exception = null;
            try
            {
                using ((result.CallbackActivity == null) ? null : ServiceModelActivity.BoundOperation(result.CallbackActivity))
                {
                    result.operationWithTimeout(result.timeoutHelper.RemainingTime());
                }
            }
            catch (Exception exception2)
            {
                if (ExceptionUtility.IsFatal(exception2))
                {
                    throw;
                }
                exception = exception2;
            }
            result.Complete(false, exception);
        }
    }
}

