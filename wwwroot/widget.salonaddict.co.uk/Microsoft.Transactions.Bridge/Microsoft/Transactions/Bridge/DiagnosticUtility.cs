namespace Microsoft.Transactions.Bridge
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class DiagnosticUtility
    {
        private static bool bridgeTracingInitialized;
        internal const string DefaultTraceListenerName = "Default";
        private static System.ServiceModel.Diagnostics.DiagnosticTrace diagnosticTrace = InitializeTracing();
        private static WsatEtwTraceListener etwListener;
        internal const string EventSourceName = "Microsoft.Transactions.Bridge 3.0.0.0";
        private static System.ServiceModel.Diagnostics.ExceptionUtility exceptionUtility = null;
        private static SourceLevels level = SourceLevels.Off;
        private static object lockObject = new object();
        private static bool serviceModelTracingInitialized;
        private static bool shouldTraceCritical = false;
        private static bool shouldTraceError = false;
        private static bool shouldTraceInformation = false;
        private static bool shouldTraceVerbose = false;
        private static bool shouldTraceWarning = false;
        private static bool shouldUseActivity = false;
        private static object syncRoot = new object();
        private const string TraceSourceName = "Microsoft.Transactions.Bridge";
        private static bool tracingEnabled = false;
        private static System.ServiceModel.Diagnostics.Utility utility = null;

        [MethodImpl(MethodImplOptions.NoInlining), Conditional("DEBUG")]
        internal static void DebugAssert(string message)
        {
            AssertUtility.DebugAssertCore(message);
        }

        [Conditional("DEBUG")]
        internal static void DebugAssert(bool condition, string message)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Exception FailFast(string message)
        {
            try
            {
                try
                {
                    ExceptionUtility.TraceFailFast(message);
                }
                finally
                {
                    Environment.FailFast(message);
                }
            }
            catch
            {
            }
            Environment.FailFast(message);
            return null;
        }

        private static System.ServiceModel.Diagnostics.ExceptionUtility GetExceptionUtility()
        {
            lock (lockObject)
            {
                if (exceptionUtility == null)
                {
                    exceptionUtility = new System.ServiceModel.Diagnostics.ExceptionUtility("Microsoft.Transactions.Bridge", "Microsoft.Transactions.Bridge 3.0.0.0", diagnosticTrace);
                }
            }
            return exceptionUtility;
        }

        private static System.ServiceModel.Diagnostics.Utility GetUtility()
        {
            lock (lockObject)
            {
                if (utility == null)
                {
                    utility = new System.ServiceModel.Diagnostics.Utility(ExceptionUtility);
                }
            }
            return utility;
        }

        [MethodImpl(MethodImplOptions.NoInlining), SecurityTreatAsSafe, SecurityCritical]
        internal static void InitDiagnosticTraceImpl(TraceSourceKind sourceType, string traceSourceName)
        {
            diagnosticTrace = new System.ServiceModel.Diagnostics.DiagnosticTrace(sourceType, traceSourceName, "Microsoft.Transactions.Bridge 3.0.0.0");
            UpdateLevel();
        }

        internal static void InitializeServiceModelSource(SourceLevels level, bool propagateActivity, bool tracePii)
        {
            lock (syncRoot)
            {
                if (!serviceModelTracingInitialized)
                {
                    serviceModelTracingInitialized = true;
                    System.ServiceModel.DiagnosticUtility.InitDiagnosticTraceImpl(TraceSourceKind.DiagnosticTraceSource, "System.ServiceModel");
                    ((DiagnosticTraceSource) System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceSource).PropagateActivity = propagateActivity;
                    System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceSource.ShouldLogPii = tracePii;
                    System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceSource.Listeners.Add(EtwListener);
                }
            }
        }

        private static System.ServiceModel.Diagnostics.DiagnosticTrace InitializeTracing()
        {
            InitDiagnosticTraceImpl(TraceSourceKind.PiiTraceSource, "Microsoft.Transactions.Bridge");
            if (!diagnosticTrace.HaveListeners)
            {
                diagnosticTrace = null;
            }
            return diagnosticTrace;
        }

        internal static void InitializeTransactionSource(SourceLevels level)
        {
            lock (syncRoot)
            {
                if (!bridgeTracingInitialized)
                {
                    bridgeTracingInitialized = true;
                    InitDiagnosticTraceImpl(TraceSourceKind.PiiTraceSource, "Microsoft.Transactions.Bridge");
                    DiagnosticTrace.TraceSource.Listeners.Add(EtwListener);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Exception InvokeFinalHandler(Exception exception)
        {
            try
            {
                try
                {
                    ExceptionUtility.TraceFailFastException(exception);
                }
                finally
                {
                    Environment.FailFast(null);
                }
            }
            catch
            {
            }
            Environment.FailFast(null);
            return null;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static bool IsFatal(Exception exception) => 
            System.ServiceModel.Diagnostics.ExceptionUtility.IsFatal(exception);

        internal static bool ShouldTrace(TraceEventType type)
        {
            bool flag = false;
            if (TracingEnabled)
            {
                switch (type)
                {
                    case TraceEventType.Critical:
                        return ShouldTraceCritical;

                    case TraceEventType.Error:
                        return ShouldTraceError;

                    case (TraceEventType.Error | TraceEventType.Critical):
                        return flag;

                    case TraceEventType.Warning:
                        return ShouldTraceWarning;

                    case TraceEventType.Information:
                        return ShouldTraceInformation;

                    case TraceEventType.Verbose:
                        return ShouldTraceVerbose;
                }
            }
            return flag;
        }

        internal static AsyncCallback ThunkAsyncCallback(AsyncCallback callback) => 
            Utility.ThunkCallback(callback);

        private static void UpdateLevel()
        {
            level = DiagnosticTrace.Level;
            tracingEnabled = DiagnosticTrace.TracingEnabled;
            shouldTraceCritical = DiagnosticTrace.ShouldTrace(TraceEventType.Critical);
            shouldTraceError = DiagnosticTrace.ShouldTrace(TraceEventType.Error);
            shouldTraceInformation = DiagnosticTrace.ShouldTrace(TraceEventType.Information);
            shouldTraceWarning = DiagnosticTrace.ShouldTrace(TraceEventType.Warning);
            shouldTraceVerbose = DiagnosticTrace.ShouldTrace(TraceEventType.Verbose);
            shouldUseActivity = DiagnosticTrace.ShouldUseActivity;
        }

        internal static System.ServiceModel.Diagnostics.DiagnosticTrace DiagnosticTrace =>
            diagnosticTrace;

        private static WsatEtwTraceListener EtwListener
        {
            get
            {
                if (etwListener == null)
                {
                    lock (syncRoot)
                    {
                        if (etwListener == null)
                        {
                            etwListener = new WsatEtwTraceListener();
                        }
                    }
                }
                return etwListener;
            }
        }

        internal static EventLogger EventLog =>
            new EventLogger("Microsoft.Transactions.Bridge 3.0.0.0", diagnosticTrace);

        internal static System.ServiceModel.Diagnostics.ExceptionUtility ExceptionUtility =>
            (exceptionUtility ?? GetExceptionUtility());

        internal static SourceLevels Level
        {
            get => 
                level;
            set
            {
                if (diagnosticTrace != null)
                {
                    DiagnosticTrace.Level = value;
                    UpdateLevel();
                }
            }
        }

        internal static bool ShouldTraceCritical =>
            shouldTraceCritical;

        internal static bool ShouldTraceError =>
            shouldTraceError;

        internal static bool ShouldTraceInformation =>
            shouldTraceInformation;

        internal static bool ShouldTraceVerbose =>
            shouldTraceVerbose;

        internal static bool ShouldTraceWarning =>
            shouldTraceWarning;

        internal static bool ShouldUseActivity =>
            shouldUseActivity;

        internal static bool TracingEnabled =>
            tracingEnabled;

        internal static System.ServiceModel.Diagnostics.Utility Utility =>
            (utility ?? GetUtility());
    }
}

