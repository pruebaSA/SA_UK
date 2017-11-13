namespace System.Runtime.Serialization
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Security;
    using System.ServiceModel.Diagnostics;

    internal static class DiagnosticUtility
    {
        internal const string DefaultTraceListenerName = "Default";
        private static System.ServiceModel.Diagnostics.DiagnosticTrace diagnosticTrace = InitializeTracing();
        internal const string EventSourceName = "System.Runtime.Serialization 3.0.0.0";
        private static System.ServiceModel.Diagnostics.ExceptionUtility exceptionUtility = null;
        private static SourceLevels level = SourceLevels.Off;
        private static object lockObject = new object();
        private static bool shouldTraceCritical = false;
        private static bool shouldTraceError = false;
        private static bool shouldTraceInformation = false;
        private static bool shouldTraceVerbose = false;
        private static bool shouldTraceWarning = false;
        private static bool shouldUseActivity = false;
        private const string TraceSourceName = "System.Runtime.Serialization";
        private static bool tracingEnabled = false;

        [MethodImpl(MethodImplOptions.NoInlining), Conditional("DEBUG")]
        internal static void DebugAssert(string message)
        {
            AssertUtility.DebugAssertCore(message);
        }

        [Conditional("DEBUG")]
        internal static void DebugAssert(bool condition, string message)
        {
        }

        private static System.ServiceModel.Diagnostics.ExceptionUtility GetExceptionUtility()
        {
            lock (lockObject)
            {
                if (exceptionUtility == null)
                {
                    exceptionUtility = new System.ServiceModel.Diagnostics.ExceptionUtility("System.Runtime.Serialization", "System.Runtime.Serialization 3.0.0.0", diagnosticTrace);
                }
            }
            return exceptionUtility;
        }

        [MethodImpl(MethodImplOptions.NoInlining), SecurityCritical, SecurityTreatAsSafe]
        internal static void InitDiagnosticTraceImpl(TraceSourceKind sourceType, string traceSourceName)
        {
            diagnosticTrace = new System.ServiceModel.Diagnostics.DiagnosticTrace(sourceType, traceSourceName, "System.Runtime.Serialization 3.0.0.0");
            UpdateLevel();
        }

        private static System.ServiceModel.Diagnostics.DiagnosticTrace InitializeTracing()
        {
            InitDiagnosticTraceImpl(TraceSourceKind.PiiTraceSource, "System.Runtime.Serialization");
            if (!diagnosticTrace.HaveListeners)
            {
                diagnosticTrace = null;
            }
            return diagnosticTrace;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static bool IsFatal(Exception exception) => 
            System.ServiceModel.Diagnostics.ExceptionUtility.IsFatal(exception);

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

        internal static System.ServiceModel.Diagnostics.ExceptionUtility ExceptionUtility =>
            (exceptionUtility ?? GetExceptionUtility());

        internal static bool ShouldTraceError =>
            shouldTraceError;

        internal static bool ShouldTraceInformation =>
            shouldTraceInformation;

        internal static bool ShouldTraceVerbose =>
            shouldTraceVerbose;

        internal static bool ShouldTraceWarning =>
            shouldTraceWarning;
    }
}

