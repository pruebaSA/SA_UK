namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.Threading;

    internal static class TraceUtility
    {
        private const string ActivityIdKey = "ActivityId";
        private const string AsyncOperationActivityKey = "AsyncOperationActivity";
        private static long messageNumber;
        private static bool shouldPropagateActivity;

        static TraceUtility()
        {
            if (DiagnosticUtility.DiagnosticTrace != null)
            {
                DiagnosticTraceSource traceSource = (DiagnosticTraceSource) DiagnosticUtility.DiagnosticTrace.TraceSource;
                shouldPropagateActivity = traceSource.PropagateActivity;
            }
        }

        internal static void AddActivityHeader(Message message)
        {
            try
            {
                new ActivityIdHeader(ExtractActivityId(message)).AddTo(message);
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                TraceEvent(TraceEventType.Error, TraceCode.FailedToAddAnActivityIdHeader, exception, message);
            }
        }

        internal static void AddAmbientActivityToMessage(Message message)
        {
            try
            {
                new ActivityIdHeader(DiagnosticTrace.ActivityId).AddTo(message);
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                TraceEvent(TraceEventType.Error, TraceCode.FailedToAddAnActivityIdHeader, exception, message);
            }
        }

        internal static void CopyActivity(Message source, Message destination)
        {
            if (DiagnosticUtility.ShouldUseActivity)
            {
                SetActivity(destination, ExtractActivity(source));
            }
        }

        private static string Description(TraceCode traceCode) => 
            System.ServiceModel.SR.GetString("TraceCode" + DiagnosticTrace.CodeToString(traceCode));

        internal static ServiceModelActivity ExtractActivity(Message message)
        {
            ServiceModelActivity activity = null;
            object obj2;
            if ((DiagnosticUtility.ShouldUseActivity && (message != null)) && ((message.State != MessageState.Closed) && message.Properties.TryGetValue("ActivityId", out obj2)))
            {
                activity = obj2 as ServiceModelActivity;
            }
            return activity;
        }

        internal static Guid ExtractActivityId(Message message)
        {
            ServiceModelActivity activity = ExtractActivity(message);
            if (activity != null)
            {
                return activity.Id;
            }
            return Guid.Empty;
        }

        internal static ServiceModelActivity ExtractAndRemoveActivity(Message message)
        {
            ServiceModelActivity activity = ExtractActivity(message);
            if (activity != null)
            {
                message.Properties["ActivityId"] = false;
            }
            return activity;
        }

        internal static object ExtractAsyncOperationContextActivity()
        {
            object obj2 = null;
            if ((OperationContext.Current != null) && OperationContext.Current.OutgoingMessageProperties.TryGetValue("AsyncOperationActivity", out obj2))
            {
                OperationContext.Current.OutgoingMessageProperties.Remove("AsyncOperationActivity");
            }
            return obj2;
        }

        internal static void ProcessIncomingMessage(Message message)
        {
            ServiceModelActivity current = ServiceModelActivity.Current;
            if (DiagnosticUtility.ShouldUseActivity)
            {
                ServiceModelActivity activity = ExtractActivity(message);
                if ((activity != null) && (activity.Id != current.Id))
                {
                    using (ServiceModelActivity.BoundOperation(activity))
                    {
                        DiagnosticUtility.DiagnosticTrace.TraceTransfer(current.Id);
                    }
                }
            }
            if ((current != null) && DiagnosticUtility.ShouldUseActivity)
            {
                SetActivity(message, current);
            }
            if (MessageLogger.LogMessagesAtServiceLevel)
            {
                MessageLogger.LogMessage(ref message, MessageLoggingSource.LastChance | MessageLoggingSource.ServiceLevelReceiveReply);
            }
        }

        internal static void ProcessOutgoingMessage(Message message)
        {
            ServiceModelActivity current = ServiceModelActivity.Current;
            if (DiagnosticUtility.ShouldUseActivity)
            {
                SetActivity(message, current);
            }
            if (PropagateUserActivity || ShouldPropagateActivity)
            {
                AddAmbientActivityToMessage(message);
            }
            if (MessageLogger.LogMessagesAtServiceLevel)
            {
                MessageLogger.LogMessage(ref message, MessageLoggingSource.LastChance | MessageLoggingSource.ServiceLevelSendRequest);
            }
        }

        public static long RetrieveMessageNumber() => 
            Interlocked.Increment(ref messageNumber);

        internal static void SetActivity(Message message, ServiceModelActivity activity)
        {
            if ((DiagnosticUtility.ShouldUseActivity && (message != null)) && (message.State != MessageState.Closed))
            {
                message.Properties["ActivityId"] = activity;
            }
        }

        internal static ArgumentException ThrowHelperArgument(string paramName, string message, Message msg) => 
            ((ArgumentException) ThrowHelperError(new ArgumentException(message, paramName), msg));

        internal static ArgumentNullException ThrowHelperArgumentNull(string paramName, Message message) => 
            ((ArgumentNullException) ThrowHelperError(new ArgumentNullException(paramName), message));

        internal static Exception ThrowHelperError(Exception exception, Message message)
        {
            Guid activityId = ExtractActivityId(message);
            if (DiagnosticUtility.ShouldTraceError)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.ThrowingException, TraceSR.GetString("ThrowingException"), null, exception, activityId, null);
            }
            return exception;
        }

        internal static Exception ThrowHelperError(Exception exception, Guid activityId, object source)
        {
            if (DiagnosticUtility.ShouldTraceError)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.ThrowingException, TraceSR.GetString("ThrowingException"), null, exception, activityId, source);
            }
            return exception;
        }

        internal static Exception ThrowHelperWarning(Exception exception, Message message)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                Guid activityId = ExtractActivityId(message);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.ThrowingException, TraceSR.GetString("ThrowingException"), null, exception, activityId, null);
            }
            return exception;
        }

        internal static void TraceDroppedMessage(Message message, EndpointDispatcher dispatcher)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                EndpointAddress endpointAddress = null;
                if (dispatcher != null)
                {
                    endpointAddress = dispatcher.EndpointAddress;
                }
                TraceEvent(TraceEventType.Information, TraceCode.DroppedAMessage, new MessageDroppedTraceRecord(message, endpointAddress));
            }
        }

        internal static void TraceEvent(TraceEventType severity, TraceCode traceCode, object source)
        {
            TraceEvent(severity, traceCode, null, source, null);
        }

        internal static void TraceEvent(TraceEventType severity, TraceCode traceCode, Message message)
        {
            if (message == null)
            {
                TraceEvent(severity, traceCode, null, (Exception) null);
            }
            else
            {
                TraceEvent(severity, traceCode, message, message);
            }
        }

        internal static void TraceEvent(TraceEventType severity, TraceCode traceCode, Exception exception, Message message)
        {
            Guid activityId = ExtractActivityId(message);
            if (DiagnosticUtility.ShouldTrace(severity))
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(severity, traceCode, Description(traceCode), new MessageTraceRecord(message), exception, activityId, null);
            }
        }

        internal static void TraceEvent(TraceEventType severity, TraceCode traceCode, object source, Exception exception)
        {
            TraceEvent(severity, traceCode, null, source, exception);
        }

        internal static void TraceEvent(TraceEventType severity, TraceCode traceCode, object source, Message message)
        {
            Guid activityId = ExtractActivityId(message);
            if (DiagnosticUtility.ShouldTrace(severity))
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(severity, traceCode, Description(traceCode), new MessageTraceRecord(message), null, activityId, message);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void TraceEvent(TraceEventType severity, TraceCode traceCode, TraceRecord extendedData, object source, Exception exception)
        {
            if (DiagnosticUtility.ShouldTrace(severity))
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(severity, traceCode, Description(traceCode), extendedData, exception, source);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void TraceEvent(TraceEventType severity, TraceCode traceCode, TraceRecord extendedData, object source, Exception exception, Message message)
        {
            Guid activityId = ExtractActivityId(message);
            if (DiagnosticUtility.ShouldTrace(severity))
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(severity, traceCode, Description(traceCode), extendedData, exception, activityId, source);
            }
        }

        internal static void TraceHttpConnectionInformation(string localEndpoint, string remoteEndpoint, object source)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>(2) {
                    ["LocalEndpoint"] = localEndpoint,
                    ["RemoteEndpoint"] = remoteEndpoint
                };
                TraceEvent(TraceEventType.Information, TraceCode.ConnectToIPEndpoint, new DictionaryTraceRecord(dictionary), source, null);
            }
        }

        internal static void TraceUserCodeException(Exception e, MethodInfo method)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                StringTraceRecord trace = new StringTraceRecord("Comment", System.ServiceModel.SR.GetString("SFxUserCodeThrewException", new object[] { method.DeclaringType.FullName, method.Name }));
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.UnhandledExceptionInUserOperation, System.ServiceModel.SR.GetString("TraceCodeUnhandledExceptionInUserOperation", new object[] { method.DeclaringType.FullName, method.Name }), trace, e);
            }
        }

        internal static void TransferFromTransport(Message message)
        {
            if ((message != null) && DiagnosticUtility.ShouldUseActivity)
            {
                Guid empty = Guid.Empty;
                if (ShouldPropagateActivity)
                {
                    empty = ActivityIdHeader.ExtractActivityId(message);
                }
                if (empty == Guid.Empty)
                {
                    empty = Guid.NewGuid();
                }
                ServiceModelActivity current = null;
                bool flag = true;
                if (ServiceModelActivity.Current != null)
                {
                    if ((ServiceModelActivity.Current.Id == empty) || (ServiceModelActivity.Current.ActivityType == ActivityType.ProcessAction))
                    {
                        current = ServiceModelActivity.Current;
                        flag = false;
                    }
                    else if ((ServiceModelActivity.Current.PreviousActivity != null) && (ServiceModelActivity.Current.PreviousActivity.Id == empty))
                    {
                        current = ServiceModelActivity.Current.PreviousActivity;
                        flag = false;
                    }
                }
                if (current == null)
                {
                    current = ServiceModelActivity.CreateActivity(empty);
                }
                if (DiagnosticUtility.ShouldUseActivity && flag)
                {
                    DiagnosticUtility.DiagnosticTrace.TraceTransfer(empty);
                    ServiceModelActivity.Start(current, System.ServiceModel.SR.GetString("ActivityProcessAction", new object[] { message.Headers.Action }), ActivityType.ProcessAction);
                }
                message.Properties["ActivityId"] = current;
            }
        }

        internal static void UpdateAsyncOperationContextWithActivity(object activity)
        {
            if ((OperationContext.Current != null) && (activity != null))
            {
                OperationContext.Current.OutgoingMessageProperties["AsyncOperationActivity"] = activity;
            }
        }

        internal static AsyncCallback WrapExecuteUserCodeAsyncCallback(AsyncCallback callback)
        {
            if (DiagnosticUtility.ShouldUseActivity && (callback != null))
            {
                return new ExecuteUserCodeAsync(callback).Callback;
            }
            return callback;
        }

        public static bool PropagateUserActivity =>
            (ShouldPropagateActivity && PropagateUserActivityCore);

        private static bool PropagateUserActivityCore =>
            (!DiagnosticUtility.TracingEnabled && (DiagnosticTrace.ActivityId != Guid.Empty));

        internal static bool ShouldPropagateActivity =>
            shouldPropagateActivity;

        private sealed class ExecuteUserCodeAsync
        {
            private AsyncCallback callback;

            public ExecuteUserCodeAsync(AsyncCallback callback)
            {
                this.callback = callback;
            }

            private void ExecuteUserCode(IAsyncResult result)
            {
                using (ServiceModelActivity activity = ServiceModelActivity.CreateBoundedActivity())
                {
                    ServiceModelActivity.Start(activity, System.ServiceModel.SR.GetString("ActivityCallback"), ActivityType.ExecuteUserCode);
                    this.callback(result);
                }
            }

            public AsyncCallback Callback =>
                DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(this.ExecuteUserCode));
        }

        internal class TracingAsyncCallbackState
        {
            private Guid activityId;
            private object innerState;

            internal TracingAsyncCallbackState(object innerState)
            {
                this.innerState = innerState;
                this.activityId = DiagnosticTrace.ActivityId;
            }

            internal Guid ActivityId =>
                this.activityId;

            internal object InnerState =>
                this.innerState;
        }
    }
}

