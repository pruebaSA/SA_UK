namespace Microsoft.InfoCards.Diagnostics
{
    using Microsoft.InfoCards;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.ServiceModel.Diagnostics;
    using System.Threading;

    internal static class InfoCardTrace
    {
        private const string InfoCardEventSource = "CardSpace 3.0.0.0";

        public static void Assert(bool condition, string format, params object[] parameters)
        {
            if (!condition)
            {
                string message = format;
                if ((parameters != null) && (parameters.Length != 0))
                {
                    message = string.Format(CultureInfo.InvariantCulture, format, parameters);
                }
                FailFast(message);
            }
        }

        public static void Audit(EventCode code)
        {
            LogEvent(code, null, EventLogEntryType.Information);
        }

        public static void Audit(EventCode code, string message)
        {
            LogEvent(code, message, EventLogEntryType.Information);
        }

        private static string BuildMessage(InfoCardBaseException ie)
        {
            Exception innerException = ie;
            string str = innerException.Message + "\n";
            if (innerException.InnerException != null)
            {
                while (innerException.InnerException != null)
                {
                    str = str + string.Format(CultureInfo.CurrentUICulture, Microsoft.InfoCards.SR.GetString("InnerExceptionTraceFormat"), new object[] { innerException.InnerException.Message });
                    innerException = innerException.InnerException;
                }
                return (str + string.Format(CultureInfo.CurrentUICulture, Microsoft.InfoCards.SR.GetString("CallStackTraceFormat"), new object[] { ie.ToString() }));
            }
            if (!string.IsNullOrEmpty(Environment.StackTrace))
            {
                str = str + string.Format(CultureInfo.CurrentUICulture, Microsoft.InfoCards.SR.GetString("CallStackTraceFormat"), new object[] { Environment.StackTrace });
            }
            return str;
        }

        public static void CloseInvalidOutSafeHandle(SafeHandle handle)
        {
            Utility.CloseInvalidOutSafeHandle(handle);
        }

        [Conditional("DEBUG")]
        public static void DebugAssert(bool condition, string format, params object[] parameters)
        {
        }

        public static void FailFast(string message)
        {
            DiagnosticUtility.FailFast(message);
        }

        public static Guid GetActivityId() => 
            DiagnosticTrace.ActivityId;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static bool IsFatal(Exception e) => 
            DiagnosticUtility.IsFatal(e);

        private static void LogEvent(EventCode code, string message, EventLogEntryType type)
        {
            using (SafeEventLogHandle handle = SafeEventLogHandle.Construct())
            {
                string str = message;
                if (handle != null)
                {
                    if (string.IsNullOrEmpty(str))
                    {
                        str = Microsoft.InfoCards.SR.GetString("GeneralExceptionMessage");
                    }
                    IntPtr[] ptrArray = new IntPtr[1];
                    GCHandle handle2 = new GCHandle();
                    GCHandle handle3 = new GCHandle();
                    try
                    {
                        handle3 = GCHandle.Alloc(ptrArray, GCHandleType.Pinned);
                        handle2 = GCHandle.Alloc(str, GCHandleType.Pinned);
                        ptrArray[0] = handle2.AddrOfPinnedObject();
                        HandleRef strings = new HandleRef(handle, handle3.AddrOfPinnedObject());
                        SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
                        byte[] binaryForm = new byte[user.BinaryLength];
                        user.GetBinaryForm(binaryForm, 0);
                        if (!ReportEvent(handle, (short) type, 1, (uint) code, binaryForm, 1, 0, strings, null))
                        {
                            Marshal.GetLastWin32Error();
                        }
                    }
                    finally
                    {
                        if (handle3.IsAllocated)
                        {
                            handle3.Free();
                        }
                        if (handle2.IsAllocated)
                        {
                            handle2.Free();
                        }
                    }
                }
            }
        }

        [DllImport("advapi32", EntryPoint="ReportEventW", CharSet=CharSet.Unicode, SetLastError=true, ExactSpelling=true)]
        private static extern bool ReportEvent([In] SafeHandle hEventLog, [In] short type, [In] ushort category, [In] uint eventID, [In] byte[] userSID, [In] short numStrings, [In] int dataLen, [In] HandleRef strings, [In] byte[] rawData);
        public static void SetActivityId(Guid activityId)
        {
            DiagnosticTrace.ActivityId = activityId;
        }

        public static bool ShouldTrace(TraceEventType type) => 
            DiagnosticUtility.ShouldTrace(type);

        public static Exception ThrowHelperArgument(string message) => 
            DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(message);

        public static Exception ThrowHelperArgumentNull(string err) => 
            DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(err);

        public static Exception ThrowHelperArgumentNull(string err, string message) => 
            DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(err, message);

        public static Exception ThrowHelperCritical(Exception e)
        {
            TraceAndLogException(e);
            return DiagnosticUtility.ExceptionUtility.ThrowHelperCritical(e);
        }

        public static Exception ThrowHelperError(Exception e)
        {
            TraceAndLogException(e);
            return DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
        }

        public static Exception ThrowHelperErrorWithNoLogging(Exception e) => 
            DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);

        public static Exception ThrowHelperWarning(Exception e)
        {
            TraceAndLogException(e);
            return DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(e);
        }

        public static void ThrowInvalidArgumentConditional(bool condition, string argument)
        {
            if (condition)
            {
                throw ThrowHelperError(new InfoCardArgumentException(string.Format(CultureInfo.CurrentUICulture, Microsoft.InfoCards.SR.GetString("ServiceInvalidArgument"), new object[] { argument })));
            }
        }

        public static TimerCallback ThunkCallback(TimerCallback callback) => 
            DiagnosticUtility.Utility.ThunkCallback(callback);

        public static WaitCallback ThunkCallback(WaitCallback callback) => 
            DiagnosticUtility.Utility.ThunkCallback(callback);

        [Conditional("DEBUG")]
        public static void Trace(TraceEventType level, TraceCode code)
        {
        }

        [Conditional("DEBUG")]
        public static void Trace(TraceEventType level, TraceCode code, params object[] parameters)
        {
        }

        public static void TraceAndLogException(Exception e)
        {
            bool flag = false;
            bool flag2 = false;
            InfoCardBaseException ie = e as InfoCardBaseException;
            if (((ie != null) && !(ie is UserCancelledException)) && !ie.Logged)
            {
                flag = true;
            }
            if (flag)
            {
                for (Exception exception2 = ie.InnerException; exception2 != null; exception2 = exception2.InnerException)
                {
                    if (exception2 is UserCancelledException)
                    {
                        flag = false;
                        break;
                    }
                    if ((exception2 is InfoCardBaseException) && (exception2 as InfoCardBaseException).Logged)
                    {
                        flag2 = true;
                    }
                }
            }
            if (flag)
            {
                EventLogEntryType type = flag2 ? EventLogEntryType.Information : EventLogEntryType.Error;
                string message = ie.Message;
                if (!flag2)
                {
                    message = BuildMessage(ie);
                }
                LogEvent((EventCode) ie.NativeHResult, message, type);
            }
        }

        [Conditional("DEBUG")]
        public static void TraceCritical(TraceCode code)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceCritical(TraceCode code, params object[] parameters)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceDebug(string message)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceDebug(string format, params object[] parameters)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceError(TraceCode code)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceError(TraceCode code, params object[] parameters)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceException(Exception e)
        {
            Exception innerException = e;
            for (int i = 0; innerException != null; i++)
            {
                innerException = innerException.InnerException;
            }
        }

        [Conditional("DEBUG")]
        public static void TraceInfo(TraceCode code)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceInfo(TraceCode code, params object[] parameters)
        {
        }

        [Conditional("DEBUG")]
        private static void TraceInternal(TraceEventType level, TraceCode code, params object[] parameters)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceVerbose(TraceCode code)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceVerbose(TraceCode code, params object[] parameters)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceWarning(TraceCode code)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceWarning(TraceCode code, params object[] parameters)
        {
        }

        public static bool ShouldTraceCritical =>
            DiagnosticUtility.ShouldTraceCritical;

        public static bool ShouldTraceError =>
            DiagnosticUtility.ShouldTraceError;

        public static bool ShouldTraceInformation =>
            DiagnosticUtility.ShouldTraceInformation;

        public static bool ShouldTraceVerbose =>
            DiagnosticUtility.ShouldTraceVerbose;

        public static bool ShouldTraceWarning =>
            DiagnosticUtility.ShouldTraceWarning;

        internal class SafeEventLogHandle : SafeHandle
        {
            private SafeEventLogHandle() : base(IntPtr.Zero, true)
            {
            }

            public static InfoCardTrace.SafeEventLogHandle Construct()
            {
                InfoCardTrace.SafeEventLogHandle handle = RegisterEventSource(null, "CardSpace 3.0.0.0");
                if ((handle == null) || handle.IsInvalid)
                {
                    Marshal.GetLastWin32Error();
                }
                return handle;
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("advapi32", CharSet=CharSet.Unicode, SetLastError=true, ExactSpelling=true)]
            private static extern bool DeregisterEventSource(IntPtr eventLog);
            [DllImport("advapi32", EntryPoint="RegisterEventSourceW", CharSet=CharSet.Unicode, SetLastError=true, ExactSpelling=true)]
            private static extern InfoCardTrace.SafeEventLogHandle RegisterEventSource(string uncServerName, string sourceName);
            protected override bool ReleaseHandle() => 
                DeregisterEventSource(base.handle);

            public override bool IsInvalid =>
                (IntPtr.Zero == base.handle);
        }
    }
}

