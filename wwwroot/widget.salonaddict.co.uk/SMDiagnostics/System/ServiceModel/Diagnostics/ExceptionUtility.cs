namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;

    internal class ExceptionUtility
    {
        [ThreadStatic]
        private static Guid activityId;
        private DiagnosticTrace diagnosticTrace;
        private string eventSourceName;
        private const string ExceptionStackAsStringKey = "System.ServiceModel.Diagnostics.ExceptionUtility.ExceptionStackAsString";
        private string name;
        [ThreadStatic]
        private static bool useStaticActivityId;

        [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.ExceptionUtility instead")]
        internal ExceptionUtility(string name, string eventSourceName, object diagnosticTrace)
        {
            this.diagnosticTrace = (DiagnosticTrace) diagnosticTrace;
            this.name = name;
            this.eventSourceName = eventSourceName;
        }

        internal static void ClearActivityId()
        {
            useStaticActivityId = false;
            activityId = Guid.Empty;
        }

        internal static bool IsFatal(Exception exception)
        {
            while (exception != null)
            {
                InternalException exception2;
                if ((((exception is FatalException) || (((exception2 = exception as InternalException) != null) && exception2.IsFatal)) || ((exception is OutOfMemoryException) && !(exception is InsufficientMemoryException))) || (((exception is ThreadAbortException) || (exception is AccessViolationException)) || (exception is SEHException)))
                {
                    return true;
                }
                if (!(exception is TypeInitializationException) && !(exception is TargetInvocationException))
                {
                    break;
                }
                exception = exception.InnerException;
            }
            return false;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static bool IsInfrastructureException(Exception exception)
        {
            if (exception == null)
            {
                return false;
            }
            return ((exception is ThreadAbortException) || (exception is AppDomainUnloadedException));
        }

        internal Exception ThrowHelper(Exception exception, TraceEventType eventType) => 
            this.ThrowHelper(exception, eventType, null);

        internal Exception ThrowHelper(Exception exception, TraceEventType eventType, TraceRecord extendedData)
        {
            if (this.diagnosticTrace != null)
            {
                using (useStaticActivityId ? Activity.CreateActivity(activityId) : null)
                {
                    this.diagnosticTrace.TraceEvent(eventType, TraceCode.ThrowingException, TraceSR.GetString("ThrowingException"), extendedData, exception, null);
                }
            }
            string str = ((this.diagnosticTrace != null) && this.diagnosticTrace.ShouldTrace(eventType)) ? exception.StackTrace : null;
            if (!string.IsNullOrEmpty(str))
            {
                IDictionary data = exception.Data;
                if (((data != null) && !data.IsReadOnly) && !data.IsFixedSize)
                {
                    object obj2 = data["System.ServiceModel.Diagnostics.ExceptionUtility.ExceptionStackAsString"];
                    string str2 = (obj2 == null) ? "" : (obj2 as string);
                    if (str2 != null)
                    {
                        str2 = str2 + ((str2.Length == 0) ? "" : Environment.NewLine) + "throw" + Environment.NewLine + str + Environment.NewLine + "catch" + Environment.NewLine;
                        data["System.ServiceModel.Diagnostics.ExceptionUtility.ExceptionStackAsString"] = str2;
                    }
                }
            }
            return exception;
        }

        internal ArgumentException ThrowHelperArgument(string message) => 
            ((ArgumentException) this.ThrowHelperError(new ArgumentException(message)));

        internal ArgumentException ThrowHelperArgument(string paramName, string message) => 
            ((ArgumentException) this.ThrowHelperError(new ArgumentException(message, paramName)));

        internal ArgumentNullException ThrowHelperArgumentNull(string paramName) => 
            ((ArgumentNullException) this.ThrowHelperError(new ArgumentNullException(paramName)));

        internal ArgumentNullException ThrowHelperArgumentNull(string paramName, string message) => 
            ((ArgumentNullException) this.ThrowHelperError(new ArgumentNullException(paramName, message)));

        internal Exception ThrowHelperCallback(Exception innerException) => 
            this.ThrowHelperCallback(TraceSR.GetString("GenericCallbackException"), innerException);

        internal Exception ThrowHelperCallback(string message, Exception innerException) => 
            this.ThrowHelperCritical(new CallbackException(message, innerException));

        internal Exception ThrowHelperCritical(Exception exception) => 
            this.ThrowHelper(exception, TraceEventType.Critical);

        internal Exception ThrowHelperError(Exception exception) => 
            this.ThrowHelper(exception, TraceEventType.Error);

        internal Exception ThrowHelperFatal(string message, Exception innerException) => 
            this.ThrowHelperError(new FatalException(message, innerException));

        internal Exception ThrowHelperInternal(bool fatal) => 
            this.ThrowHelperError(new InternalException(fatal));

        internal Exception ThrowHelperWarning(Exception exception) => 
            this.ThrowHelper(exception, TraceEventType.Warning);

        [MethodImpl(MethodImplOptions.NoInlining), Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.ExceptionUtility instead")]
        internal void TraceFailFast(string message)
        {
            EventLogger logger = null;
            try
            {
                logger = new EventLogger(this.eventSourceName, this.diagnosticTrace);
            }
            finally
            {
                TraceFailFast(message, logger);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining), Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.ExceptionUtility instead")]
        internal static void TraceFailFast(string message, EventLogger logger)
        {
            try
            {
                if (logger != null)
                {
                    string str = null;
                    try
                    {
                        str = new StackTrace().ToString();
                    }
                    catch (Exception exception)
                    {
                        str = exception.Message;
                    }
                    finally
                    {
                        logger.LogEvent(TraceEventType.Critical, EventLogCategory.FailFast, (EventLogEventId) (-1073676186), new string[] { message, str });
                    }
                }
            }
            catch (Exception exception2)
            {
                if (logger != null)
                {
                    logger.LogEvent(TraceEventType.Critical, EventLogCategory.FailFast, (EventLogEventId) (-1073676185), new string[] { exception2.ToString() });
                }
                throw;
            }
        }

        [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.ExceptionUtility instead")]
        internal void TraceFailFastException(Exception exception)
        {
            this.TraceFailFast(exception?.ToString());
        }

        internal void TraceHandledException(Exception exception, TraceEventType eventType)
        {
            if (this.diagnosticTrace != null)
            {
                using (useStaticActivityId ? Activity.CreateActivity(activityId) : null)
                {
                    this.diagnosticTrace.TraceEvent(eventType, TraceCode.TraceHandledException, TraceSR.GetString("TraceHandledException"), null, exception, null);
                }
            }
        }

        internal static void UseActivityId(Guid activityId)
        {
            ExceptionUtility.activityId = activityId;
            useStaticActivityId = true;
        }

        [Serializable]
        private class InternalException : SystemException
        {
            private bool fatal;

            public InternalException()
            {
            }

            public InternalException(bool fatal) : base(TraceSR.GetString("InternalException"))
            {
                this.fatal = fatal;
            }

            protected InternalException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public bool IsFatal =>
                this.fatal;
        }
    }
}

