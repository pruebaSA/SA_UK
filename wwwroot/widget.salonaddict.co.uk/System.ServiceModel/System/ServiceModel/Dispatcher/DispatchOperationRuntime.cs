namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Security;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;

    internal class DispatchOperationRuntime
    {
        private readonly string action;
        private readonly ICallContextInitializer[] callContextInitializers;
        private readonly bool deserializeRequest;
        private readonly bool disposeParameters;
        private readonly IDispatchFaultFormatter faultFormatter;
        private readonly IDispatchMessageFormatter formatter;
        private readonly ImpersonationOption impersonation;
        private readonly IParameterInspector[] inspectors;
        private static AsyncCallback invokeCallback = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(DispatchOperationRuntime.InvokeCallback));
        private readonly IOperationInvoker invoker;
        private readonly bool isOneWay;
        private readonly bool isSynchronous;
        private readonly bool isTerminating;
        private readonly string name;
        private readonly ImmutableDispatchRuntime parent;
        private readonly bool releaseInstanceAfterCall;
        private readonly bool releaseInstanceBeforeCall;
        private readonly string replyAction;
        private readonly bool serializeReply;
        private readonly bool transactionAutoComplete;
        private readonly bool transactionRequired;

        internal DispatchOperationRuntime(DispatchOperation operation, ImmutableDispatchRuntime parent)
        {
            if (operation == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operation");
            }
            if (parent == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parent");
            }
            if (operation.Invoker == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("RuntimeRequiresInvoker0")));
            }
            this.disposeParameters = operation.AutoDisposeParameters && !operation.HasNoDisposableParameters;
            this.parent = parent;
            this.callContextInitializers = EmptyArray<ICallContextInitializer>.ToArray(operation.CallContextInitializers);
            this.inspectors = EmptyArray<IParameterInspector>.ToArray(operation.ParameterInspectors);
            this.faultFormatter = operation.FaultFormatter;
            this.impersonation = operation.Impersonation;
            this.deserializeRequest = operation.DeserializeRequest;
            this.serializeReply = operation.SerializeReply;
            this.formatter = operation.Formatter;
            this.invoker = operation.Invoker;
            try
            {
                this.isSynchronous = operation.Invoker.IsSynchronous;
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
            }
            this.isTerminating = operation.IsTerminating;
            this.action = operation.Action;
            this.name = operation.Name;
            this.releaseInstanceAfterCall = operation.ReleaseInstanceAfterCall;
            this.releaseInstanceBeforeCall = operation.ReleaseInstanceBeforeCall;
            this.replyAction = operation.ReplyAction;
            this.isOneWay = operation.IsOneWay;
            this.transactionAutoComplete = operation.TransactionAutoComplete;
            this.transactionRequired = operation.TransactionRequired;
            if ((this.formatter == null) && (this.deserializeRequest || this.serializeReply))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("DispatchRuntimeRequiresFormatter0", new object[] { this.name })));
            }
            if ((operation.Parent.InstanceProvider == null) && (operation.Parent.Type != null))
            {
                SyncMethodInvoker invoker = this.invoker as SyncMethodInvoker;
                if (invoker != null)
                {
                    this.ValidateInstanceType(operation.Parent.Type, invoker.Method);
                }
                AsyncMethodInvoker invoker2 = this.invoker as AsyncMethodInvoker;
                if (invoker2 != null)
                {
                    this.ValidateInstanceType(operation.Parent.Type, invoker2.BeginMethod);
                    this.ValidateInstanceType(operation.Parent.Type, invoker2.EndMethod);
                }
            }
        }

        private void DeserializeInputs(ref MessageRpc rpc)
        {
            bool flag = false;
            try
            {
                try
                {
                    rpc.InputParameters = this.Invoker.AllocateInputs();
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    if (ErrorBehavior.ShouldRethrowExceptionAsIs(exception))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
                try
                {
                    if (this.deserializeRequest)
                    {
                        this.Formatter.DeserializeRequest(rpc.Request, rpc.InputParameters);
                    }
                    else
                    {
                        rpc.InputParameters[0] = rpc.Request;
                    }
                    flag = true;
                }
                catch (Exception exception2)
                {
                    if (DiagnosticUtility.IsFatal(exception2))
                    {
                        throw;
                    }
                    if (ErrorBehavior.ShouldRethrowExceptionAsIs(exception2))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception2);
                }
            }
            finally
            {
                rpc.DidDeserializeRequestBody = rpc.Request.State != MessageState.Created;
                if (!flag && MessageLogger.LoggingEnabled)
                {
                    MessageLogger.LogMessage(ref rpc.Request, MessageLoggingSource.Malformed);
                }
            }
        }

        private void InitializeCallContext(ref MessageRpc rpc)
        {
            if (this.CallContextInitializers.Length > 0)
            {
                this.InitializeCallContextCore(ref rpc);
            }
        }

        private void InitializeCallContextCore(ref MessageRpc rpc)
        {
            IClientChannel proxy = rpc.Channel.Proxy as IClientChannel;
            int callContextCorrelationOffset = this.Parent.CallContextCorrelationOffset;
            try
            {
                for (int i = 0; i < rpc.Operation.CallContextInitializers.Length; i++)
                {
                    rpc.Correlation[callContextCorrelationOffset + i] = this.CallContextInitializers[i].BeforeInvoke(rpc.InstanceContext, proxy, rpc.Request);
                }
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                if (ErrorBehavior.ShouldRethrowExceptionAsIs(exception))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
            }
        }

        private void InspectInputs(ref MessageRpc rpc)
        {
            if (this.ParameterInspectors.Length > 0)
            {
                this.InspectInputsCore(ref rpc);
            }
        }

        private void InspectInputsCore(ref MessageRpc rpc)
        {
            int parameterInspectorCorrelationOffset = this.Parent.ParameterInspectorCorrelationOffset;
            for (int i = 0; i < this.ParameterInspectors.Length; i++)
            {
                rpc.Correlation[parameterInspectorCorrelationOffset + i] = this.ParameterInspectors[i].BeforeCall(this.Name, rpc.InputParameters);
            }
        }

        private void InspectOutputs(ref MessageRpc rpc)
        {
            if (this.ParameterInspectors.Length > 0)
            {
                this.InspectOutputsCore(ref rpc);
            }
        }

        private void InspectOutputsCore(ref MessageRpc rpc)
        {
            int parameterInspectorCorrelationOffset = this.Parent.ParameterInspectorCorrelationOffset;
            for (int i = this.ParameterInspectors.Length - 1; i >= 0; i--)
            {
                this.ParameterInspectors[i].AfterCall(this.Name, rpc.OutputParameters, rpc.ReturnParameter, rpc.Correlation[parameterInspectorCorrelationOffset + i]);
            }
        }

        [SecurityTreatAsSafe, SecurityCritical, DebuggerStepperBoundary]
        internal void InvokeBegin(ref MessageRpc rpc)
        {
            if (rpc.Error == null)
            {
                try
                {
                    this.InitializeCallContext(ref rpc);
                    object instance = rpc.Instance;
                    this.DeserializeInputs(ref rpc);
                    this.InspectInputs(ref rpc);
                    this.ValidateMustUnderstand(ref rpc);
                    IAsyncResult result = null;
                    IDisposable impersonationContext = null;
                    IPrincipal originalPrincipal = null;
                    bool isThreadPrincipalSet = false;
                    try
                    {
                        if (this.parent.SecurityImpersonation != null)
                        {
                            this.parent.SecurityImpersonation.StartImpersonation(ref rpc, out impersonationContext, out originalPrincipal, out isThreadPrincipalSet);
                        }
                        if (this.isSynchronous)
                        {
                            rpc.ReturnParameter = this.Invoker.Invoke(instance, rpc.InputParameters, out rpc.OutputParameters);
                        }
                        else
                        {
                            bool flag2 = false;
                            IResumeMessageRpc state = rpc.Pause();
                            try
                            {
                                result = this.Invoker.InvokeBegin(instance, rpc.InputParameters, invokeCallback, state);
                                flag2 = true;
                            }
                            finally
                            {
                                if (!flag2)
                                {
                                    rpc.UnPause();
                                }
                            }
                        }
                    }
                    finally
                    {
                        try
                        {
                            if (this.parent.SecurityImpersonation != null)
                            {
                                this.parent.SecurityImpersonation.StopImpersonation(ref rpc, impersonationContext, originalPrincipal, isThreadPrincipalSet);
                            }
                        }
                        catch
                        {
                            string message = null;
                            try
                            {
                                message = System.ServiceModel.SR.GetString("SFxRevertImpersonationFailed0");
                            }
                            finally
                            {
                                DiagnosticUtility.FailFast(message);
                            }
                        }
                    }
                    if (this.isSynchronous)
                    {
                        this.InspectOutputs(ref rpc);
                        this.SerializeOutputs(ref rpc);
                    }
                    else
                    {
                        if (result == null)
                        {
                            throw TraceUtility.ThrowHelperError(new ArgumentNullException("IOperationInvoker.BeginDispatch"), rpc.Request);
                        }
                        if (rpc.CompletedSynchronously.Value)
                        {
                            rpc.UnPause();
                            rpc.AsyncResult = result;
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    this.UninitializeCallContext(ref rpc);
                }
            }
        }

        private static void InvokeCallback(IAsyncResult result)
        {
            IResumeMessageRpc asyncState = result.AsyncState as IResumeMessageRpc;
            if (asyncState == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("SFxInvalidAsyncResultState0"));
            }
            if (ImmutableDispatchRuntime.processMessage5IsOnTheStack == asyncState.AsyncOperationWaitEvent)
            {
                asyncState.SetCompletedSynchronously();
            }
            else
            {
                asyncState.Resume(result);
            }
        }

        [SecurityTreatAsSafe, SecurityCritical, DebuggerStepperBoundary]
        internal void InvokeEnd(ref MessageRpc rpc)
        {
            if ((rpc.Error == null) && !this.isSynchronous)
            {
                try
                {
                    this.InitializeCallContext(ref rpc);
                    IDisposable impersonationContext = null;
                    IPrincipal originalPrincipal = null;
                    bool isThreadPrincipalSet = false;
                    try
                    {
                        if (this.parent.SecurityImpersonation != null)
                        {
                            this.parent.SecurityImpersonation.StartImpersonation(ref rpc, out impersonationContext, out originalPrincipal, out isThreadPrincipalSet);
                        }
                        rpc.ReturnParameter = this.Invoker.InvokeEnd(rpc.Instance, out rpc.OutputParameters, rpc.AsyncResult);
                    }
                    finally
                    {
                        try
                        {
                            if (this.parent.SecurityImpersonation != null)
                            {
                                this.parent.SecurityImpersonation.StopImpersonation(ref rpc, impersonationContext, originalPrincipal, isThreadPrincipalSet);
                            }
                        }
                        catch
                        {
                            string message = null;
                            try
                            {
                                message = System.ServiceModel.SR.GetString("SFxRevertImpersonationFailed0");
                            }
                            finally
                            {
                                DiagnosticUtility.FailFast(message);
                            }
                        }
                    }
                    this.InspectOutputs(ref rpc);
                    this.SerializeOutputs(ref rpc);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    this.UninitializeCallContext(ref rpc);
                }
            }
        }

        private void SerializeOutputs(ref MessageRpc rpc)
        {
            if (!this.IsOneWay && this.parent.EnableFaults)
            {
                Message returnParameter;
                if (this.serializeReply)
                {
                    try
                    {
                        returnParameter = this.Formatter.SerializeReply(rpc.RequestVersion, rpc.OutputParameters, rpc.ReturnParameter);
                    }
                    catch (Exception exception)
                    {
                        if (DiagnosticUtility.IsFatal(exception))
                        {
                            throw;
                        }
                        if (ErrorBehavior.ShouldRethrowExceptionAsIs(exception))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                    }
                    if (returnParameter == null)
                    {
                        object[] args = new object[] { this.Formatter.GetType().ToString(), this.name ?? "" };
                        ErrorBehavior.ThrowAndCatch(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxNullReplyFromFormatter2", args)));
                    }
                }
                else
                {
                    if ((rpc.ReturnParameter == null) && (rpc.OperationContext.RequestContext != null))
                    {
                        ErrorBehavior.ThrowAndCatch(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxDispatchRuntimeMessageCannotBeNull", new object[] { this.name })));
                    }
                    returnParameter = (Message) rpc.ReturnParameter;
                    if ((returnParameter != null) && !ProxyOperationRuntime.IsValidAction(returnParameter, this.ReplyAction))
                    {
                        object[] objArray3 = new object[] { this.Name, returnParameter.Headers.Action ?? "{NULL}", this.ReplyAction };
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxInvalidReplyAction", objArray3)));
                    }
                }
                if ((DiagnosticUtility.ShouldUseActivity && (rpc.Activity != null)) && (returnParameter != null))
                {
                    TraceUtility.SetActivity(returnParameter, rpc.Activity);
                    if (TraceUtility.ShouldPropagateActivity)
                    {
                        TraceUtility.AddActivityHeader(returnParameter);
                    }
                }
                else if ((TraceUtility.ShouldPropagateActivity && (returnParameter != null)) && (rpc.ResponseActivityId != Guid.Empty))
                {
                    new ActivityIdHeader(rpc.ResponseActivityId).AddTo(returnParameter);
                }
                if (MessageLogger.LoggingEnabled && (returnParameter != null))
                {
                    MessageLogger.LogMessage(ref returnParameter, MessageLoggingSource.LastChance | MessageLoggingSource.ServiceLevelSendReply);
                }
                rpc.Reply = returnParameter;
            }
        }

        private void UninitializeCallContext(ref MessageRpc rpc)
        {
            if (this.CallContextInitializers.Length > 0)
            {
                this.UninitializeCallContextCore(ref rpc);
            }
        }

        private void UninitializeCallContextCore(ref MessageRpc rpc)
        {
            object proxy = rpc.Channel.Proxy;
            int callContextCorrelationOffset = this.Parent.CallContextCorrelationOffset;
            try
            {
                for (int i = this.CallContextInitializers.Length - 1; i >= 0; i--)
                {
                    this.CallContextInitializers[i].AfterInvoke(rpc.Correlation[callContextCorrelationOffset + i]);
                }
            }
            catch (Exception exception)
            {
                DiagnosticUtility.FailFast(string.Format(CultureInfo.InvariantCulture, "ICallContextInitializer.BeforeInvoke threw an exception of type {0}: {1}", new object[] { exception.GetType(), exception.Message }));
            }
        }

        private void ValidateInstanceType(Type type, MethodInfo method)
        {
            if (!method.DeclaringType.IsAssignableFrom(type))
            {
                string message = System.ServiceModel.SR.GetString("SFxMethodNotSupportedByType2", new object[] { type.FullName, method.DeclaringType.FullName });
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(message));
            }
        }

        private void ValidateMustUnderstand(ref MessageRpc rpc)
        {
            if (this.parent.ValidateMustUnderstand)
            {
                rpc.NotUnderstoodHeaders = rpc.Request.Headers.GetHeadersNotUnderstood();
                if (rpc.NotUnderstoodHeaders != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MustUnderstandSoapException(rpc.NotUnderstoodHeaders, rpc.Request.Version.Envelope));
                }
            }
        }

        internal string Action =>
            this.action;

        internal ICallContextInitializer[] CallContextInitializers =>
            this.callContextInitializers;

        internal bool DisposeParameters =>
            this.disposeParameters;

        internal IDispatchFaultFormatter FaultFormatter =>
            this.faultFormatter;

        internal IDispatchMessageFormatter Formatter =>
            this.formatter;

        internal bool HasDefaultUnhandledActionInvoker =>
            (this.invoker is DispatchRuntime.UnhandledActionInvoker);

        internal ImpersonationOption Impersonation =>
            this.impersonation;

        internal IOperationInvoker Invoker =>
            this.invoker;

        internal bool IsOneWay =>
            this.isOneWay;

        internal bool IsSynchronous =>
            this.isSynchronous;

        internal bool IsTerminating =>
            this.isTerminating;

        internal string Name =>
            this.name;

        internal IParameterInspector[] ParameterInspectors =>
            this.inspectors;

        internal ImmutableDispatchRuntime Parent =>
            this.parent;

        internal bool ReleaseInstanceAfterCall =>
            this.releaseInstanceAfterCall;

        internal bool ReleaseInstanceBeforeCall =>
            this.releaseInstanceBeforeCall;

        internal string ReplyAction =>
            this.replyAction;

        internal bool SerializeReply =>
            this.serializeReply;

        internal bool TransactionAutoComplete =>
            this.transactionAutoComplete;

        internal bool TransactionRequired =>
            this.transactionRequired;
    }
}

