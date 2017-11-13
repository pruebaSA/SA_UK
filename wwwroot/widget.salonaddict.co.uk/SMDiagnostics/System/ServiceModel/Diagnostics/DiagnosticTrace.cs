namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Xml;

    internal class DiagnosticTrace
    {
        private string AppDomainFriendlyName;
        private bool calledShutdown;
        private static object classLockObject = new object();
        private const SourceLevels DefaultLevel = SourceLevels.Off;
        private const string DefaultTraceListenerName = "Default";
        [SecurityCritical]
        private string eventSourceName = string.Empty;
        private bool haveListeners;
        private DateTime lastFailure = DateTime.MinValue;
        private SourceLevels level;
        private object localSyncObject = new object();
        private const int MaxTraceSize = 0xffff;
        private bool shouldUseActivity;
        private const string subType = "";
        private static SortedList<TraceCode, string> traceCodes = new SortedList<TraceCode, string>();
        private const int traceFailureLogThreshold = 1;
        private const string TraceRecordVersion = "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord";
        private PiiTraceSource traceSource;
        private string TraceSourceName = string.Empty;
        private TraceSourceKind traceSourceType = TraceSourceKind.PiiTraceSource;
        private bool tracingEnabled = true;
        private const string version = "1";

        [Obsolete("For SMDiagnostics.dll use only. Never 'new' this type up unless you are DiagnosticUtility."), SecurityCritical]
        internal DiagnosticTrace(TraceSourceKind sourceType, string traceSourceName, string eventSourceName)
        {
            this.traceSourceType = sourceType;
            this.TraceSourceName = traceSourceName;
            this.eventSourceName = eventSourceName;
            this.AppDomainFriendlyName = AppDomain.CurrentDomain.FriendlyName;
            try
            {
                this.CreateTraceSource();
                this.UnsafeAddDomainEventHandlersForCleanup();
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (ExceptionUtility.IsFatal(exception))
                {
                    throw;
                }
                new EventLogger(this.eventSourceName, null).LogEvent(TraceEventType.Error, EventLogCategory.Tracing, (EventLogEventId) (-1073676188), false, new string[] { exception.ToString() });
            }
        }

        private void AddExceptionToTraceString(XmlWriter xml, Exception exception)
        {
            xml.WriteElementString("ExceptionType", XmlEncode(exception.GetType().AssemblyQualifiedName));
            xml.WriteElementString("Message", XmlEncode(exception.Message));
            xml.WriteElementString("StackTrace", XmlEncode(this.StackTraceString(exception)));
            xml.WriteElementString("ExceptionString", XmlEncode(exception.ToString()));
            Win32Exception exception2 = exception as Win32Exception;
            if (exception2 != null)
            {
                xml.WriteElementString("NativeErrorCode", exception2.NativeErrorCode.ToString("X", CultureInfo.InvariantCulture));
            }
            if ((exception.Data != null) && (exception.Data.Count > 0))
            {
                xml.WriteStartElement("DataItems");
                foreach (object obj2 in exception.Data.Keys)
                {
                    xml.WriteStartElement("Data");
                    xml.WriteElementString("Key", XmlEncode(obj2.ToString()));
                    xml.WriteElementString("Value", XmlEncode(exception.Data[obj2].ToString()));
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }
            if (exception.InnerException != null)
            {
                xml.WriteStartElement("InnerException");
                this.AddExceptionToTraceString(xml, exception.InnerException);
                xml.WriteEndElement();
            }
        }

        private void BuildTrace(TraceEventType type, TraceCode code, string description, TraceRecord trace, Exception exception, object source, out TraceXPathNavigator navigator)
        {
            PlainXmlWriter xml = new PlainXmlWriter(0xffff);
            navigator = xml.Navigator;
            this.BuildTrace(xml, type, code, description, trace, exception, source);
            if (!this.TraceSource.ShouldLogPii)
            {
                navigator.RemovePii(DiagnosticStrings.HeadersPaths);
            }
        }

        private void BuildTrace(PlainXmlWriter xml, TraceEventType type, TraceCode code, string description, TraceRecord trace, Exception exception, object source)
        {
            xml.WriteStartElement("TraceRecord");
            xml.WriteAttributeString("xmlns", "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord");
            xml.WriteAttributeString("Severity", LookupSeverity(type));
            xml.WriteElementString("TraceIdentifier", GenerateTraceCode(code));
            xml.WriteElementString("Description", description);
            xml.WriteElementString("AppDomain", this.AppDomainFriendlyName);
            if (source != null)
            {
                xml.WriteElementString("Source", CreateSourceString(source));
            }
            if (trace != null)
            {
                xml.WriteStartElement("ExtendedData");
                xml.WriteAttributeString("xmlns", trace.EventId);
                trace.WriteTo(xml);
                xml.WriteEndElement();
            }
            if (exception != null)
            {
                xml.WriteStartElement("Exception");
                this.AddExceptionToTraceString(xml, exception);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }

        internal static string CodeToString(TraceCode code)
        {
            string str = null;
            if (!traceCodes.TryGetValue(code, out str))
            {
                lock (classLockObject)
                {
                    if (!traceCodes.TryGetValue(code, out str))
                    {
                        str = code.ToString();
                        traceCodes.Add(code, str);
                    }
                }
            }
            return str;
        }

        internal static string CreateSourceString(object source) => 
            (source.GetType().ToString() + "/" + source.GetHashCode().ToString(CultureInfo.CurrentCulture));

        [SecurityCritical, SecurityTreatAsSafe]
        private void CreateTraceSource()
        {
            PiiTraceSource piiTraceSource = null;
            if (this.traceSourceType == TraceSourceKind.PiiTraceSource)
            {
                piiTraceSource = new PiiTraceSource(this.TraceSourceName, this.eventSourceName, SourceLevels.Off);
            }
            else
            {
                piiTraceSource = new DiagnosticTraceSource(this.TraceSourceName, this.eventSourceName, SourceLevels.Off);
            }
            this.UnsafeRemoveDefaultTraceListener(piiTraceSource);
            this.TraceSource = piiTraceSource;
        }

        private void ExitOrUnloadEventHandler(object sender, EventArgs e)
        {
            this.ShutdownTracing();
        }

        private SourceLevels FixLevel(SourceLevels level)
        {
            if (((level & ~SourceLevels.Information) & SourceLevels.Verbose) != SourceLevels.Off)
            {
                level |= SourceLevels.Verbose;
            }
            else if (((level & ~SourceLevels.Warning) & SourceLevels.Information) != SourceLevels.Off)
            {
                level |= SourceLevels.Information;
            }
            else if (((level & ~SourceLevels.Error) & SourceLevels.Warning) != SourceLevels.Off)
            {
                level |= SourceLevels.Warning;
            }
            if (((level & ~SourceLevels.Critical) & SourceLevels.Error) != SourceLevels.Off)
            {
                level |= SourceLevels.Error;
            }
            if ((level & SourceLevels.Critical) != SourceLevels.Off)
            {
                level |= SourceLevels.Critical;
            }
            if (level == SourceLevels.ActivityTracing)
            {
                level = SourceLevels.Off;
            }
            return level;
        }

        internal static string GenerateTraceCode(TraceCode code)
        {
            TraceCode code2 = (TraceCode) ((int) (((long) code) & 0xffff0000L));
            string str = null;
            switch (code2)
            {
                case TraceCode.Channels:
                    str = "System.ServiceModel.Channels";
                    break;

                case TraceCode.ComIntegration:
                    str = "System.ServiceModel.ComIntegration";
                    break;

                case TraceCode.Security:
                    str = "System.ServiceModel.Security";
                    break;

                case TraceCode.Administration:
                    str = "System.ServiceModel.Administration";
                    break;

                case TraceCode.Diagnostics:
                    str = "System.ServiceModel.Diagnostics";
                    break;

                case TraceCode.Serialization:
                    str = "System.Runtime.Serialization";
                    break;

                case TraceCode.ServiceModel:
                case TraceCode.ServiceModelTransaction:
                    str = "System.ServiceModel";
                    break;

                case TraceCode.Activation:
                    str = "System.ServiceModel.Activation";
                    break;

                case TraceCode.PortSharing:
                    str = "System.ServiceModel.PortSharing";
                    break;

                case TraceCode.IdentityModelSelectors:
                    str = "System.IdentityModel.Selectors";
                    break;

                case TraceCode.TransactionBridge:
                    str = "Microsoft.Transactions.TransactionBridge";
                    break;

                case TraceCode.IdentityModel:
                    str = "System.IdentityModel";
                    break;

                default:
                    str = string.Empty;
                    break;
            }
            return string.Format(CultureInfo.InvariantCulture, "http://msdn.microsoft.com/{0}/library/{1}.{2}.aspx", new object[] { CultureInfo.CurrentCulture.Name, str, CodeToString(code) });
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private void LogTraceFailure(string traceString, Exception e)
        {
            TimeSpan span = TimeSpan.FromMinutes(10.0);
            try
            {
                lock (this.localSyncObject)
                {
                    if (DateTime.UtcNow.Subtract(this.LastFailure) >= span)
                    {
                        this.LastFailure = DateTime.UtcNow;
                        EventLogger logger = EventLogger.UnsafeCreateEventLogger(this.eventSourceName, this);
                        if (e == null)
                        {
                            logger.UnsafeLogEvent(TraceEventType.Error, EventLogCategory.Tracing, (EventLogEventId) (-1073676184), false, new string[] { traceString });
                        }
                        else
                        {
                            logger.UnsafeLogEvent(TraceEventType.Error, EventLogCategory.Tracing, (EventLogEventId) (-1073676183), false, new string[] { traceString, e.ToString() });
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private static string LookupSeverity(TraceEventType type)
        {
            switch (type)
            {
                case TraceEventType.Critical:
                    return "Critical";

                case TraceEventType.Error:
                    return "Error";

                case TraceEventType.Warning:
                    return "Warning";

                case TraceEventType.Information:
                    return "Information";

                case TraceEventType.Verbose:
                    return "Verbose";

                case TraceEventType.Suspend:
                    return "Suspend";

                case TraceEventType.Transfer:
                    return "Transfer";

                case TraceEventType.Start:
                    return "Start";

                case TraceEventType.Stop:
                    return "Stop";
            }
            return type.ToString();
        }

        private void SetLevel(SourceLevels level)
        {
            SourceLevels levels = this.FixLevel(level);
            this.level = levels;
            if (this.TraceSource != null)
            {
                this.haveListeners = this.TraceSource.Listeners.Count > 0;
                if ((this.TraceSource.Switch.Level != SourceLevels.Off) && (level == SourceLevels.Off))
                {
                    System.Diagnostics.TraceSource traceSource = this.TraceSource;
                    this.CreateTraceSource();
                    traceSource.Close();
                }
                this.tracingEnabled = this.HaveListeners && (levels != SourceLevels.Off);
                this.TraceSource.Switch.Level = levels;
                this.shouldUseActivity = (levels & SourceLevels.ActivityTracing) != SourceLevels.Off;
            }
        }

        private void SetLevelThreadSafe(SourceLevels level)
        {
            lock (this.localSyncObject)
            {
                this.SetLevel(level);
            }
        }

        [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.ShouldTrace instead")]
        internal bool ShouldTrace(TraceEventType type) => 
            ((this.TracingEnabled && (this.TraceSource != null)) && (((TraceEventType) 0) != (type & ((TraceEventType) ((int) this.Level)))));

        private void ShutdownTracing()
        {
            if ((this.TraceSource != null) && !this.calledShutdown)
            {
                try
                {
                    if (this.Level != SourceLevels.Off)
                    {
                        if (this.ShouldTrace(TraceEventType.Information))
                        {
                            Dictionary<string, string> dictionary = new Dictionary<string, string>(3) {
                                ["AppDomain.FriendlyName"] = AppDomain.CurrentDomain.FriendlyName,
                                ["ProcessName"] = ProcessName,
                                ["ProcessId"] = ProcessId.ToString(CultureInfo.CurrentCulture)
                            };
                            this.TraceEvent(TraceEventType.Information, TraceCode.AppDomainUnload, TraceSR.GetString("TraceCodeAppDomainUnload"), new DictionaryTraceRecord(dictionary), null, null);
                        }
                        this.calledShutdown = true;
                        this.TraceSource.Flush();
                    }
                }
                catch (Exception exception)
                {
                    if (ExceptionUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    this.LogTraceFailure(null, exception);
                }
            }
        }

        private string StackTraceString(Exception exception)
        {
            string stackTrace = exception.StackTrace;
            if (!string.IsNullOrEmpty(stackTrace))
            {
                return stackTrace;
            }
            StackFrame[] frames = new StackTrace(false).GetFrames();
            int skipFrames = 0;
            bool flag = false;
            foreach (StackFrame frame in frames)
            {
                string str3;
                string name = frame.GetMethod().Name;
                if (((str3 = name) != null) && (((str3 == "StackTraceString") || (str3 == "AddExceptionToTraceString")) || (((str3 == "BuildTrace") || (str3 == "TraceEvent")) || (str3 == "TraceException"))))
                {
                    skipFrames++;
                }
                else if (name.StartsWith("ThrowHelper", StringComparison.Ordinal))
                {
                    skipFrames++;
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    break;
                }
            }
            StackTrace trace = new StackTrace(skipFrames, false);
            return trace.ToString();
        }

        internal void TraceEvent(TraceEventType type, TraceCode code, string description)
        {
            this.TraceEvent(type, code, description, null, null, null);
        }

        internal void TraceEvent(TraceEventType type, TraceCode code, string description, TraceRecord trace)
        {
            this.TraceEvent(type, code, description, trace, null, null);
        }

        internal void TraceEvent(TraceEventType type, TraceCode code, string description, TraceRecord trace, Exception exception)
        {
            this.TraceEvent(type, code, description, trace, exception, null);
        }

        internal void TraceEvent(TraceEventType type, TraceCode code, string description, TraceRecord trace, Exception exception, object source)
        {
            TraceXPathNavigator navigator = null;
            try
            {
                if ((this.TraceSource != null) && this.HaveListeners)
                {
                    try
                    {
                        this.BuildTrace(type, code, description, trace, exception, source, out navigator);
                    }
                    catch (PlainXmlWriter.MaxSizeExceededException)
                    {
                        StringTraceRecord record = new StringTraceRecord("TruncatedTraceId", GenerateTraceCode(code));
                        this.TraceEvent(type, TraceCode.TraceTruncatedQuotaExceeded, TraceSR.GetString("TraceCodeTraceTruncatedQuotaExceeded"), record);
                    }
                    this.TraceSource.TraceData(type, (int) code, navigator);
                    if (this.calledShutdown)
                    {
                        this.TraceSource.Flush();
                    }
                    this.LastFailure = DateTime.MinValue;
                }
            }
            catch (Exception exception2)
            {
                if (ExceptionUtility.IsFatal(exception2))
                {
                    throw;
                }
                this.LogTraceFailure((navigator == null) ? string.Empty : navigator.ToString(), exception2);
            }
        }

        internal void TraceEvent(TraceEventType type, TraceCode code, string description, TraceRecord trace, Exception exception, Guid activityId, object source)
        {
            using ((this.ShouldUseActivity && (Guid.Empty == activityId)) ? null : Activity.CreateActivity(activityId))
            {
                this.TraceEvent(type, code, description, trace, exception, source);
            }
        }

        internal void TraceTransfer(Guid newId)
        {
            if (this.ShouldUseActivity)
            {
                Guid activityId = ActivityId;
                if ((newId != activityId) && this.HaveListeners)
                {
                    try
                    {
                        this.TraceSource.TraceTransfer(0, null, newId);
                    }
                    catch (Exception exception)
                    {
                        if (ExceptionUtility.IsFatal(exception))
                        {
                            throw;
                        }
                        this.LogTraceFailure(null, exception);
                    }
                }
            }
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception exceptionObject = (Exception) args.ExceptionObject;
            this.TraceEvent(TraceEventType.Critical, TraceCode.UnhandledException, TraceSR.GetString("UnhandledException"), null, exceptionObject, null);
            this.ShutdownTracing();
        }

        [Obsolete("For SMDiagnostics.dll use only"), SecurityCritical, SecurityPermission(SecurityAction.Assert, UnmanagedCode=true)]
        private void UnsafeAddDomainEventHandlersForCleanup()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            this.haveListeners = this.TraceSource.Listeners.Count > 0;
            this.tracingEnabled = this.HaveListeners;
            if (this.TracingEnabled)
            {
                currentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.UnhandledExceptionHandler);
                this.SetLevel(this.TraceSource.Switch.Level);
                currentDomain.DomainUnload += new EventHandler(this.ExitOrUnloadEventHandler);
                currentDomain.ProcessExit += new EventHandler(this.ExitOrUnloadEventHandler);
            }
        }

        [SecurityCritical, SecurityPermission(SecurityAction.Assert, UnmanagedCode=true)]
        private void UnsafeRemoveDefaultTraceListener(PiiTraceSource piiTraceSource)
        {
            piiTraceSource.Listeners.Remove("Default");
        }

        internal static string XmlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            int length = text.Length;
            StringBuilder builder = new StringBuilder(length + 8);
            for (int i = 0; i < length; i++)
            {
                char ch = text[i];
                switch (ch)
                {
                    case '<':
                    {
                        builder.Append("&lt;");
                        continue;
                    }
                    case '>':
                    {
                        builder.Append("&gt;");
                        continue;
                    }
                    case '&':
                    {
                        builder.Append("&amp;");
                        continue;
                    }
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

        internal static Guid ActivityId
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                object activityId = Trace.CorrelationManager.ActivityId;
                if (activityId != null)
                {
                    return (Guid) activityId;
                }
                return Guid.Empty;
            }
            [SecurityCritical, SecurityTreatAsSafe]
            set
            {
                Trace.CorrelationManager.ActivityId = value;
            }
        }

        [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.HaveListeners instead")]
        internal bool HaveListeners =>
            this.haveListeners;

        private DateTime LastFailure
        {
            get => 
                this.lastFailure;
            set
            {
                this.lastFailure = value;
            }
        }

        [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.Level instead")]
        internal SourceLevels Level
        {
            get
            {
                if ((this.TraceSource != null) && (this.TraceSource.Switch.Level != this.level))
                {
                    this.level = this.TraceSource.Switch.Level;
                }
                return this.level;
            }
            set
            {
                this.SetLevelThreadSafe(value);
            }
        }

        internal static int ProcessId
        {
            get
            {
                using (Process process = Process.GetCurrentProcess())
                {
                    return process.Id;
                }
            }
        }

        internal static string ProcessName
        {
            get
            {
                using (Process process = Process.GetCurrentProcess())
                {
                    return process.ProcessName;
                }
            }
        }

        [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.ShouldUseActivity instead")]
        internal bool ShouldUseActivity =>
            this.shouldUseActivity;

        internal PiiTraceSource TraceSource
        {
            get => 
                this.traceSource;
            set
            {
                this.traceSource = value;
            }
        }

        [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.TracingEnabled instead")]
        internal bool TracingEnabled =>
            (this.tracingEnabled && (this.traceSource != null));
    }
}

