namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;

    internal class SyncMethodInvoker : IOperationInvoker
    {
        private int inputParameterCount;
        private InvokeDelegate invokeDelegate;
        private MethodInfo method;
        private string methodName;
        private int outputParameterCount;
        private Type type;

        public SyncMethodInvoker(MethodInfo method)
        {
            if (method == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("method"));
            }
            this.method = method;
        }

        public SyncMethodInvoker(Type type, string methodName)
        {
            if (type == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
            }
            if (methodName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("methodName"));
            }
            this.type = type;
            this.methodName = methodName;
        }

        public object[] AllocateInputs()
        {
            this.EnsureIsInitialized();
            return EmptyArray.Allocate(this.inputParameterCount);
        }

        private void EnsureIsInitialized()
        {
            if (this.invokeDelegate == null)
            {
                this.EnsureIsInitializedCore();
            }
        }

        private void EnsureIsInitializedCore()
        {
            int num;
            int num2;
            InvokeDelegate delegate2 = new InvokerUtil().GenerateInvokeDelegate(this.Method, out num, out num2);
            this.outputParameterCount = num2;
            this.inputParameterCount = num;
            this.invokeDelegate = delegate2;
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            object obj2;
            this.EnsureIsInitialized();
            if (instance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxNoServiceObject")));
            }
            if (inputs == null)
            {
                if (this.inputParameterCount > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxInputParametersToServiceNull", new object[] { this.inputParameterCount })));
                }
            }
            else if (inputs.Length != this.inputParameterCount)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxInputParametersToServiceInvalid", new object[] { this.inputParameterCount, inputs.Length })));
            }
            outputs = EmptyArray.Allocate(this.outputParameterCount);
            long time = 0L;
            long num2 = 0L;
            bool flag = true;
            bool flag2 = false;
            if (PerformanceCounters.PerformanceCountersEnabled)
            {
                PerformanceCounters.MethodCalled(this.MethodName);
                try
                {
                    if (UnsafeNativeMethods.QueryPerformanceCounter(out time) == 0)
                    {
                        time = -1L;
                    }
                }
                catch (SecurityException exception)
                {
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityException(System.ServiceModel.SR.GetString("PartialTrustPerformanceCountersNotEnabled"), exception));
                }
            }
            try
            {
                ServiceModelActivity activity = null;
                IDisposable disposable = null;
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    activity = ServiceModelActivity.CreateBoundedActivity(true);
                    disposable = activity;
                }
                else if (TraceUtility.ShouldPropagateActivity)
                {
                    Guid activityId = ActivityIdHeader.ExtractActivityId(OperationContext.Current.IncomingMessage);
                    if (activityId != Guid.Empty)
                    {
                        disposable = Activity.CreateActivity(activityId);
                    }
                }
                using (disposable)
                {
                    if (DiagnosticUtility.ShouldUseActivity)
                    {
                        ServiceModelActivity.Start(activity, System.ServiceModel.SR.GetString("ActivityExecuteMethod", new object[] { this.method.DeclaringType.FullName, this.method.Name }), ActivityType.ExecuteUserCode);
                    }
                    obj2 = this.invokeDelegate(instance, inputs, outputs);
                    flag = false;
                }
                return obj2;
            }
            catch (FaultException)
            {
                flag2 = true;
                flag = false;
                throw;
            }
            catch (SecurityException exception2)
            {
                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Warning);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(AuthorizationBehavior.CreateAccessDeniedFaultException());
            }
            finally
            {
                if (PerformanceCounters.PerformanceCountersEnabled)
                {
                    if (flag)
                    {
                        PerformanceCounters.MethodReturnedError(this.MethodName);
                    }
                    else if (flag2)
                    {
                        PerformanceCounters.MethodReturnedFault(this.MethodName);
                    }
                    else
                    {
                        long num3;
                        if ((time >= 0L) && (UnsafeNativeMethods.QueryPerformanceCounter(out num2) != 0))
                        {
                            num3 = num2 - time;
                        }
                        else
                        {
                            num3 = 0L;
                        }
                        PerformanceCounters.MethodReturnedSuccess(this.MethodName, num3);
                    }
                }
            }
            return obj2;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
        }

        public bool IsSynchronous =>
            true;

        public MethodInfo Method
        {
            get
            {
                if (this.method == null)
                {
                    this.method = this.type.GetMethod(this.methodName);
                }
                return this.method;
            }
        }

        public string MethodName
        {
            get
            {
                if (this.methodName == null)
                {
                    this.methodName = this.method.Name;
                }
                return this.methodName;
            }
        }
    }
}

