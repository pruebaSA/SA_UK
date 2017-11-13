namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Threading;

    internal static class IOThreadScheduler
    {
        [SecurityCritical, SecurityTreatAsSafe]
        public static void ScheduleCallback(WaitCallback callback, object state)
        {
            CriticalHelper.ScheduleCallback(callback, state, true, false);
        }

        [SecurityCritical]
        public static void ScheduleCallbackLowPriNoFlow(WaitCallback callback, object state)
        {
            CriticalHelper.ScheduleCallback(callback, state, false, true);
        }

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private static class CriticalHelper
        {
            private static object lockObject = new object();
            private static Queue<WorkItem> lowPriQueue = new Queue<WorkItem>();
            private static ScheduledOverlapped overlapped = new ScheduledOverlapped(new WaitCallback(IOThreadScheduler.CriticalHelper.CompletionCallback));
            private static bool queuedCompletion;
            private static Queue<WorkItem> workQueue = new Queue<WorkItem>();

            private static void CompletionCallback(object state)
            {
                lock (lockObject)
                {
                    queuedCompletion = false;
                }
                ThreadTrace.Trace("IOThreadScheduler.CompletionCallback");
                ProcessCallbacks();
            }

            private static void ProcessCallbacks()
            {
                while (true)
                {
                    WorkItem item;
                    bool flag = false;
                    try
                    {
                        lock (lockObject)
                        {
                            if (workQueue.Count != 0)
                            {
                                item = workQueue.Dequeue();
                            }
                            else if (lowPriQueue.Count != 0)
                            {
                                item = lowPriQueue.Dequeue();
                            }
                            else
                            {
                                return;
                            }
                            if (!queuedCompletion && ((workQueue.Count > 0) || (lowPriQueue.Count > 0)))
                            {
                                flag = true;
                                queuedCompletion = true;
                            }
                        }
                    }
                    finally
                    {
                        if (flag)
                        {
                            overlapped.Post();
                        }
                    }
                    item.Invoke();
                }
            }

            public static void ScheduleCallback(WaitCallback callback, object state, bool flow, bool lowPri)
            {
                if (callback == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("callback"));
                }
                object obj2 = state;
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    obj2 = new TraceUtility.TracingAsyncCallbackState(state);
                }
                ThreadTrace.Trace("IOThreadScheduler.ScheduleCallback");
                WorkItem item = new WorkItem(callback, obj2, flow);
                bool flag = false;
                try
                {
                    lock (lockObject)
                    {
                        (lowPri ? lowPriQueue : workQueue).Enqueue(item);
                        if (!queuedCompletion)
                        {
                            flag = true;
                            queuedCompletion = true;
                        }
                    }
                }
                finally
                {
                    if (flag)
                    {
                        overlapped.Post();
                    }
                }
            }

            private class ScheduledOverlapped
            {
                private WaitCallback callback;
                private unsafe NativeOverlapped* nativeOverlapped;

                public unsafe ScheduledOverlapped(WaitCallback callback)
                {
                    this.nativeOverlapped = new Overlapped(0, 0, IntPtr.Zero, null).UnsafePack(DiagnosticUtility.Utility.ThunkCallback(new IOCompletionCallback(this.IOCallback)), null);
                    this.callback = callback;
                }

                private unsafe void IOCallback(uint errorCode, uint numBytes, NativeOverlapped* nativeOverlapped)
                {
                    this.callback(null);
                }

                public unsafe void Post()
                {
                    ThreadPool.UnsafeQueueNativeOverlapped(this.nativeOverlapped);
                }
            }

            private class WorkItem
            {
                private WaitCallback callback;
                private SecurityContext context;
                private static ContextCallback securityContextCallback = new ContextCallback(IOThreadScheduler.CriticalHelper.WorkItem.OnSecurityContextCallback);
                private object state;

                public WorkItem(WaitCallback callback, object state, bool flow)
                {
                    this.callback = callback;
                    this.state = state;
                    this.context = flow ? PartialTrustHelpers.CaptureSecurityContextNoIdentityFlow() : null;
                }

                public void Invoke()
                {
                    if (this.context != null)
                    {
                        SecurityContext.Run(this.context, securityContextCallback, this);
                    }
                    else
                    {
                        this.Invoke2();
                    }
                }

                private void Invoke2()
                {
                    TraceUtility.TracingAsyncCallbackState state = this.state as TraceUtility.TracingAsyncCallbackState;
                    if (state == null)
                    {
                        this.callback(this.state);
                    }
                    else
                    {
                        using (DiagnosticUtility.ShouldUseActivity ? Activity.CreateActivity(state.ActivityId) : null)
                        {
                            this.callback(state.InnerState);
                        }
                    }
                }

                private static void OnSecurityContextCallback(object o)
                {
                    ((IOThreadScheduler.CriticalHelper.WorkItem) o).Invoke2();
                }
            }
        }
    }
}

