namespace System.Deployment.Application
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Deployment.Application.Manifest;
    using System.Deployment.Internal.Isolation;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading;

    internal class Logger
    {
        protected ErrorSection _errors = new ErrorSection();
        protected static object _header = null;
        protected IdentitySection _identities = new IdentitySection();
        protected static object _logAccessLock = new object();
        protected static object _logFileEncoding;
        protected LogFileLocation _logFileLocation;
        protected string _logFilePath;
        protected static Hashtable _loggerCollection = new Hashtable();
        protected LogIdentity _logIdentity = new LogIdentity();
        protected PhaseSection _phases = new PhaseSection();
        protected SourceSection _sources = new SourceSection();
        protected SummarySection _summary = new SummarySection();
        protected static Hashtable _threadLogIdTable = new Hashtable();
        protected TransactionSection _transactions = new TransactionSection();
        protected string _urlName;
        protected WarningSection _warnings = new WarningSection();

        protected Logger()
        {
        }

        protected static void AddCurrentThreadLogger(Logger logger)
        {
            lock (_logAccessLock)
            {
                if (_threadLogIdTable.Contains(logger.Identity.ThreadId))
                {
                    _threadLogIdTable.Remove(logger.Identity.ThreadId);
                }
                _threadLogIdTable.Add(logger.Identity.ThreadId, logger.Identity);
                if (!_loggerCollection.Contains(logger.Identity.ToString()))
                {
                    _loggerCollection.Add(logger.Identity.ToString(), logger);
                }
            }
        }

        internal static void AddErrorInformation(string message, Exception exception)
        {
            AddErrorInformation(message, exception, DateTime.Now);
        }

        internal static void AddErrorInformation(LogIdentity log, string message, Exception exception)
        {
            AddErrorInformation(log, message, exception, DateTime.Now);
        }

        internal static void AddErrorInformation(Exception exception, string messageFormat, params object[] args)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat(messageFormat, args);
                AddErrorInformation(builder.ToString(), exception, DateTime.Now);
            }
            catch (FormatException)
            {
            }
        }

        internal static void AddErrorInformation(string message, Exception exception, DateTime time)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    currentThreadLogger.Errors.AddError(message, exception, time);
                }
            }
        }

        internal static void AddErrorInformation(LogIdentity log, string message, Exception exception, DateTime time)
        {
            Logger logger = GetLogger(log);
            if (logger != null)
            {
                lock (logger)
                {
                    logger.Errors.AddError(message, exception, time);
                }
            }
        }

        protected static void AddLogger(Logger logger)
        {
            lock (_logAccessLock)
            {
                if (!_loggerCollection.Contains(logger.Identity.ToString()))
                {
                    _loggerCollection.Add(logger.Identity.ToString(), logger);
                }
            }
        }

        internal static void AddPhaseInformation(string message)
        {
            AddPhaseInformation(message, DateTime.Now);
        }

        internal static void AddPhaseInformation(string message, DateTime time)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    currentThreadLogger.Phases.AddPhaseInformation(message, time);
                }
            }
        }

        internal static void AddPhaseInformation(string messageFormat, params object[] args)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat(messageFormat, args);
                AddPhaseInformation(builder.ToString(), DateTime.Now);
            }
            catch (FormatException)
            {
            }
        }

        internal static void AddTransactionInformation(System.Deployment.Internal.Isolation.StoreTransactionOperation[] storeOperations, uint[] rgDispositions, int[] rgResults)
        {
            AddTransactionInformation(storeOperations, rgDispositions, rgResults, DateTime.Now);
        }

        internal static void AddTransactionInformation(System.Deployment.Internal.Isolation.StoreTransactionOperation[] storeOperations, uint[] rgDispositions, int[] rgResults, DateTime time)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    currentThreadLogger.Transactions.AddTransactionInformation(storeOperations, rgDispositions, rgResults, time);
                }
            }
        }

        internal static void AddWarningInformation(string message)
        {
            AddWarningInformation(message, DateTime.Now);
        }

        internal static void AddWarningInformation(string message, DateTime time)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    currentThreadLogger.Warnings.AddWarning(message, time);
                }
            }
        }

        protected FileStream CreateLogFileStream()
        {
            string logFilePath = this.LogFilePath;
            for (uint i = 0; i < 0x3e8; i++)
            {
                try
                {
                    if (this._logFileLocation == LogFileLocation.RegistryBased)
                    {
                        return new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.None);
                    }
                    return new FileStream(logFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                }
                catch (IOException)
                {
                    if (i == 0x3e8)
                    {
                        throw;
                    }
                }
                Thread.Sleep(20);
            }
            return null;
        }

        internal static void EndCurrentThreadLogging()
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    currentThreadLogger.EndLogOperation();
                }
                RemoveCurrentThreadLogger();
            }
        }

        internal static void EndLogging(LogIdentity logIdentity)
        {
            try
            {
                Logger logger = GetLogger(logIdentity);
                if (logger != null)
                {
                    lock (logger)
                    {
                        logger.EndLogOperation();
                    }
                }
                RemoveLogger(logIdentity);
            }
            catch (Exception)
            {
            }
        }

        protected void EndLogOperation()
        {
            if (this.FlushLogs() && (this._logFileLocation == LogFileLocation.WinInetCache))
            {
                System.Deployment.Application.NativeMethods.CommitUrlCacheEntry(this._urlName, this._logFilePath, 0L, 0L, 4, null, 0, null, null);
            }
        }

        internal static bool FlushCurrentThreadLogs()
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    return currentThreadLogger.FlushLogs();
                }
            }
            return false;
        }

        internal static bool FlushLog(LogIdentity log)
        {
            Logger logger = GetLogger(log);
            if (logger != null)
            {
                lock (logger)
                {
                    return logger.FlushLogs();
                }
            }
            return false;
        }

        protected bool FlushLogs()
        {
            FileStream stream = null;
            try
            {
                stream = this.CreateLogFileStream();
                if (stream == null)
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return false;
            }
            catch (SecurityException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            StreamWriter writer = new StreamWriter(stream, LogFileEncoding);
            writer.WriteLine(Header);
            writer.Write(this.Sources);
            writer.Write(this.Identities);
            writer.Write(this.Summary);
            writer.WriteLine(this.Errors.ErrorSummary);
            writer.WriteLine(this.Transactions.FailureSummary);
            writer.WriteLine(this.Warnings);
            writer.WriteLine(this.Phases);
            writer.WriteLine(this.Errors);
            writer.WriteLine(this.Transactions);
            writer.Close();
            stream.Close();
            return true;
        }

        protected static uint GetCurrentLogThreadId() => 
            System.Deployment.Application.NativeMethods.GetCurrentThreadId();

        protected static Logger GetCurrentThreadLogger()
        {
            Logger logger = null;
            uint currentLogThreadId = GetCurrentLogThreadId();
            lock (_logAccessLock)
            {
                if (_threadLogIdTable.Contains(currentLogThreadId))
                {
                    LogIdentity identity = (LogIdentity) _threadLogIdTable[currentLogThreadId];
                    if (_loggerCollection.Contains(identity.ToString()))
                    {
                        logger = (Logger) _loggerCollection[identity.ToString()];
                    }
                }
            }
            return logger;
        }

        internal static string GetLogFilePath()
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                return GetLogFilePath(currentThreadLogger);
            }
            return null;
        }

        internal static string GetLogFilePath(Logger logger)
        {
            if (logger == null)
            {
                return null;
            }
            lock (logger)
            {
                return logger.LogFilePath;
            }
        }

        internal static string GetLogFilePath(LogIdentity log)
        {
            Logger logger = GetLogger(log);
            if (logger != null)
            {
                return GetLogFilePath(logger);
            }
            return null;
        }

        protected static Logger GetLogger(LogIdentity logIdentity)
        {
            Logger logger = null;
            lock (_logAccessLock)
            {
                if (_loggerCollection.Contains(logIdentity.ToString()))
                {
                    logger = (Logger) _loggerCollection[logIdentity.ToString()];
                }
            }
            return logger;
        }

        protected string GetRegitsryBasedLogFilePath()
        {
            string str = null;
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\Software\Microsoft\Windows\CurrentVersion\Deployment"))
                {
                    if (key != null)
                    {
                        str = key.GetValue("LogFilePath") as string;
                    }
                }
            }
            catch (ArgumentException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            return str;
        }

        protected string GetWinInetBasedLogFilePath()
        {
            try
            {
                string urlName = "System_Deployment_Log_";
                if (this.Identities.DeploymentIdentity != null)
                {
                    urlName = urlName + this.Identities.DeploymentIdentity.KeyForm;
                }
                urlName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[] { urlName, this.Identity.ToString() });
                StringBuilder fileName = new StringBuilder(0x105);
                if (!System.Deployment.Application.NativeMethods.CreateUrlCacheEntry(urlName, 0, "log", fileName, 0))
                {
                    return null;
                }
                this._urlName = urlName;
                return fileName.ToString();
            }
            catch (COMException)
            {
                return null;
            }
            catch (SEHException)
            {
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        protected static void RemoveCurrentThreadLogger()
        {
            lock (_logAccessLock)
            {
                uint currentLogThreadId = GetCurrentLogThreadId();
                if (_threadLogIdTable.Contains(currentLogThreadId))
                {
                    LogIdentity identity = (LogIdentity) _threadLogIdTable[currentLogThreadId];
                    _threadLogIdTable.Remove(currentLogThreadId);
                    if (_loggerCollection.Contains(identity.ToString()))
                    {
                        _loggerCollection.Remove(identity.ToString());
                    }
                }
            }
        }

        protected static void RemoveLogger(LogIdentity logIdentity)
        {
            lock (_logAccessLock)
            {
                if (_loggerCollection.Contains(logIdentity.ToString()))
                {
                    _loggerCollection.Remove(logIdentity.ToString());
                }
            }
        }

        internal static void SetApplicationManifest(AssemblyManifest applicationManifest)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    if (applicationManifest.Identity != null)
                    {
                        currentThreadLogger.Identities.ApplicationIdentity = applicationManifest.Identity;
                    }
                    currentThreadLogger.Summary.ApplicationManifest = applicationManifest;
                }
            }
        }

        internal static void SetApplicationManifest(LogIdentity log, AssemblyManifest applicationManifest)
        {
            Logger logger = GetLogger(log);
            if (logger != null)
            {
                lock (logger)
                {
                    if (applicationManifest.Identity != null)
                    {
                        logger.Identities.ApplicationIdentity = applicationManifest.Identity;
                    }
                    logger.Summary.ApplicationManifest = applicationManifest;
                }
            }
        }

        internal static void SetApplicationServerInformation(ServerInformation serverInformation)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    currentThreadLogger.Sources.ApplicationServerInformation = serverInformation;
                }
            }
        }

        internal static void SetApplicationUrl(Uri applicationUri)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    currentThreadLogger.Sources.ApplicationUri = applicationUri;
                }
            }
        }

        internal static void SetApplicationUrl(LogIdentity log, Uri applicationUri)
        {
            Logger logger = GetLogger(log);
            if (logger != null)
            {
                lock (logger)
                {
                    logger.Sources.ApplicationUri = applicationUri;
                }
            }
        }

        internal static void SetDeploymentManifest(AssemblyManifest deploymentManifest)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    if (deploymentManifest.Identity != null)
                    {
                        currentThreadLogger.Identities.DeploymentIdentity = deploymentManifest.Identity;
                    }
                    currentThreadLogger.Summary.DeploymentManifest = deploymentManifest;
                }
            }
        }

        internal static void SetDeploymentManifest(LogIdentity log, AssemblyManifest deploymentManifest)
        {
            Logger logger = GetLogger(log);
            if (logger != null)
            {
                lock (logger)
                {
                    if (deploymentManifest.Identity != null)
                    {
                        logger.Identities.DeploymentIdentity = deploymentManifest.Identity;
                    }
                    logger.Summary.DeploymentManifest = deploymentManifest;
                }
            }
        }

        internal static void SetDeploymentProviderServerInformation(ServerInformation serverInformation)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    currentThreadLogger.Sources.DeploymentProviderServerInformation = serverInformation;
                }
            }
        }

        internal static void SetDeploymentProviderUrl(Uri deploymentProviderUri)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    currentThreadLogger.Sources.DeploymentProviderUri = deploymentProviderUri;
                }
            }
        }

        internal static void SetSubscriptionServerInformation(ServerInformation serverInformation)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                lock (currentThreadLogger)
                {
                    currentThreadLogger.Sources.SubscriptionServerInformation = serverInformation;
                }
            }
        }

        private void SetSubscriptionUri(Uri subscriptionUri)
        {
            lock (this)
            {
                this.Sources.SubscriptionUri = subscriptionUri;
            }
        }

        internal static void SetSubscriptionUrl(string subscrioptionUrl)
        {
            try
            {
                Uri subscriptionUri = new Uri(subscrioptionUrl);
                SetSubscriptionUrl(subscriptionUri);
            }
            catch (UriFormatException)
            {
            }
        }

        internal static void SetSubscriptionUrl(Uri subscriptionUri)
        {
            Logger currentThreadLogger = GetCurrentThreadLogger();
            if (currentThreadLogger != null)
            {
                currentThreadLogger.SetSubscriptionUri(subscriptionUri);
            }
        }

        internal static void SetSubscriptionUrl(LogIdentity log, Uri subscriptionUri)
        {
            Logger logger = GetLogger(log);
            if (logger != null)
            {
                logger.SetSubscriptionUri(subscriptionUri);
            }
        }

        internal void SetTextualSubscriptionIdentity(System.Deployment.Application.DefinitionIdentity definitionIdentity)
        {
            lock (this)
            {
                this.Identities.DeploymentIdentity = definitionIdentity;
            }
        }

        internal static void SetTextualSubscriptionIdentity(string textualIdentity)
        {
            try
            {
                Logger currentThreadLogger = GetCurrentThreadLogger();
                if (currentThreadLogger != null)
                {
                    currentThreadLogger.SetTextualSubscriptionIdentity(new System.Deployment.Application.DefinitionIdentity(textualIdentity));
                }
            }
            catch (COMException)
            {
            }
            catch (SEHException)
            {
            }
        }

        internal static void SetTextualSubscriptionIdentity(LogIdentity log, string textualIdentity)
        {
            try
            {
                Logger logger = GetLogger(log);
                if (logger != null)
                {
                    logger.SetTextualSubscriptionIdentity(new System.Deployment.Application.DefinitionIdentity(textualIdentity));
                }
            }
            catch (COMException)
            {
            }
            catch (SEHException)
            {
            }
        }

        internal static LogIdentity StartCurrentThreadLogging()
        {
            EndCurrentThreadLogging();
            Logger logger = new Logger();
            AddCurrentThreadLogger(logger);
            return logger.Identity;
        }

        internal static LogIdentity StartLogging()
        {
            Logger logger = new Logger();
            AddLogger(logger);
            return logger.Identity;
        }

        protected ErrorSection Errors =>
            this._errors;

        protected static HeaderSection Header
        {
            get
            {
                if (_header == null)
                {
                    object obj2 = new HeaderSection();
                    Interlocked.CompareExchange(ref _header, obj2, null);
                }
                return (HeaderSection) _header;
            }
        }

        protected IdentitySection Identities =>
            this._identities;

        protected LogIdentity Identity =>
            this._logIdentity;

        protected static Encoding LogFileEncoding
        {
            get
            {
                if (_logFileEncoding == null)
                {
                    Encoding unicode = null;
                    if (PlatformSpecific.OnWin9x)
                    {
                        unicode = Encoding.Default;
                    }
                    else
                    {
                        unicode = Encoding.Unicode;
                    }
                    Interlocked.CompareExchange(ref _logFileEncoding, unicode, null);
                }
                return (Encoding) _logFileEncoding;
            }
        }

        protected string LogFilePath
        {
            get
            {
                if (this._logFilePath == null)
                {
                    this._logFilePath = this.GetRegitsryBasedLogFilePath();
                    if (this._logFilePath == null)
                    {
                        this._logFilePath = this.GetWinInetBasedLogFilePath();
                        if (this._logFilePath != null)
                        {
                            this._logFileLocation = LogFileLocation.WinInetCache;
                        }
                    }
                    else
                    {
                        this._logFileLocation = LogFileLocation.RegistryBased;
                    }
                }
                return this._logFilePath;
            }
        }

        protected PhaseSection Phases =>
            this._phases;

        protected SourceSection Sources =>
            this._sources;

        protected SummarySection Summary =>
            this._summary;

        protected TransactionSection Transactions =>
            this._transactions;

        protected WarningSection Warnings =>
            this._warnings;

        protected class ErrorInformation : Logger.LogInformation
        {
            protected Exception _exception;

            public ErrorInformation(string message, Exception exception, DateTime time) : base(message, time)
            {
                this._exception = exception;
            }

            private static string GetExceptionType(Exception exception)
            {
                if (!(exception is DeploymentException))
                {
                    return exception.GetType().ToString();
                }
                DeploymentException exception2 = (DeploymentException) exception;
                if (exception2.SubType != ExceptionTypes.Unknown)
                {
                    return string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_ExceptionType"), new object[] { exception2.GetType().ToString(), exception2.SubType.ToString() });
                }
                return string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_ExceptionTypeUnknown"), new object[] { exception2.GetType().ToString() });
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                for (Exception exception = this._exception; exception != null; exception = exception.InnerException)
                {
                    string str = null;
                    if (exception.StackTrace != null)
                    {
                        str = exception.StackTrace.Replace("   ", "\t\t\t");
                    }
                    if (exception == this._exception)
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IndividualErrorOutermostException"), new object[] { base.Time.ToString(DateTimeFormatInfo.CurrentInfo), GetExceptionType(exception), exception.Message, exception.Source, str });
                    }
                    else
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IndividualErrorInnerException"), new object[] { GetExceptionType(exception), exception.Message, exception.Source, str });
                    }
                }
                return builder.ToString();
            }

            public string Summary
            {
                get
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IndividualErrorSummary"), new object[] { base._message });
                    for (Exception exception = this._exception; exception != null; exception = exception.InnerException)
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IndividualErrorSummaryBullets"), new object[] { exception.Message });
                    }
                    return builder.ToString();
                }
            }
        }

        protected class ErrorSection : Logger.LogInformation
        {
            protected ArrayList _errors = new ArrayList();

            public void AddError(string message, Exception exception, DateTime time)
            {
                Logger.ErrorInformation information = new Logger.ErrorInformation(message, exception, time);
                this._errors.Add(information);
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Resources.GetString("LogFile_Error"));
                if (this._errors.Count > 0)
                {
                    builder.Append(Resources.GetString("LogFile_ErrorStatusError"));
                    foreach (Logger.ErrorInformation information in this._errors)
                    {
                        builder.Append(information.ToString());
                    }
                }
                else
                {
                    builder.Append(Resources.GetString("LogFile_ErrorStatusNoError"));
                }
                return builder.ToString();
            }

            public string ErrorSummary
            {
                get
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(Resources.GetString("LogFile_ErrorSummary"));
                    if (this._errors.Count > 0)
                    {
                        builder.Append(Resources.GetString("LogFile_ErrorSummaryStatusError"));
                        foreach (Logger.ErrorInformation information in this._errors)
                        {
                            builder.Append(information.Summary);
                        }
                    }
                    else
                    {
                        builder.Append(Resources.GetString("LogFile_ErrorSummaryStatusNoError"));
                    }
                    return builder.ToString();
                }
            }
        }

        protected class HeaderSection : Logger.LogInformation
        {
            public HeaderSection()
            {
                base._message = GenerateLogHeaderText();
            }

            protected static string GenerateLogHeaderText()
            {
                string executingAssemblyPath = GetExecutingAssemblyPath();
                string modulePath = GetModulePath("mscorwks.dll");
                string str3 = GetModulePath("dfdll.dll");
                string str4 = GetModulePath("dfshim.dll");
                FileVersionInfo versionInfo = GetVersionInfo(executingAssemblyPath);
                if (versionInfo == null)
                {
                    versionInfo = GetVersionInfo(GetModulePathInClrFolder("system.deployment.dll"));
                }
                FileVersionInfo info2 = GetVersionInfo(modulePath);
                FileVersionInfo info3 = GetVersionInfo(str3);
                FileVersionInfo info4 = GetVersionInfo(str4);
                StringBuilder builder = new StringBuilder();
                try
                {
                    builder.Append(Resources.GetString("LogFile_Header"));
                    builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderOSVersion"), new object[] { Environment.OSVersion.Platform.ToString(), Environment.OSVersion.Version.ToString() });
                    builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderCLRVersion"), new object[] { Environment.Version.ToString() });
                    if (versionInfo != null)
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderSystemDeploymentVersion"), new object[] { versionInfo.FileVersion });
                    }
                    if (info2 != null)
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderMscorwksVersion"), new object[] { info2.FileVersion });
                    }
                    if (info3 != null)
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderDfdllVersion"), new object[] { info3.FileVersion });
                    }
                    if (info4 != null)
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderDfshimVersion"), new object[] { info4.FileVersion });
                    }
                }
                catch (ArgumentException)
                {
                }
                catch (FormatException)
                {
                }
                return builder.ToString();
            }

            protected static string GetExecutingAssemblyPath() => 
                Assembly.GetExecutingAssembly().Location;

            protected static string GetModulePath(string moduleName)
            {
                string loadedModulePath = System.Deployment.Application.NativeMethods.GetLoadedModulePath(moduleName);
                if (loadedModulePath == null)
                {
                    loadedModulePath = GetModulePathInClrFolder(moduleName);
                    if (loadedModulePath == null)
                    {
                        loadedModulePath = GetModulePathInSystemFolder(moduleName);
                    }
                }
                return loadedModulePath;
            }

            protected static string GetModulePathInClrFolder(string moduleName)
            {
                string str = null;
                string loadedModulePath = System.Deployment.Application.NativeMethods.GetLoadedModulePath("dfsvc.exe");
                try
                {
                    if (loadedModulePath != null)
                    {
                        str = Path.Combine(Path.GetDirectoryName(loadedModulePath), moduleName);
                    }
                }
                catch (ArgumentException)
                {
                    str = null;
                }
                return str;
            }

            protected static string GetModulePathInSystemFolder(string moduleName)
            {
                try
                {
                    return Path.Combine(Environment.SystemDirectory, moduleName);
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }

            protected static FileVersionInfo GetVersionInfo(string modulePath)
            {
                FileVersionInfo versionInfo = null;
                if ((modulePath != null) && System.IO.File.Exists(modulePath))
                {
                    try
                    {
                        versionInfo = FileVersionInfo.GetVersionInfo(modulePath);
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
                return versionInfo;
            }

            public override string ToString() => 
                base._message;
        }

        protected class IdentitySection : Logger.LogInformation
        {
            protected System.Deployment.Application.DefinitionIdentity _applicationIdentity;
            protected System.Deployment.Application.DefinitionIdentity _deploymentIdentity;

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                if ((this._deploymentIdentity != null) || (this._applicationIdentity != null))
                {
                    builder.Append(Resources.GetString("LogFile_Identity"));
                    if (this._deploymentIdentity != null)
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IdentityDeploymentIdentity"), new object[] { this._deploymentIdentity.ToString() });
                    }
                    if (this._applicationIdentity != null)
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IdentityApplicationIdentity"), new object[] { this._applicationIdentity.ToString() });
                    }
                    builder.Append(Environment.NewLine);
                }
                return builder.ToString();
            }

            public System.Deployment.Application.DefinitionIdentity ApplicationIdentity
            {
                set
                {
                    this._applicationIdentity = value;
                }
            }

            public System.Deployment.Application.DefinitionIdentity DeploymentIdentity
            {
                get => 
                    this._deploymentIdentity;
                set
                {
                    this._deploymentIdentity = value;
                }
            }
        }

        protected enum LogFileLocation
        {
            NoLogFile,
            RegistryBased,
            WinInetCache
        }

        public class LogIdentity
        {
            protected string _logIdentityStringForm;
            protected readonly uint _threadId = System.Deployment.Application.NativeMethods.GetCurrentThreadId();
            protected readonly long _ticks = DateTime.Now.Ticks;

            public override string ToString()
            {
                if (this._logIdentityStringForm == null)
                {
                    this._logIdentityStringForm = string.Format(CultureInfo.InvariantCulture, "{0:x8}{1:x16}", new object[] { this._threadId, this._ticks });
                }
                return this._logIdentityStringForm;
            }

            public uint ThreadId =>
                this._threadId;
        }

        protected class LogInformation
        {
            protected string _message;
            protected DateTime _time;

            public LogInformation()
            {
                this._message = "";
                this._time = DateTime.Now;
            }

            public LogInformation(string message, DateTime time)
            {
                this._message = "";
                this._time = DateTime.Now;
                if (message != null)
                {
                    this._message = message;
                }
                this._time = time;
            }

            public string Message =>
                this._message;

            public DateTime Time =>
                this._time;
        }

        protected class PhaseSection : Logger.LogInformation
        {
            protected ArrayList _phaseInformations = new ArrayList();

            public void AddPhaseInformation(string phaseMessage, DateTime time)
            {
                Logger.LogInformation information = new Logger.LogInformation(phaseMessage, time);
                this._phaseInformations.Add(information);
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Resources.GetString("LogFile_PhaseInformation"));
                if (this._phaseInformations.Count > 0)
                {
                    foreach (Logger.LogInformation information in this._phaseInformations)
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_PhaseInformationStatusIndivualPhaseInformation"), new object[] { information.Time.ToString(DateTimeFormatInfo.CurrentInfo), information.Message });
                    }
                }
                else
                {
                    builder.Append(Resources.GetString("LogFile_PhaseInformationStatusNoPhaseInformation"));
                }
                return builder.ToString();
            }
        }

        protected class SourceSection : Logger.LogInformation
        {
            protected ServerInformation _applicationServerInformation;
            protected Uri _applicationUri;
            protected ServerInformation _deploymentProviderServerInformation;
            protected Uri _deploymentProviderUri;
            protected ServerInformation _subscriptionServerInformation;
            protected Uri _subscriptonUri;

            private static void AppendServerInformation(StringBuilder destination, ServerInformation serverInformation)
            {
                if (!string.IsNullOrEmpty(serverInformation.Server))
                {
                    destination.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_ServerInformationServer"), new object[] { serverInformation.Server });
                }
                if (!string.IsNullOrEmpty(serverInformation.PoweredBy))
                {
                    destination.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_ServerInformationPoweredBy"), new object[] { serverInformation.PoweredBy });
                }
                if (!string.IsNullOrEmpty(serverInformation.AspNetVersion))
                {
                    destination.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_ServerInformationAspNetVersion"), new object[] { serverInformation.AspNetVersion });
                }
            }

            public override string ToString()
            {
                StringBuilder destination = new StringBuilder();
                if (((this._subscriptonUri != null) || (this._deploymentProviderUri != null)) || (this._applicationUri != null))
                {
                    destination.Append(Resources.GetString("LogFile_Source"));
                    if (this._subscriptonUri != null)
                    {
                        destination.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_SourceDeploymentUrl"), new object[] { this._subscriptonUri.AbsoluteUri });
                    }
                    if (this._subscriptionServerInformation != null)
                    {
                        AppendServerInformation(destination, this._subscriptionServerInformation);
                    }
                    if (this._deploymentProviderUri != null)
                    {
                        destination.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_SourceDeploymentProviderUrl"), new object[] { this._deploymentProviderUri.AbsoluteUri });
                    }
                    if (this._deploymentProviderServerInformation != null)
                    {
                        AppendServerInformation(destination, this._deploymentProviderServerInformation);
                    }
                    if (this._applicationUri != null)
                    {
                        destination.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_SourceApplicationUrl"), new object[] { this._applicationUri.AbsoluteUri });
                    }
                    if (this._applicationServerInformation != null)
                    {
                        AppendServerInformation(destination, this._applicationServerInformation);
                    }
                    destination.Append(Environment.NewLine);
                }
                return destination.ToString();
            }

            public ServerInformation ApplicationServerInformation
            {
                set
                {
                    this._applicationServerInformation = value;
                }
            }

            public Uri ApplicationUri
            {
                set
                {
                    this._applicationUri = value;
                }
            }

            public ServerInformation DeploymentProviderServerInformation
            {
                set
                {
                    this._deploymentProviderServerInformation = value;
                }
            }

            public Uri DeploymentProviderUri
            {
                set
                {
                    this._deploymentProviderUri = value;
                }
            }

            public ServerInformation SubscriptionServerInformation
            {
                set
                {
                    this._subscriptionServerInformation = value;
                }
            }

            public Uri SubscriptionUri
            {
                set
                {
                    this._subscriptonUri = value;
                }
            }
        }

        protected class SummarySection : Logger.LogInformation
        {
            protected AssemblyManifest _applicationManifest;
            protected AssemblyManifest _deploymentManifest;

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                if (this._deploymentManifest != null)
                {
                    builder.Append(Resources.GetString("LogFile_Summary"));
                    if (this._deploymentManifest.Deployment.Install)
                    {
                        builder.Append(Resources.GetString("LogFile_SummaryInstallableApp"));
                    }
                    else
                    {
                        builder.Append(Resources.GetString("LogFile_SummaryOnlineOnlyApp"));
                    }
                    if (this._deploymentManifest.Deployment.TrustURLParameters)
                    {
                        builder.Append(Resources.GetString("LogFile_SummaryTrustUrlParameterSet"));
                    }
                    if ((this._applicationManifest != null) && this._applicationManifest.EntryPoints[0].HostInBrowser)
                    {
                        builder.Append(Resources.GetString("LogFile_SummaryBrowserHostedApp"));
                    }
                    builder.Append(Environment.NewLine);
                }
                return builder.ToString();
            }

            public AssemblyManifest ApplicationManifest
            {
                set
                {
                    this._applicationManifest = value;
                }
            }

            public AssemblyManifest DeploymentManifest
            {
                set
                {
                    this._deploymentManifest = value;
                }
            }
        }

        protected class TransactionInformation : Logger.LogInformation
        {
            protected bool _failed;
            protected ArrayList _operations;

            public TransactionInformation(System.Deployment.Internal.Isolation.StoreTransactionOperation[] storeOperations, uint[] rgDispositions, int[] rgResults, DateTime time) : base(null, time)
            {
                this._operations = new ArrayList();
                int num = Math.Min(Math.Min(storeOperations.Length, rgDispositions.Length), rgResults.Length);
                int index = 0;
                for (index = 0; index < num; index++)
                {
                    TransactionOperation operation = new TransactionOperation(storeOperations[index], rgDispositions[index], rgResults[index]);
                    this._operations.Add(operation);
                    if (operation.Failed)
                    {
                        this._failed = true;
                    }
                }
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionItem"), new object[] { base.Time.ToString(DateTimeFormatInfo.CurrentInfo) });
                foreach (TransactionOperation operation in this._operations)
                {
                    builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionBullets"), new object[] { operation });
                }
                return builder.ToString();
            }

            public bool Failed =>
                this._failed;

            public string FailureSummary
            {
                get
                {
                    if (!this.Failed)
                    {
                        return Resources.GetString("LogFile_TransactionFailureSummaryNoFailure");
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionFailureSummaryItem"), new object[] { base.Time.ToString(DateTimeFormatInfo.CurrentInfo) });
                    foreach (TransactionOperation operation in this._operations)
                    {
                        if (operation.Failed)
                        {
                            builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionFailureSummaryBullets"), new object[] { operation.FailureMessage });
                        }
                    }
                    return builder.ToString();
                }
            }

            public class TransactionOperation
            {
                protected bool _failed;
                protected string _failureMessage = "";
                protected string _message = "";

                public TransactionOperation(System.Deployment.Internal.Isolation.StoreTransactionOperation operation, uint disposition, int hresult)
                {
                    this.AnalyzeTransactionOperation(operation, disposition, hresult);
                }

                protected void AnalyzeTransactionOperation(System.Deployment.Internal.Isolation.StoreTransactionOperation operation, uint dispositionValue, int hresult)
                {
                    string str = "";
                    try
                    {
                        if (operation.Operation == System.Deployment.Internal.Isolation.StoreTransactionOperationType.StageComponent)
                        {
                            System.Deployment.Internal.Isolation.StoreOperationStageComponent component = (System.Deployment.Internal.Isolation.StoreOperationStageComponent) Marshal.PtrToStructure(operation.Data.DataPtr, typeof(System.Deployment.Internal.Isolation.StoreOperationStageComponent));
                            System.Deployment.Internal.Isolation.StoreOperationStageComponent.Disposition disposition = (System.Deployment.Internal.Isolation.StoreOperationStageComponent.Disposition) dispositionValue;
                            str = disposition.ToString();
                            this._message = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionStageComponent"), new object[] { component.GetType().ToString(), str, hresult, Path.GetFileName(component.ManifestPath) });
                            if (disposition == System.Deployment.Internal.Isolation.StoreOperationStageComponent.Disposition.Failed)
                            {
                                this._failureMessage = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionStageComponentFailure"), new object[] { Path.GetFileName(component.ManifestPath) });
                                this._failed = true;
                            }
                        }
                        else if (operation.Operation == System.Deployment.Internal.Isolation.StoreTransactionOperationType.PinDeployment)
                        {
                            System.Deployment.Internal.Isolation.StoreOperationPinDeployment deployment = (System.Deployment.Internal.Isolation.StoreOperationPinDeployment) Marshal.PtrToStructure(operation.Data.DataPtr, typeof(System.Deployment.Internal.Isolation.StoreOperationPinDeployment));
                            System.Deployment.Internal.Isolation.StoreOperationPinDeployment.Disposition disposition2 = (System.Deployment.Internal.Isolation.StoreOperationPinDeployment.Disposition) dispositionValue;
                            str = disposition2.ToString();
                            System.Deployment.Application.DefinitionAppId id = new System.Deployment.Application.DefinitionAppId(deployment.Application);
                            this._message = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionPinDeployment"), new object[] { deployment.GetType().ToString(), str, hresult, id.ToString() });
                            if (disposition2 == System.Deployment.Internal.Isolation.StoreOperationPinDeployment.Disposition.Failed)
                            {
                                this._failureMessage = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionPinDeploymentFailure"), new object[] { id.ToString() });
                                this._failed = true;
                            }
                        }
                        else if (operation.Operation == System.Deployment.Internal.Isolation.StoreTransactionOperationType.UnpinDeployment)
                        {
                            System.Deployment.Internal.Isolation.StoreOperationUnpinDeployment deployment2 = (System.Deployment.Internal.Isolation.StoreOperationUnpinDeployment) Marshal.PtrToStructure(operation.Data.DataPtr, typeof(System.Deployment.Internal.Isolation.StoreOperationUnpinDeployment));
                            System.Deployment.Internal.Isolation.StoreOperationUnpinDeployment.Disposition disposition3 = (System.Deployment.Internal.Isolation.StoreOperationUnpinDeployment.Disposition) dispositionValue;
                            str = disposition3.ToString();
                            System.Deployment.Application.DefinitionAppId id2 = new System.Deployment.Application.DefinitionAppId(deployment2.Application);
                            this._message = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionUnPinDeployment"), new object[] { deployment2.GetType().ToString(), str, hresult, id2.ToString() });
                            if (disposition3 == System.Deployment.Internal.Isolation.StoreOperationUnpinDeployment.Disposition.Failed)
                            {
                                this._failureMessage = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionUnPinDeploymentFailure"), new object[] { id2.ToString() });
                                this._failed = true;
                            }
                        }
                        else if (operation.Operation == System.Deployment.Internal.Isolation.StoreTransactionOperationType.InstallDeployment)
                        {
                            System.Deployment.Internal.Isolation.StoreOperationInstallDeployment deployment3 = (System.Deployment.Internal.Isolation.StoreOperationInstallDeployment) Marshal.PtrToStructure(operation.Data.DataPtr, typeof(System.Deployment.Internal.Isolation.StoreOperationInstallDeployment));
                            System.Deployment.Internal.Isolation.StoreOperationInstallDeployment.Disposition disposition4 = (System.Deployment.Internal.Isolation.StoreOperationInstallDeployment.Disposition) dispositionValue;
                            str = disposition4.ToString();
                            System.Deployment.Application.DefinitionAppId id3 = new System.Deployment.Application.DefinitionAppId(deployment3.Application);
                            this._message = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionInstallDeployment"), new object[] { deployment3.GetType().ToString(), str, hresult, id3.ToString() });
                            if (disposition4 == System.Deployment.Internal.Isolation.StoreOperationInstallDeployment.Disposition.Failed)
                            {
                                this._failureMessage = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionInstallDeploymentFailure"), new object[] { id3.ToString() });
                                this._failed = true;
                            }
                        }
                        else if (operation.Operation == System.Deployment.Internal.Isolation.StoreTransactionOperationType.UninstallDeployment)
                        {
                            System.Deployment.Internal.Isolation.StoreOperationUninstallDeployment deployment4 = (System.Deployment.Internal.Isolation.StoreOperationUninstallDeployment) Marshal.PtrToStructure(operation.Data.DataPtr, typeof(System.Deployment.Internal.Isolation.StoreOperationUninstallDeployment));
                            System.Deployment.Internal.Isolation.StoreOperationUninstallDeployment.Disposition disposition5 = (System.Deployment.Internal.Isolation.StoreOperationUninstallDeployment.Disposition) dispositionValue;
                            str = disposition5.ToString();
                            System.Deployment.Application.DefinitionAppId id4 = new System.Deployment.Application.DefinitionAppId(deployment4.Application);
                            this._message = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionUninstallDeployment"), new object[] { deployment4.GetType().ToString(), str, hresult, id4.ToString() });
                            if (disposition5 == System.Deployment.Internal.Isolation.StoreOperationUninstallDeployment.Disposition.Failed)
                            {
                                this._failureMessage = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionUninstallDeploymentFailure"), new object[] { id4.ToString() });
                                this._failed = true;
                            }
                        }
                        else if (operation.Operation == System.Deployment.Internal.Isolation.StoreTransactionOperationType.SetDeploymentMetadata)
                        {
                            System.Deployment.Internal.Isolation.StoreOperationSetDeploymentMetadata metadata = (System.Deployment.Internal.Isolation.StoreOperationSetDeploymentMetadata) Marshal.PtrToStructure(operation.Data.DataPtr, typeof(System.Deployment.Internal.Isolation.StoreOperationSetDeploymentMetadata));
                            System.Deployment.Internal.Isolation.StoreOperationSetDeploymentMetadata.Disposition disposition6 = (System.Deployment.Internal.Isolation.StoreOperationSetDeploymentMetadata.Disposition) dispositionValue;
                            str = disposition6.ToString();
                            this._message = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionSetDeploymentMetadata"), new object[] { metadata.GetType().ToString(), str, hresult });
                            if (disposition6 == System.Deployment.Internal.Isolation.StoreOperationSetDeploymentMetadata.Disposition.Failed)
                            {
                                this._failureMessage = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionSetDeploymentMetadataFailure"), new object[0]);
                                this._failed = true;
                            }
                        }
                        else if (operation.Operation == System.Deployment.Internal.Isolation.StoreTransactionOperationType.StageComponentFile)
                        {
                            System.Deployment.Internal.Isolation.StoreOperationStageComponentFile file = (System.Deployment.Internal.Isolation.StoreOperationStageComponentFile) Marshal.PtrToStructure(operation.Data.DataPtr, typeof(System.Deployment.Internal.Isolation.StoreOperationStageComponentFile));
                            System.Deployment.Internal.Isolation.StoreOperationStageComponentFile.Disposition disposition7 = (System.Deployment.Internal.Isolation.StoreOperationStageComponentFile.Disposition) dispositionValue;
                            str = disposition7.ToString();
                            this._message = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionStageComponentFile"), new object[] { file.GetType().ToString(), str, hresult, file.ComponentRelativePath });
                            if (disposition7 == System.Deployment.Internal.Isolation.StoreOperationStageComponentFile.Disposition.Failed)
                            {
                                this._failureMessage = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionStageComponentFileFailure"), new object[] { file.ComponentRelativePath });
                                this._failed = true;
                            }
                        }
                        else
                        {
                            this._message = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionUnknownOperation"), new object[] { operation.Operation.GetType().ToString(), (uint) operation.Operation, hresult });
                        }
                    }
                    catch (FormatException)
                    {
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                public override string ToString() => 
                    this._message;

                public bool Failed =>
                    this._failed;

                public string FailureMessage =>
                    this._failureMessage;
            }
        }

        protected class TransactionSection : Logger.LogInformation
        {
            protected ArrayList _failedTransactionInformations = new ArrayList();
            protected ArrayList _transactionInformations = new ArrayList();

            public void AddTransactionInformation(System.Deployment.Internal.Isolation.StoreTransactionOperation[] storeOperations, uint[] rgDispositions, int[] rgResults, DateTime time)
            {
                Logger.TransactionInformation information = new Logger.TransactionInformation(storeOperations, rgDispositions, rgResults, time);
                this._transactionInformations.Add(information);
                if (information.Failed)
                {
                    this._failedTransactionInformations.Add(information);
                }
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Resources.GetString("LogFile_Transaction"));
                if (this._transactionInformations.Count > 0)
                {
                    foreach (Logger.TransactionInformation information in this._transactionInformations)
                    {
                        builder.Append(information.ToString());
                    }
                }
                else
                {
                    builder.Append(Resources.GetString("LogFile_TransactionNoTransaction"));
                }
                return builder.ToString();
            }

            public string FailureSummary
            {
                get
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(Resources.GetString("LogFile_TransactionFailureSummary"));
                    if (this._failedTransactionInformations.Count > 0)
                    {
                        foreach (Logger.TransactionInformation information in this._failedTransactionInformations)
                        {
                            builder.Append(information.FailureSummary);
                        }
                    }
                    else
                    {
                        builder.Append(Resources.GetString("LogFile_TransactionFailureSummaryNoError"));
                    }
                    return builder.ToString();
                }
            }
        }

        protected class WarningSection : Logger.LogInformation
        {
            protected ArrayList _warnings = new ArrayList();

            public void AddWarning(string message, DateTime time)
            {
                Logger.LogInformation information = new Logger.LogInformation(message, time);
                this._warnings.Add(information);
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Resources.GetString("LogFile_Warning"));
                if (this._warnings.Count > 0)
                {
                    foreach (Logger.LogInformation information in this._warnings)
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, Resources.GetString("LogFile_WarningStatusIndivualWarning"), new object[] { information.Message });
                    }
                }
                else
                {
                    builder.Append(Resources.GetString("LogFile_WarningStatusNoWarning"));
                }
                return builder.ToString();
            }
        }
    }
}

