namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class ComPlusTLBImportTrace
    {
        public static void Trace(TraceEventType type, TraceCode code, string description, Guid iid, Guid typeLibraryID)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                ComPlusTLBImportSchema trace = new ComPlusTLBImportSchema(iid, typeLibraryID);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }

        public static void Trace(TraceEventType type, TraceCode code, string description, Guid iid, Guid typeLibraryID, string assembly)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                ComPlusTLBImportFromAssemblySchema trace = new ComPlusTLBImportFromAssemblySchema(iid, typeLibraryID, assembly);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }

        public static void Trace(TraceEventType type, TraceCode code, string description, Guid iid, Guid typeLibraryID, ImporterEventKind eventKind, int eventCode, string eventMsg)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                ComPlusTLBImportConverterEventSchema trace = new ComPlusTLBImportConverterEventSchema(iid, typeLibraryID, eventKind, eventCode, eventMsg);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }
    }
}

