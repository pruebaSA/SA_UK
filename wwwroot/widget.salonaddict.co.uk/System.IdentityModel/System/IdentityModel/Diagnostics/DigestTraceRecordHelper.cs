namespace System.IdentityModel.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.IdentityModel;
    using System.IO;
    using System.Security.Cryptography;
    using System.ServiceModel.Diagnostics;

    internal static class DigestTraceRecordHelper
    {
        private static bool _initialized;
        private static bool _shouldTraceDigest;
        private const string DigestTrace = "DigestTrace";

        private static void InitializeShouldTraceDigest()
        {
            if (((DiagnosticUtility.DiagnosticTrace != null) && (DiagnosticUtility.DiagnosticTrace.TraceSource != null)) && (DiagnosticUtility.DiagnosticTrace.TraceSource.ShouldLogPii && DiagnosticUtility.ShouldTraceVerbose))
            {
                _shouldTraceDigest = true;
            }
            _initialized = true;
        }

        internal static void TraceDigest(MemoryStream logStream, HashAlgorithm hash)
        {
            if (ShouldTraceDigest)
            {
                TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.IdentityModel, new DigestTraceRecord("DigestTrace", logStream, hash), null, null);
            }
        }

        internal static bool ShouldTraceDigest
        {
            get
            {
                if (!_initialized)
                {
                    InitializeShouldTraceDigest();
                }
                return _shouldTraceDigest;
            }
        }
    }
}

