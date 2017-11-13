namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading;

    internal class ThreadBehavior
    {
        private static WaitCallback cleanThreadCallback;
        private readonly SynchronizationContext context;
        private SendOrPostCallback threadAffinityCallback;

        internal ThreadBehavior(DispatchRuntime dispatch)
        {
            this.context = dispatch.SynchronizationContext;
        }

        internal void BindEndThread(ref MessageRpc rpc)
        {
            this.BindThread(ref rpc);
        }

        internal void BindThread(ref MessageRpc rpc)
        {
            SynchronizationContext context = rpc.InstanceContext.SynchronizationContext ?? this.context;
            if (context != null)
            {
                IResumeMessageRpc state = rpc.Pause();
                context.Post(this.ThreadAffinityCallbackDelegate, state);
            }
            else if (rpc.SwitchedThreads)
            {
                IResumeMessageRpc rpc3 = rpc.Pause();
                IOThreadScheduler.ScheduleCallback(CleanThreadCallbackDelegate, rpc3);
            }
        }

        private static void CleanThreadCallback(object state)
        {
            bool flag;
            ((IResumeMessageRpc) state).Resume(out flag);
        }

        internal static SynchronizationContext GetCurrentSynchronizationContext()
        {
            if (ServiceHostingEnvironment.ApplicationDomainHosted)
            {
                return null;
            }
            return SynchronizationContext.Current;
        }

        private void SynchronizationContextCallback(object state)
        {
            bool flag;
            ((IResumeMessageRpc) state).Resume(out flag);
            if (flag)
            {
                string message = System.ServiceModel.SR.GetString("SFxMultipleCallbackFromSynchronizationContext", new object[] { this.context.GetType().ToString() });
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(message));
            }
        }

        private static WaitCallback CleanThreadCallbackDelegate
        {
            get
            {
                if (cleanThreadCallback == null)
                {
                    cleanThreadCallback = new WaitCallback(ThreadBehavior.CleanThreadCallback);
                }
                return cleanThreadCallback;
            }
        }

        private SendOrPostCallback ThreadAffinityCallbackDelegate
        {
            get
            {
                if (this.threadAffinityCallback == null)
                {
                    this.threadAffinityCallback = new SendOrPostCallback(this.SynchronizationContextCallback);
                }
                return this.threadAffinityCallback;
            }
        }
    }
}

