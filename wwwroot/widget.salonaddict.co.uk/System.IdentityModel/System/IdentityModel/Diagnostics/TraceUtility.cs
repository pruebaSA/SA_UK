namespace System.IdentityModel.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.IdentityModel;
    using System.Runtime.CompilerServices;
    using System.ServiceModel.Diagnostics;

    internal static class TraceUtility
    {
        private static string Description(TraceCode traceCode) => 
            System.IdentityModel.SR.GetString("TraceCode" + DiagnosticTrace.CodeToString(traceCode));

        internal static void TraceEvent(TraceEventType severity, TraceCode traceCode, object source, Exception exception)
        {
            TraceEvent(severity, traceCode, null, source, exception);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void TraceEvent(TraceEventType severity, TraceCode traceCode, TraceRecord extendedData, object source, Exception exception)
        {
            if (DiagnosticUtility.ShouldTrace(severity))
            {
                Guid activityId = DiagnosticTrace.ActivityId;
                DiagnosticUtility.DiagnosticTrace.TraceEvent(severity, traceCode, Description(traceCode), extendedData, exception, activityId, source);
            }
        }
    }
}

