namespace System.Diagnostics
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;

    [MonitoringDescription("EventLogDesc"), InstallerType("System.Diagnostics.EventLogInstaller, System.Configuration.Install, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultEvent("EntryWritten")]
    public class EventLog : Component, ISupportInitialize
    {
        private BitVector32 boolFlags;
        private const int BUF_SIZE = 0x9c40;
        private int bytesCached;
        private byte[] cache;
        private const int DefaultMaxSize = 0x80000;
        private const int DefaultRetention = 0x93a80;
        internal const string DllName = "EventLogMessages.dll";
        private EventLogEntryCollection entriesCollection;
        private const string EventLogKey = @"SYSTEM\CurrentControlSet\Services\EventLog";
        private const string eventLogMutexName = "netfxeventlog.1.0";
        private int firstCachedEntry;
        private const int Flag_disposed = 0x100;
        private const int Flag_forwards = 2;
        private const int Flag_initializing = 4;
        private const int Flag_monitoring = 8;
        private const int Flag_notifying = 1;
        private const int Flag_registeredAsListener = 0x10;
        private const int Flag_sourceVerified = 0x200;
        private const int Flag_writeGranted = 0x20;
        private int lastSeenCount;
        private int lastSeenEntry;
        private int lastSeenPos;
        private static Hashtable listenerInfos = new Hashtable(StringComparer.OrdinalIgnoreCase);
        private string logDisplayName;
        private string logName;
        private string machineName;
        private Hashtable messageLibraries;
        private EntryWrittenEventHandler onEntryWrittenHandler;
        private SafeEventLogReadHandle readHandle;
        private static bool s_CheckedOsVersion;
        private static object s_InternalSyncObject;
        private static bool s_SkipRegPatch;
        private const int SecondsPerDay = 0x15180;
        private string sourceName;
        private ISynchronizeInvoke synchronizingObject;
        private SafeEventLogWriteHandle writeHandle;

        [MonitoringDescription("LogEntryWritten")]
        public event EntryWrittenEventHandler EntryWritten
        {
            add
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
                this.onEntryWrittenHandler = (EntryWrittenEventHandler) Delegate.Combine(this.onEntryWrittenHandler, value);
            }
            remove
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
                this.onEntryWrittenHandler = (EntryWrittenEventHandler) Delegate.Remove(this.onEntryWrittenHandler, value);
            }
        }

        public EventLog() : this("", ".", "")
        {
        }

        public EventLog(string logName) : this(logName, ".", "")
        {
        }

        public EventLog(string logName, string machineName) : this(logName, machineName, "")
        {
        }

        public EventLog(string logName, string machineName, string source)
        {
            this.firstCachedEntry = -1;
            this.boolFlags = new BitVector32();
            if (logName == null)
            {
                throw new ArgumentNullException("logName");
            }
            if (!ValidLogName(logName, true))
            {
                throw new ArgumentException(SR.GetString("BadLogName"));
            }
            if (!SyntaxCheck.CheckMachineName(machineName))
            {
                throw new ArgumentException(SR.GetString("InvalidParameter", new object[] { "machineName", machineName }));
            }
            new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
            this.machineName = machineName;
            this.logName = logName;
            this.sourceName = source;
            this.readHandle = null;
            this.writeHandle = null;
            this.boolFlags[2] = true;
        }

        private static PermissionSet _GetAssertPermSet()
        {
            PermissionSet set = new PermissionSet(PermissionState.None);
            RegistryPermission perm = new RegistryPermission(PermissionState.Unrestricted);
            set.AddPermission(perm);
            EnvironmentPermission permission2 = new EnvironmentPermission(PermissionState.Unrestricted);
            set.AddPermission(permission2);
            return set;
        }

        [HostProtection(SecurityAction.LinkDemand, Synchronization=true)]
        private static void AddListenerComponent(EventLog component, string compMachineName, string compLogName)
        {
            lock (InternalSyncObject)
            {
                LogListeningInfo state = (LogListeningInfo) listenerInfos[compLogName];
                if (state != null)
                {
                    state.listeningComponents.Add(component);
                }
                else
                {
                    state = new LogListeningInfo();
                    state.listeningComponents.Add(component);
                    state.handleOwner = new EventLog();
                    state.handleOwner.MachineName = compMachineName;
                    state.handleOwner.Log = compLogName;
                    SafeEventHandle hEvent = SafeEventHandle.CreateEvent(NativeMethods.NullHandleRef, false, false, null);
                    if (hEvent.IsInvalid)
                    {
                        Win32Exception innerException = null;
                        if (Marshal.GetLastWin32Error() != 0)
                        {
                            innerException = SharedUtils.CreateSafeWin32Exception();
                        }
                        throw new InvalidOperationException(SR.GetString("NotifyCreateFailed"), innerException);
                    }
                    if (!Microsoft.Win32.UnsafeNativeMethods.NotifyChangeEventLog(state.handleOwner.ReadHandle, hEvent))
                    {
                        throw new InvalidOperationException(SR.GetString("CantMonitorEventLog"), SharedUtils.CreateSafeWin32Exception());
                    }
                    state.waitHandle = new EventLogWaitHandle(hEvent);
                    state.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(state.waitHandle, new WaitOrTimerCallback(EventLog.StaticCompletionCallback), state, -1, false);
                    listenerInfos[compLogName] = state;
                }
            }
        }

        public void BeginInit()
        {
            string machineName = this.machineName;
            new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
            if (this.boolFlags[4])
            {
                throw new InvalidOperationException(SR.GetString("InitTwice"));
            }
            this.boolFlags[4] = true;
            if (this.boolFlags[8])
            {
                this.StopListening(this.GetLogName(machineName));
            }
        }

        private static bool CharIsPrintable(char c)
        {
            UnicodeCategory unicodeCategory = char.GetUnicodeCategory(c);
            if (((unicodeCategory == UnicodeCategory.Control) && (unicodeCategory != UnicodeCategory.Format)) && ((unicodeCategory != UnicodeCategory.LineSeparator) && (unicodeCategory != UnicodeCategory.ParagraphSeparator)))
            {
                return (unicodeCategory == UnicodeCategory.OtherNotAssigned);
            }
            return true;
        }

        public void Clear()
        {
            string machineName = this.machineName;
            new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
            if (!this.IsOpenForRead)
            {
                this.OpenForRead(machineName);
            }
            if (!Microsoft.Win32.UnsafeNativeMethods.ClearEventLog(this.readHandle, NativeMethods.NullHandleRef) && (Marshal.GetLastWin32Error() != 2))
            {
                throw SharedUtils.CreateSafeWin32Exception();
            }
            this.Reset(machineName);
        }

        public void Close()
        {
            this.Close(this.machineName);
        }

        private void Close(string currentMachineName)
        {
            new EventLogPermission(EventLogPermissionAccess.Write, currentMachineName).Demand();
            if (this.readHandle != null)
            {
                try
                {
                    this.readHandle.Close();
                }
                catch (IOException)
                {
                    throw SharedUtils.CreateSafeWin32Exception();
                }
                this.readHandle = null;
            }
            if (this.writeHandle != null)
            {
                try
                {
                    this.writeHandle.Close();
                }
                catch (IOException)
                {
                    throw SharedUtils.CreateSafeWin32Exception();
                }
                this.writeHandle = null;
            }
            if (this.boolFlags[8])
            {
                this.StopRaisingEvents(this.GetLogName(currentMachineName));
            }
            if (this.messageLibraries != null)
            {
                foreach (SafeLibraryHandle handle in this.messageLibraries.Values)
                {
                    handle.Close();
                }
                this.messageLibraries = null;
            }
            this.boolFlags[0x200] = false;
        }

        private void CompletionCallback(object context)
        {
            if (!this.boolFlags[0x100])
            {
                lock (this)
                {
                    if (this.boolFlags[1])
                    {
                        return;
                    }
                    this.boolFlags[1] = true;
                }
                int lastSeenCount = this.lastSeenCount;
                try
                {
                    EventLogEntry entry;
                    int oldestEntryNumber = this.OldestEntryNumber;
                    int num3 = this.EntryCount + oldestEntryNumber;
                    goto Label_00D3;
                Label_005F:
                    entry = this.GetEntryWithOldest(lastSeenCount);
                    if ((this.SynchronizingObject != null) && this.SynchronizingObject.InvokeRequired)
                    {
                        this.SynchronizingObject.BeginInvoke(this.onEntryWrittenHandler, new object[] { this, new EntryWrittenEventArgs(entry) });
                    }
                    else
                    {
                        this.onEntryWrittenHandler(this, new EntryWrittenEventArgs(entry));
                    }
                    lastSeenCount++;
                Label_00BF:
                    if (lastSeenCount < num3)
                    {
                        goto Label_005F;
                    }
                    oldestEntryNumber = this.OldestEntryNumber;
                    num3 = this.EntryCount + oldestEntryNumber;
                Label_00D3:
                    if (lastSeenCount < num3)
                    {
                        goto Label_00BF;
                    }
                }
                catch (Exception)
                {
                }
                catch
                {
                }
                try
                {
                    int num4 = this.EntryCount + this.OldestEntryNumber;
                    if (lastSeenCount > num4)
                    {
                        this.lastSeenCount = num4;
                    }
                    else
                    {
                        this.lastSeenCount = lastSeenCount;
                    }
                }
                catch (Win32Exception)
                {
                }
                lock (this)
                {
                    this.boolFlags[1] = false;
                }
            }
        }

        public static void CreateEventSource(EventSourceCreationData sourceData)
        {
            if (sourceData == null)
            {
                throw new ArgumentNullException("sourceData");
            }
            string logName = sourceData.LogName;
            string source = sourceData.Source;
            string machineName = sourceData.MachineName;
            if (!SyntaxCheck.CheckMachineName(machineName))
            {
                throw new ArgumentException(SR.GetString("InvalidParameter", new object[] { "machineName", machineName }));
            }
            if ((logName == null) || (logName.Length == 0))
            {
                logName = "Application";
            }
            if (!ValidLogName(logName, false))
            {
                throw new ArgumentException(SR.GetString("BadLogName"));
            }
            if ((source == null) || (source.Length == 0))
            {
                throw new ArgumentException(SR.GetString("MissingParameter", new object[] { "source" }));
            }
            if ((source.Length + @"SYSTEM\CurrentControlSet\Services\EventLog".Length) > 0xfe)
            {
                throw new ArgumentException(SR.GetString("ParameterTooLong", new object[] { "source", 0xfe - @"SYSTEM\CurrentControlSet\Services\EventLog".Length }));
            }
            new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
            Mutex mutex = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                SharedUtils.EnterMutex("netfxeventlog.1.0", ref mutex);
                if (SourceExists(source, machineName))
                {
                    if (".".Equals(machineName))
                    {
                        throw new ArgumentException(SR.GetString("LocalSourceAlreadyExists", new object[] { source }));
                    }
                    throw new ArgumentException(SR.GetString("SourceAlreadyExists", new object[] { source, machineName }));
                }
                _GetAssertPermSet().Assert();
                RegistryKey localMachine = null;
                RegistryKey keyParent = null;
                RegistryKey logKey = null;
                RegistryKey sourceLogKey = null;
                RegistryKey key5 = null;
                try
                {
                    if (machineName == ".")
                    {
                        localMachine = Registry.LocalMachine;
                    }
                    else
                    {
                        localMachine = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName);
                    }
                    keyParent = localMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\EventLog", true);
                    if (keyParent == null)
                    {
                        if (!".".Equals(machineName))
                        {
                            throw new InvalidOperationException(SR.GetString("RegKeyMissing", new object[] { @"SYSTEM\CurrentControlSet\Services\EventLog", logName, source, machineName }));
                        }
                        throw new InvalidOperationException(SR.GetString("LocalRegKeyMissing", new object[] { @"SYSTEM\CurrentControlSet\Services\EventLog", logName, source }));
                    }
                    logKey = keyParent.OpenSubKey(logName, true);
                    if ((logKey == null) && (logName.Length >= 8))
                    {
                        string strA = logName.Substring(0, 8);
                        if (((string.Compare(strA, "AppEvent", StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare(strA, "SecEvent", StringComparison.OrdinalIgnoreCase) == 0)) || (string.Compare(strA, "SysEvent", StringComparison.OrdinalIgnoreCase) == 0))
                        {
                            throw new ArgumentException(SR.GetString("InvalidCustomerLogName", new object[] { logName }));
                        }
                        string str5 = FindSame8FirstCharsLog(keyParent, logName);
                        if (str5 != null)
                        {
                            throw new ArgumentException(SR.GetString("DuplicateLogName", new object[] { logName, str5 }));
                        }
                    }
                    bool flag = logKey == null;
                    if (flag)
                    {
                        if (SourceExists(logName, machineName))
                        {
                            if (".".Equals(machineName))
                            {
                                throw new ArgumentException(SR.GetString("LocalLogAlreadyExistsAsSource", new object[] { logName }));
                            }
                            throw new ArgumentException(SR.GetString("LogAlreadyExistsAsSource", new object[] { logName, machineName }));
                        }
                        logKey = keyParent.CreateSubKey(logName);
                        if (!SkipRegPatch)
                        {
                            logKey.SetValue("Sources", new string[] { logName, source }, RegistryValueKind.MultiString);
                        }
                        SetSpecialLogRegValues(logKey, logName);
                        sourceLogKey = logKey.CreateSubKey(logName);
                        SetSpecialSourceRegValues(sourceLogKey, sourceData);
                    }
                    if (logName != source)
                    {
                        if (!flag)
                        {
                            SetSpecialLogRegValues(logKey, logName);
                            if (!SkipRegPatch)
                            {
                                string[] array = logKey.GetValue("Sources") as string[];
                                if (array == null)
                                {
                                    logKey.SetValue("Sources", new string[] { logName, source }, RegistryValueKind.MultiString);
                                }
                                else if (Array.IndexOf<string>(array, source) == -1)
                                {
                                    string[] destinationArray = new string[array.Length + 1];
                                    Array.Copy(array, destinationArray, array.Length);
                                    destinationArray[array.Length] = source;
                                    logKey.SetValue("Sources", destinationArray, RegistryValueKind.MultiString);
                                }
                            }
                        }
                        key5 = logKey.CreateSubKey(source);
                        SetSpecialSourceRegValues(key5, sourceData);
                    }
                }
                finally
                {
                    if (localMachine != null)
                    {
                        localMachine.Close();
                    }
                    if (keyParent != null)
                    {
                        keyParent.Close();
                    }
                    if (logKey != null)
                    {
                        logKey.Flush();
                        logKey.Close();
                    }
                    if (sourceLogKey != null)
                    {
                        sourceLogKey.Flush();
                        sourceLogKey.Close();
                    }
                    if (key5 != null)
                    {
                        key5.Flush();
                        key5.Close();
                    }
                    CodeAccessPermission.RevertAssert();
                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                }
            }
        }

        public static void CreateEventSource(string source, string logName)
        {
            CreateEventSource(new EventSourceCreationData(source, logName, "."));
        }

        [Obsolete("This method has been deprecated.  Please use System.Diagnostics.EventLog.CreateEventSource(EventSourceCreationData sourceData) instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public static void CreateEventSource(string source, string logName, string machineName)
        {
            CreateEventSource(new EventSourceCreationData(source, logName, machineName));
        }

        public static void Delete(string logName)
        {
            Delete(logName, ".");
        }

        public static void Delete(string logName, string machineName)
        {
            if (!SyntaxCheck.CheckMachineName(machineName))
            {
                throw new ArgumentException(SR.GetString("InvalidParameterFormat", new object[] { "machineName" }));
            }
            if ((logName == null) || (logName.Length == 0))
            {
                throw new ArgumentException(SR.GetString("NoLogName"));
            }
            if (!ValidLogName(logName, false))
            {
                throw new InvalidOperationException(SR.GetString("BadLogName"));
            }
            new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
            SharedUtils.CheckEnvironment();
            _GetAssertPermSet().Assert();
            RegistryKey eventLogRegKey = null;
            Mutex mutex = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                SharedUtils.EnterMutex("netfxeventlog.1.0", ref mutex);
                try
                {
                    eventLogRegKey = GetEventLogRegKey(machineName, true);
                    if (eventLogRegKey == null)
                    {
                        throw new InvalidOperationException(SR.GetString("RegKeyNoAccess", new object[] { @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog", machineName }));
                    }
                    using (RegistryKey key2 = eventLogRegKey.OpenSubKey(logName))
                    {
                        if (key2 == null)
                        {
                            throw new InvalidOperationException(SR.GetString("MissingLog", new object[] { logName, machineName }));
                        }
                        EventLog log = new EventLog();
                        try
                        {
                            log.Log = logName;
                            log.MachineName = machineName;
                            log.Clear();
                        }
                        finally
                        {
                            log.Close();
                        }
                        string path = null;
                        try
                        {
                            path = (string) key2.GetValue("File");
                        }
                        catch
                        {
                        }
                        if (path != null)
                        {
                            try
                            {
                                File.Delete(path);
                            }
                            catch
                            {
                            }
                        }
                    }
                    eventLogRegKey.DeleteSubKeyTree(logName);
                }
                finally
                {
                    if (eventLogRegKey != null)
                    {
                        eventLogRegKey.Close();
                    }
                    CodeAccessPermission.RevertAssert();
                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        public static void DeleteEventSource(string source)
        {
            DeleteEventSource(source, ".");
        }

        public static void DeleteEventSource(string source, string machineName)
        {
            if (!SyntaxCheck.CheckMachineName(machineName))
            {
                throw new ArgumentException(SR.GetString("InvalidParameter", new object[] { "machineName", machineName }));
            }
            new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
            SharedUtils.CheckEnvironment();
            _GetAssertPermSet().Assert();
            Mutex mutex = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                SharedUtils.EnterMutex("netfxeventlog.1.0", ref mutex);
                RegistryKey key = null;
                using (key = FindSourceRegistration(source, machineName, true))
                {
                    if (key == null)
                    {
                        if (machineName == null)
                        {
                            throw new ArgumentException(SR.GetString("LocalSourceNotRegistered", new object[] { source }));
                        }
                        throw new ArgumentException(SR.GetString("SourceNotRegistered", new object[] { source, machineName, @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog" }));
                    }
                    string name = key.Name;
                    int num = name.LastIndexOf('\\');
                    if (string.Compare(name, num + 1, source, 0, name.Length - num, StringComparison.Ordinal) == 0)
                    {
                        throw new InvalidOperationException(SR.GetString("CannotDeleteEqualSource", new object[] { source }));
                    }
                }
                try
                {
                    key = FindSourceRegistration(source, machineName, false);
                    key.DeleteSubKeyTree(source);
                    if (!SkipRegPatch)
                    {
                        string[] strArray = (string[]) key.GetValue("Sources");
                        ArrayList list = new ArrayList(strArray.Length - 1);
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            if (strArray[i] != source)
                            {
                                list.Add(strArray[i]);
                            }
                        }
                        string[] array = new string[list.Count];
                        list.CopyTo(array);
                        key.SetValue("Sources", array, RegistryValueKind.MultiString);
                    }
                }
                finally
                {
                    if (key != null)
                    {
                        key.Flush();
                        key.Close();
                    }
                    CodeAccessPermission.RevertAssert();
                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.IsOpen)
                {
                    this.Close();
                }
            }
            else
            {
                if (this.readHandle != null)
                {
                    this.readHandle.Close();
                }
                if (this.writeHandle != null)
                {
                    this.writeHandle.Close();
                }
                this.messageLibraries = null;
            }
            this.boolFlags[0x100] = true;
            base.Dispose(disposing);
        }

        public void EndInit()
        {
            string machineName = this.machineName;
            new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
            this.boolFlags[4] = false;
            if (this.boolFlags[8])
            {
                this.StartListening(machineName, this.GetLogName(machineName));
            }
        }

        public static bool Exists(string logName) => 
            Exists(logName, ".");

        public static bool Exists(string logName, string machineName)
        {
            bool flag;
            if (!SyntaxCheck.CheckMachineName(machineName))
            {
                throw new ArgumentException(SR.GetString("InvalidParameterFormat", new object[] { "machineName" }));
            }
            new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
            if ((logName == null) || (logName.Length == 0))
            {
                return false;
            }
            SharedUtils.CheckEnvironment();
            _GetAssertPermSet().Assert();
            RegistryKey eventLogRegKey = null;
            RegistryKey key2 = null;
            try
            {
                eventLogRegKey = GetEventLogRegKey(machineName, false);
                if (eventLogRegKey == null)
                {
                    return false;
                }
                key2 = eventLogRegKey.OpenSubKey(logName, false);
                flag = key2 != null;
            }
            finally
            {
                if (eventLogRegKey != null)
                {
                    eventLogRegKey.Close();
                }
                if (key2 != null)
                {
                    key2.Close();
                }
                CodeAccessPermission.RevertAssert();
            }
            return flag;
        }

        private static string FindSame8FirstCharsLog(RegistryKey keyParent, string logName)
        {
            string strB = logName.Substring(0, 8);
            foreach (string str2 in keyParent.GetSubKeyNames())
            {
                if ((str2.Length >= 8) && (string.Compare(str2.Substring(0, 8), strB, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    return str2;
                }
            }
            return null;
        }

        private static RegistryKey FindSourceRegistration(string source, string machineName, bool readOnly)
        {
            if ((source != null) && (source.Length != 0))
            {
                SharedUtils.CheckEnvironment();
                _GetAssertPermSet().Assert();
                RegistryKey eventLogRegKey = null;
                try
                {
                    eventLogRegKey = GetEventLogRegKey(machineName, !readOnly);
                    if (eventLogRegKey == null)
                    {
                        return null;
                    }
                    StringBuilder builder = null;
                    string[] subKeyNames = eventLogRegKey.GetSubKeyNames();
                    for (int i = 0; i < subKeyNames.Length; i++)
                    {
                        RegistryKey key2 = null;
                        try
                        {
                            RegistryKey key3 = eventLogRegKey.OpenSubKey(subKeyNames[i], !readOnly);
                            if (key3 != null)
                            {
                                key2 = key3.OpenSubKey(source, !readOnly);
                                if (key2 != null)
                                {
                                    return key3;
                                }
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(subKeyNames[i]);
                            }
                            else
                            {
                                builder.Append(", ");
                                builder.Append(subKeyNames[i]);
                            }
                        }
                        catch (SecurityException)
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(subKeyNames[i]);
                            }
                            else
                            {
                                builder.Append(", ");
                                builder.Append(subKeyNames[i]);
                            }
                        }
                        finally
                        {
                            if (key2 != null)
                            {
                                key2.Close();
                            }
                        }
                    }
                    if (builder != null)
                    {
                        throw new SecurityException(SR.GetString("SomeLogsInaccessible", new object[] { builder.ToString() }));
                    }
                }
                finally
                {
                    if (eventLogRegKey != null)
                    {
                        eventLogRegKey.Close();
                    }
                    CodeAccessPermission.RevertAssert();
                }
            }
            return null;
        }

        private static string FixupPath(string path)
        {
            if (path[0] == '%')
            {
                return path;
            }
            return Path.GetFullPath(path);
        }

        internal string FormatMessageWrapper(string dllNameList, uint messageNum, string[] insertionStrings)
        {
            if (dllNameList != null)
            {
                if (insertionStrings == null)
                {
                    insertionStrings = new string[0];
                }
                foreach (string str in dllNameList.Split(new char[] { ';' }))
                {
                    if ((str != null) && (str.Length != 0))
                    {
                        SafeLibraryHandle hModule = null;
                        if (this.IsOpen)
                        {
                            hModule = this.MessageLibraries[str] as SafeLibraryHandle;
                            if ((hModule == null) || hModule.IsInvalid)
                            {
                                hModule = SafeLibraryHandle.LoadLibraryEx(str, IntPtr.Zero, 2);
                                this.MessageLibraries[str] = hModule;
                            }
                        }
                        else
                        {
                            hModule = SafeLibraryHandle.LoadLibraryEx(str, IntPtr.Zero, 2);
                        }
                        if (!hModule.IsInvalid)
                        {
                            string str2 = null;
                            try
                            {
                                str2 = TryFormatMessage(hModule, messageNum, insertionStrings);
                            }
                            finally
                            {
                                if (!this.IsOpen)
                                {
                                    hModule.Close();
                                }
                            }
                            if (str2 != null)
                            {
                                return str2;
                            }
                        }
                    }
                }
            }
            return null;
        }

        internal EventLogEntry[] GetAllEntries()
        {
            string machineName = this.machineName;
            if (!this.IsOpenForRead)
            {
                this.OpenForRead(machineName);
            }
            EventLogEntry[] entryArray = new EventLogEntry[this.EntryCount];
            int index = 0;
            int oldestEntryNumber = this.OldestEntryNumber;
            int[] bytesRead = new int[1];
            int[] minNumOfBytesNeeded = new int[] { 0x9c40 };
            int error = 0;
            while (index < entryArray.Length)
            {
                byte[] buffer = new byte[0x9c40];
                if (!Microsoft.Win32.UnsafeNativeMethods.ReadEventLog(this.readHandle, 6, oldestEntryNumber + index, buffer, buffer.Length, bytesRead, minNumOfBytesNeeded))
                {
                    error = Marshal.GetLastWin32Error();
                    if ((error != 0x7a) && (error != 0x5df))
                    {
                        break;
                    }
                    if (error == 0x5df)
                    {
                        this.Reset(machineName);
                    }
                    else if (minNumOfBytesNeeded[0] > buffer.Length)
                    {
                        buffer = new byte[minNumOfBytesNeeded[0]];
                    }
                    if (!Microsoft.Win32.UnsafeNativeMethods.ReadEventLog(this.readHandle, 6, oldestEntryNumber + index, buffer, buffer.Length, bytesRead, minNumOfBytesNeeded))
                    {
                        break;
                    }
                    error = 0;
                }
                entryArray[index] = new EventLogEntry(buffer, 0, this);
                int offset = IntFrom(buffer, 0);
                index++;
                while ((offset < bytesRead[0]) && (index < entryArray.Length))
                {
                    entryArray[index] = new EventLogEntry(buffer, offset, this);
                    offset += IntFrom(buffer, offset);
                    index++;
                }
            }
            if (index == entryArray.Length)
            {
                return entryArray;
            }
            if (error != 0)
            {
                throw new InvalidOperationException(SR.GetString("CantRetrieveEntries"), SharedUtils.CreateSafeWin32Exception(error));
            }
            throw new InvalidOperationException(SR.GetString("CantRetrieveEntries"));
        }

        private int GetCachedEntryPos(int entryIndex)
        {
            if ((((this.cache != null) && (!this.boolFlags[2] || (entryIndex >= this.firstCachedEntry))) && (this.boolFlags[2] || (entryIndex <= this.firstCachedEntry))) && (this.firstCachedEntry != -1))
            {
            Label_009A:
                if (this.lastSeenEntry < entryIndex)
                {
                    this.lastSeenEntry++;
                    if (this.boolFlags[2])
                    {
                        this.lastSeenPos = this.GetNextEntryPos(this.lastSeenPos);
                        if (this.lastSeenPos < this.bytesCached)
                        {
                            goto Label_009A;
                        }
                        goto Label_00FE;
                    }
                    this.lastSeenPos = this.GetPreviousEntryPos(this.lastSeenPos);
                    if (this.lastSeenPos < 0)
                    {
                        goto Label_00FE;
                    }
                    goto Label_009A;
                }
            }
            else
            {
                return -1;
            }
        Label_00FE:
            while (this.lastSeenEntry > entryIndex)
            {
                this.lastSeenEntry--;
                if (this.boolFlags[2])
                {
                    this.lastSeenPos = this.GetPreviousEntryPos(this.lastSeenPos);
                    if (this.lastSeenPos >= 0)
                    {
                        continue;
                    }
                    break;
                }
                this.lastSeenPos = this.GetNextEntryPos(this.lastSeenPos);
                if (this.lastSeenPos >= this.bytesCached)
                {
                    break;
                }
            }
            if (this.lastSeenPos >= this.bytesCached)
            {
                this.lastSeenPos = this.GetPreviousEntryPos(this.lastSeenPos);
                if (this.boolFlags[2])
                {
                    this.lastSeenEntry--;
                }
                else
                {
                    this.lastSeenEntry++;
                }
                return -1;
            }
            if (this.lastSeenPos >= 0)
            {
                return this.lastSeenPos;
            }
            this.lastSeenPos = 0;
            if (this.boolFlags[2])
            {
                this.lastSeenEntry++;
            }
            else
            {
                this.lastSeenEntry--;
            }
            return -1;
        }

        internal static string GetDllPath(string machineName) => 
            (SharedUtils.GetLatestBuildDllDirectory(machineName) + @"\EventLogMessages.dll");

        internal EventLogEntry GetEntryAt(int index)
        {
            EventLogEntry entryAtNoThrow = this.GetEntryAtNoThrow(index);
            if (entryAtNoThrow == null)
            {
                throw new ArgumentException(SR.GetString("IndexOutOfBounds", new object[] { index.ToString(CultureInfo.CurrentCulture) }));
            }
            return entryAtNoThrow;
        }

        internal EventLogEntry GetEntryAtNoThrow(int index)
        {
            if (!this.IsOpenForRead)
            {
                this.OpenForRead(this.machineName);
            }
            if ((index < 0) || (index >= this.EntryCount))
            {
                return null;
            }
            index += this.OldestEntryNumber;
            return this.GetEntryWithOldest(index);
        }

        private EventLogEntry GetEntryWithOldest(int index)
        {
            int cachedEntryPos = this.GetCachedEntryPos(index);
            if (cachedEntryPos >= 0)
            {
                return new EventLogEntry(this.cache, cachedEntryPos, this);
            }
            string machineName = this.machineName;
            int dwReadFlags = 0;
            if (this.GetCachedEntryPos(index + 1) < 0)
            {
                dwReadFlags = 6;
                this.boolFlags[2] = true;
            }
            else
            {
                dwReadFlags = 10;
                this.boolFlags[2] = false;
            }
            this.cache = new byte[0x9c40];
            int[] bytesRead = new int[1];
            int[] minNumOfBytesNeeded = new int[] { this.cache.Length };
            bool flag = Microsoft.Win32.UnsafeNativeMethods.ReadEventLog(this.readHandle, dwReadFlags, index, this.cache, this.cache.Length, bytesRead, minNumOfBytesNeeded);
            if (!flag)
            {
                int num3 = Marshal.GetLastWin32Error();
                switch (num3)
                {
                    case 0x7a:
                    case 0x5df:
                        if (num3 == 0x5df)
                        {
                            byte[] cache = this.cache;
                            this.Reset(machineName);
                            this.cache = cache;
                        }
                        else if (minNumOfBytesNeeded[0] > this.cache.Length)
                        {
                            this.cache = new byte[minNumOfBytesNeeded[0]];
                        }
                        flag = Microsoft.Win32.UnsafeNativeMethods.ReadEventLog(this.readHandle, 6, index, this.cache, this.cache.Length, bytesRead, minNumOfBytesNeeded);
                        break;
                }
                if (!flag)
                {
                    throw new InvalidOperationException(SR.GetString("CantReadLogEntryAt", new object[] { index.ToString(CultureInfo.CurrentCulture) }), SharedUtils.CreateSafeWin32Exception());
                }
            }
            this.bytesCached = bytesRead[0];
            this.firstCachedEntry = index;
            this.lastSeenEntry = index;
            this.lastSeenPos = 0;
            return new EventLogEntry(this.cache, 0, this);
        }

        internal static RegistryKey GetEventLogRegKey(string machine, bool writable)
        {
            RegistryKey localMachine = null;
            try
            {
                if (machine.Equals("."))
                {
                    localMachine = Registry.LocalMachine;
                }
                else
                {
                    localMachine = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machine);
                }
                if (localMachine != null)
                {
                    return localMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\EventLog", writable);
                }
            }
            finally
            {
                if (localMachine != null)
                {
                    localMachine.Close();
                }
            }
            return null;
        }

        public static EventLog[] GetEventLogs() => 
            GetEventLogs(".");

        public static EventLog[] GetEventLogs(string machineName)
        {
            if (!SyntaxCheck.CheckMachineName(machineName))
            {
                throw new ArgumentException(SR.GetString("InvalidParameter", new object[] { "machineName", machineName }));
            }
            new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
            SharedUtils.CheckEnvironment();
            string[] subKeyNames = new string[0];
            _GetAssertPermSet().Assert();
            RegistryKey eventLogRegKey = null;
            try
            {
                eventLogRegKey = GetEventLogRegKey(machineName, false);
                if (eventLogRegKey == null)
                {
                    throw new InvalidOperationException(SR.GetString("RegKeyMissingShort", new object[] { @"SYSTEM\CurrentControlSet\Services\EventLog", machineName }));
                }
                subKeyNames = eventLogRegKey.GetSubKeyNames();
            }
            finally
            {
                if (eventLogRegKey != null)
                {
                    eventLogRegKey.Close();
                }
                CodeAccessPermission.RevertAssert();
            }
            EventLog[] logArray = new EventLog[subKeyNames.Length];
            for (int i = 0; i < subKeyNames.Length; i++)
            {
                logArray[i] = new EventLog { 
                    Log = subKeyNames[i],
                    MachineName = machineName
                };
            }
            return logArray;
        }

        private string GetLogName(string currentMachineName)
        {
            if (((this.logName == null) || (this.logName.Length == 0)) && ((this.sourceName != null) && (this.sourceName.Length != 0)))
            {
                this.logName = LogNameFromSourceName(this.sourceName, currentMachineName);
            }
            return this.logName;
        }

        private RegistryKey GetLogRegKey(string currentMachineName, bool writable)
        {
            string logName = this.GetLogName(currentMachineName);
            if (!ValidLogName(logName, false))
            {
                throw new InvalidOperationException(SR.GetString("BadLogName"));
            }
            RegistryKey eventLogRegKey = null;
            RegistryKey key2 = null;
            try
            {
                eventLogRegKey = GetEventLogRegKey(currentMachineName, false);
                if (eventLogRegKey == null)
                {
                    throw new InvalidOperationException(SR.GetString("RegKeyMissingShort", new object[] { @"SYSTEM\CurrentControlSet\Services\EventLog", currentMachineName }));
                }
                key2 = eventLogRegKey.OpenSubKey(logName, writable);
                if (key2 == null)
                {
                    throw new InvalidOperationException(SR.GetString("MissingLog", new object[] { logName, currentMachineName }));
                }
            }
            finally
            {
                if (eventLogRegKey != null)
                {
                    eventLogRegKey.Close();
                }
            }
            return key2;
        }

        private object GetLogRegValue(string currentMachineName, string valuename)
        {
            object obj3;
            _GetAssertPermSet().Assert();
            RegistryKey logRegKey = null;
            try
            {
                logRegKey = this.GetLogRegKey(currentMachineName, false);
                if (logRegKey == null)
                {
                    throw new InvalidOperationException(SR.GetString("MissingLog", new object[] { this.GetLogName(currentMachineName), currentMachineName }));
                }
                obj3 = logRegKey.GetValue(valuename);
            }
            finally
            {
                if (logRegKey != null)
                {
                    logRegKey.Close();
                }
                CodeAccessPermission.RevertAssert();
            }
            return obj3;
        }

        private int GetNextEntryPos(int pos) => 
            (pos + IntFrom(this.cache, pos));

        private int GetPreviousEntryPos(int pos) => 
            (pos - IntFrom(this.cache, pos - 4));

        private void InternalWriteEvent(uint eventID, ushort category, EventLogEntryType type, string[] strings, byte[] rawData, string currentMachineName)
        {
            if (strings == null)
            {
                strings = new string[0];
            }
            if (strings.Length >= 0x100)
            {
                throw new ArgumentException(SR.GetString("TooManyReplacementStrings"));
            }
            for (int i = 0; i < strings.Length; i++)
            {
                if (strings[i] == null)
                {
                    strings[i] = string.Empty;
                }
                if (strings[i].Length > 0x7ffe)
                {
                    throw new ArgumentException(SR.GetString("LogEntryTooLong"));
                }
            }
            if (rawData == null)
            {
                rawData = new byte[0];
            }
            if (this.Source.Length == 0)
            {
                throw new ArgumentException(SR.GetString("NeedSourceToWrite"));
            }
            if (!this.IsOpenForWrite)
            {
                this.OpenForWrite(currentMachineName);
            }
            IntPtr[] ptrArray = new IntPtr[strings.Length];
            GCHandle[] handleArray = new GCHandle[strings.Length];
            GCHandle handle = GCHandle.Alloc(ptrArray, GCHandleType.Pinned);
            try
            {
                for (int j = 0; j < strings.Length; j++)
                {
                    handleArray[j] = GCHandle.Alloc(strings[j], GCHandleType.Pinned);
                    ptrArray[j] = handleArray[j].AddrOfPinnedObject();
                }
                byte[] userSID = null;
                if (!Microsoft.Win32.UnsafeNativeMethods.ReportEvent(this.writeHandle, (short) type, category, eventID, userSID, (short) strings.Length, rawData.Length, new HandleRef(this, handle.AddrOfPinnedObject()), rawData))
                {
                    throw SharedUtils.CreateSafeWin32Exception();
                }
            }
            finally
            {
                for (int k = 0; k < strings.Length; k++)
                {
                    if (handleArray[k].IsAllocated)
                    {
                        handleArray[k].Free();
                    }
                }
                handle.Free();
            }
        }

        private static int IntFrom(byte[] buf, int offset) => 
            ((((-16777216 & (buf[offset + 3] << 0x18)) | (0xff0000 & (buf[offset + 2] << 0x10))) | (0xff00 & (buf[offset + 1] << 8))) | (0xff & buf[offset]));

        public static string LogNameFromSourceName(string source, string machineName)
        {
            new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
            using (RegistryKey key = FindSourceRegistration(source, machineName, true))
            {
                if (key == null)
                {
                    return "";
                }
                string name = key.Name;
                int num = name.LastIndexOf('\\');
                return name.Substring(num + 1);
            }
        }

        [ComVisible(false)]
        public void ModifyOverflowPolicy(System.Diagnostics.OverflowAction action, int retentionDays)
        {
            string machineName = this.machineName;
            new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
            if ((action < System.Diagnostics.OverflowAction.DoNotOverwrite) || (action > System.Diagnostics.OverflowAction.OverwriteOlder))
            {
                throw new InvalidEnumArgumentException("action", (int) action, typeof(System.Diagnostics.OverflowAction));
            }
            long num = (long) action;
            if (action == System.Diagnostics.OverflowAction.OverwriteOlder)
            {
                if ((retentionDays < 1) || (retentionDays > 0x16d))
                {
                    throw new ArgumentOutOfRangeException(SR.GetString("RentionDaysOutOfRange"));
                }
                num = retentionDays * 0x15180L;
            }
            _GetAssertPermSet().Assert();
            using (RegistryKey key = this.GetLogRegKey(machineName, true))
            {
                key.SetValue("Retention", num, RegistryValueKind.DWord);
            }
        }

        private void OpenForRead(string currentMachineName)
        {
            if (this.boolFlags[0x100])
            {
                throw new ObjectDisposedException(base.GetType().Name);
            }
            string logName = this.GetLogName(currentMachineName);
            if ((logName == null) || (logName.Length == 0))
            {
                throw new ArgumentException(SR.GetString("MissingLogProperty"));
            }
            if (!Exists(logName, currentMachineName))
            {
                throw new InvalidOperationException(SR.GetString("LogDoesNotExists", new object[] { logName, currentMachineName }));
            }
            SharedUtils.CheckEnvironment();
            this.lastSeenEntry = 0;
            this.lastSeenPos = 0;
            this.bytesCached = 0;
            this.firstCachedEntry = -1;
            this.readHandle = SafeEventLogReadHandle.OpenEventLog(currentMachineName, logName);
            if (this.readHandle.IsInvalid)
            {
                Win32Exception innerException = null;
                if (Marshal.GetLastWin32Error() != 0)
                {
                    innerException = SharedUtils.CreateSafeWin32Exception();
                }
                throw new InvalidOperationException(SR.GetString("CantOpenLog", new object[] { logName.ToString(), currentMachineName }), innerException);
            }
        }

        private void OpenForWrite(string currentMachineName)
        {
            if (this.boolFlags[0x100])
            {
                throw new ObjectDisposedException(base.GetType().Name);
            }
            if ((this.sourceName == null) || (this.sourceName.Length == 0))
            {
                throw new ArgumentException(SR.GetString("NeedSourceToOpen"));
            }
            SharedUtils.CheckEnvironment();
            this.writeHandle = SafeEventLogWriteHandle.RegisterEventSource(currentMachineName, this.sourceName);
            if (this.writeHandle.IsInvalid)
            {
                Win32Exception innerException = null;
                if (Marshal.GetLastWin32Error() != 0)
                {
                    innerException = SharedUtils.CreateSafeWin32Exception();
                }
                throw new InvalidOperationException(SR.GetString("CantOpenLogAccess", new object[] { this.sourceName }), innerException);
            }
        }

        [ComVisible(false)]
        public void RegisterDisplayName(string resourceFile, long resourceId)
        {
            string machineName = this.machineName;
            new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
            _GetAssertPermSet().Assert();
            using (RegistryKey key = this.GetLogRegKey(machineName, true))
            {
                key.SetValue("DisplayNameFile", resourceFile, RegistryValueKind.ExpandString);
                key.SetValue("DisplayNameID", resourceId, RegistryValueKind.DWord);
            }
        }

        [HostProtection(SecurityAction.LinkDemand, Synchronization=true)]
        private static void RemoveListenerComponent(EventLog component, string compLogName)
        {
            lock (InternalSyncObject)
            {
                LogListeningInfo info = (LogListeningInfo) listenerInfos[compLogName];
                info.listeningComponents.Remove(component);
                if (info.listeningComponents.Count == 0)
                {
                    info.handleOwner.Dispose();
                    info.registeredWaitHandle.Unregister(info.waitHandle);
                    info.waitHandle.Close();
                    listenerInfos[compLogName] = null;
                }
            }
        }

        private void Reset(string currentMachineName)
        {
            bool isOpenForRead = this.IsOpenForRead;
            bool isOpenForWrite = this.IsOpenForWrite;
            bool flag3 = this.boolFlags[8];
            bool flag4 = this.boolFlags[0x10];
            this.Close(currentMachineName);
            this.cache = null;
            if (isOpenForRead)
            {
                this.OpenForRead(currentMachineName);
            }
            if (isOpenForWrite)
            {
                this.OpenForWrite(currentMachineName);
            }
            if (flag4)
            {
                this.StartListening(currentMachineName, this.GetLogName(currentMachineName));
            }
            this.boolFlags[8] = flag3;
        }

        private void SetLogName(string currentMachineName, string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (!ValidLogName(value, true))
            {
                throw new ArgumentException(SR.GetString("BadLogName"));
            }
            new EventLogPermission(EventLogPermissionAccess.Write, currentMachineName).Demand();
            if (value == null)
            {
                value = string.Empty;
            }
            if (this.logName == null)
            {
                this.logName = value;
            }
            else if (string.Compare(this.logName, value, StringComparison.OrdinalIgnoreCase) != 0)
            {
                this.logDisplayName = null;
                this.logName = value;
                if (this.IsOpen)
                {
                    bool enableRaisingEvents = this.EnableRaisingEvents;
                    this.Close(currentMachineName);
                    this.EnableRaisingEvents = enableRaisingEvents;
                }
            }
        }

        private static void SetSpecialLogRegValues(RegistryKey logKey, string logName)
        {
            if (logKey.GetValue("MaxSize") == null)
            {
                logKey.SetValue("MaxSize", 0x80000, RegistryValueKind.DWord);
            }
            if (logKey.GetValue("AutoBackupLogFiles") == null)
            {
                logKey.SetValue("AutoBackupLogFiles", 0, RegistryValueKind.DWord);
            }
            if (!SkipRegPatch)
            {
                if (logKey.GetValue("Retention") == null)
                {
                    logKey.SetValue("Retention", 0x93a80, RegistryValueKind.DWord);
                }
                if (logKey.GetValue("File") == null)
                {
                    string str;
                    if (logName.Length > 8)
                    {
                        str = @"%SystemRoot%\System32\config\" + logName.Substring(0, 8) + ".evt";
                    }
                    else
                    {
                        str = @"%SystemRoot%\System32\config\" + logName + ".evt";
                    }
                    logKey.SetValue("File", str, RegistryValueKind.ExpandString);
                }
            }
        }

        private static void SetSpecialSourceRegValues(RegistryKey sourceLogKey, EventSourceCreationData sourceData)
        {
            if (string.IsNullOrEmpty(sourceData.MessageResourceFile))
            {
                sourceLogKey.SetValue("EventMessageFile", GetDllPath(sourceData.MachineName), RegistryValueKind.ExpandString);
            }
            else
            {
                sourceLogKey.SetValue("EventMessageFile", FixupPath(sourceData.MessageResourceFile), RegistryValueKind.ExpandString);
            }
            if (!string.IsNullOrEmpty(sourceData.ParameterResourceFile))
            {
                sourceLogKey.SetValue("ParameterMessageFile", FixupPath(sourceData.ParameterResourceFile), RegistryValueKind.ExpandString);
            }
            if (!string.IsNullOrEmpty(sourceData.CategoryResourceFile))
            {
                sourceLogKey.SetValue("CategoryMessageFile", FixupPath(sourceData.CategoryResourceFile), RegistryValueKind.ExpandString);
                sourceLogKey.SetValue("CategoryCount", sourceData.CategoryCount, RegistryValueKind.DWord);
            }
        }

        public static bool SourceExists(string source) => 
            SourceExists(source, ".");

        public static bool SourceExists(string source, string machineName)
        {
            if (!SyntaxCheck.CheckMachineName(machineName))
            {
                throw new ArgumentException(SR.GetString("InvalidParameter", new object[] { "machineName", machineName }));
            }
            new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
            using (RegistryKey key = FindSourceRegistration(source, machineName, true))
            {
                return (key != null);
            }
        }

        [HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        private void StartListening(string currentMachineName, string currentLogName)
        {
            this.lastSeenCount = this.EntryCount + this.OldestEntryNumber;
            AddListenerComponent(this, currentMachineName, currentLogName);
            this.boolFlags[0x10] = true;
        }

        private void StartRaisingEvents(string currentMachineName, string currentLogName)
        {
            if ((!this.boolFlags[4] && !this.boolFlags[8]) && !base.DesignMode)
            {
                this.StartListening(currentMachineName, currentLogName);
            }
            this.boolFlags[8] = true;
        }

        private static void StaticCompletionCallback(object context, bool wasSignaled)
        {
            LogListeningInfo info = (LogListeningInfo) context;
            EventLog[] logArray = (EventLog[]) info.listeningComponents.ToArray(typeof(EventLog));
            for (int i = 0; i < logArray.Length; i++)
            {
                try
                {
                    if (logArray[i] != null)
                    {
                        logArray[i].CompletionCallback(null);
                    }
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        [HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        private void StopListening(string currentLogName)
        {
            RemoveListenerComponent(this, currentLogName);
            this.boolFlags[0x10] = false;
        }

        private void StopRaisingEvents(string currentLogName)
        {
            if ((!this.boolFlags[4] && this.boolFlags[8]) && !base.DesignMode)
            {
                this.StopListening(currentLogName);
            }
            this.boolFlags[8] = false;
        }

        internal static string TryFormatMessage(SafeLibraryHandle hModule, uint messageNum, string[] insertionStrings)
        {
            string str = null;
            int num = 0;
            StringBuilder lpBuffer = new StringBuilder(0x400);
            int dwFlags = 0x2800;
            IntPtr[] ptrArray = new IntPtr[insertionStrings.Length];
            GCHandle[] handleArray = new GCHandle[insertionStrings.Length];
            GCHandle handle = GCHandle.Alloc(ptrArray, GCHandleType.Pinned);
            if (insertionStrings.Length == 0)
            {
                dwFlags |= 0x200;
            }
            try
            {
                for (int i = 0; i < handleArray.Length; i++)
                {
                    handleArray[i] = GCHandle.Alloc(insertionStrings[i], GCHandleType.Pinned);
                    ptrArray[i] = handleArray[i].AddrOfPinnedObject();
                }
                int num4 = 0x7a;
                while ((num == 0) && (num4 == 0x7a))
                {
                    num = Microsoft.Win32.SafeNativeMethods.FormatMessage(dwFlags, hModule, messageNum, 0, lpBuffer, lpBuffer.Capacity, ptrArray);
                    if (num == 0)
                    {
                        num4 = Marshal.GetLastWin32Error();
                        if (num4 == 0x7a)
                        {
                            lpBuffer.Capacity *= 2;
                        }
                    }
                }
            }
            catch
            {
                num = 0;
            }
            finally
            {
                for (int j = 0; j < handleArray.Length; j++)
                {
                    if (handleArray[j].IsAllocated)
                    {
                        handleArray[j].Free();
                    }
                }
                handle.Free();
            }
            if (num > 0)
            {
                str = lpBuffer.ToString();
                if ((str.Length > 1) && (str[str.Length - 1] == '\n'))
                {
                    str = str.Substring(0, str.Length - 2);
                }
            }
            return str;
        }

        internal static bool ValidLogName(string logName, bool ignoreEmpty)
        {
            if ((logName.Length == 0) && !ignoreEmpty)
            {
                return false;
            }
            foreach (char ch in logName)
            {
                if ((!CharIsPrintable(ch) || (ch == '\\')) || ((ch == '*') || (ch == '?')))
                {
                    return false;
                }
            }
            return true;
        }

        private void VerifyAndCreateSource(string sourceName, string currentMachineName)
        {
            if (!this.boolFlags[0x200])
            {
                if (!SourceExists(sourceName, currentMachineName))
                {
                    Mutex mutex = null;
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                        SharedUtils.EnterMutex("netfxeventlog.1.0", ref mutex);
                        if (!SourceExists(sourceName, currentMachineName))
                        {
                            if (this.GetLogName(currentMachineName) == null)
                            {
                                this.SetLogName(currentMachineName, "Application");
                            }
                            CreateEventSource(new EventSourceCreationData(sourceName, this.GetLogName(currentMachineName), currentMachineName));
                            this.Reset(currentMachineName);
                        }
                        else
                        {
                            string strA = LogNameFromSourceName(sourceName, currentMachineName);
                            string logName = this.GetLogName(currentMachineName);
                            if (((strA != null) && (logName != null)) && (string.Compare(strA, logName, StringComparison.OrdinalIgnoreCase) != 0))
                            {
                                throw new ArgumentException(SR.GetString("LogSourceMismatch", new object[] { this.Source.ToString(), logName, strA }));
                            }
                        }
                    }
                    finally
                    {
                        if (mutex != null)
                        {
                            mutex.ReleaseMutex();
                            mutex.Close();
                        }
                    }
                }
                else
                {
                    string str3 = LogNameFromSourceName(sourceName, currentMachineName);
                    string strB = this.GetLogName(currentMachineName);
                    if (((str3 != null) && (strB != null)) && (string.Compare(str3, strB, StringComparison.OrdinalIgnoreCase) != 0))
                    {
                        throw new ArgumentException(SR.GetString("LogSourceMismatch", new object[] { this.Source.ToString(), strB, str3 }));
                    }
                }
                this.boolFlags[0x200] = true;
            }
        }

        public void WriteEntry(string message)
        {
            this.WriteEntry(message, EventLogEntryType.Information, 0, 0, null);
        }

        public void WriteEntry(string message, EventLogEntryType type)
        {
            this.WriteEntry(message, type, 0, 0, null);
        }

        public static void WriteEntry(string source, string message)
        {
            WriteEntry(source, message, EventLogEntryType.Information, 0, 0, null);
        }

        public void WriteEntry(string message, EventLogEntryType type, int eventID)
        {
            this.WriteEntry(message, type, eventID, 0, null);
        }

        public static void WriteEntry(string source, string message, EventLogEntryType type)
        {
            WriteEntry(source, message, type, 0, 0, null);
        }

        public void WriteEntry(string message, EventLogEntryType type, int eventID, short category)
        {
            this.WriteEntry(message, type, eventID, category, null);
        }

        public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID)
        {
            WriteEntry(source, message, type, eventID, 0, null);
        }

        public void WriteEntry(string message, EventLogEntryType type, int eventID, short category, byte[] rawData)
        {
            if ((eventID < 0) || (eventID > 0xffff))
            {
                throw new ArgumentException(SR.GetString("EventID", new object[] { eventID, 0, 0xffff }));
            }
            if (this.Source.Length == 0)
            {
                throw new ArgumentException(SR.GetString("NeedSourceToWrite"));
            }
            if (!Enum.IsDefined(typeof(EventLogEntryType), type))
            {
                throw new InvalidEnumArgumentException("type", (int) type, typeof(EventLogEntryType));
            }
            string machineName = this.machineName;
            if (!this.boolFlags[0x20])
            {
                new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
                this.boolFlags[0x20] = true;
            }
            this.VerifyAndCreateSource(this.sourceName, machineName);
            this.InternalWriteEvent((uint) eventID, (ushort) category, type, new string[] { message }, rawData, machineName);
        }

        public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID, short category)
        {
            WriteEntry(source, message, type, eventID, category, null);
        }

        public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID, short category, byte[] rawData)
        {
            EventLog log = new EventLog();
            try
            {
                log.Source = source;
                log.WriteEntry(message, type, eventID, category, rawData);
            }
            finally
            {
                log.Dispose(true);
            }
        }

        [ComVisible(false)]
        public void WriteEvent(EventInstance instance, params object[] values)
        {
            this.WriteEvent(instance, null, values);
        }

        [ComVisible(false)]
        public void WriteEvent(EventInstance instance, byte[] data, params object[] values)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (this.Source.Length == 0)
            {
                throw new ArgumentException(SR.GetString("NeedSourceToWrite"));
            }
            string machineName = this.machineName;
            if (!this.boolFlags[0x20])
            {
                new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
                this.boolFlags[0x20] = true;
            }
            this.VerifyAndCreateSource(this.Source, machineName);
            string[] strings = null;
            if (values != null)
            {
                strings = new string[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] != null)
                    {
                        strings[i] = values[i].ToString();
                    }
                    else
                    {
                        strings[i] = string.Empty;
                    }
                }
            }
            this.InternalWriteEvent((uint) instance.InstanceId, (ushort) instance.CategoryId, instance.EntryType, strings, data, machineName);
        }

        public static void WriteEvent(string source, EventInstance instance, params object[] values)
        {
            using (EventLog log = new EventLog())
            {
                log.Source = source;
                log.WriteEvent(instance, null, values);
            }
        }

        public static void WriteEvent(string source, EventInstance instance, byte[] data, params object[] values)
        {
            using (EventLog log = new EventLog())
            {
                log.Source = source;
                log.WriteEvent(instance, data, values);
            }
        }

        [Browsable(false), MonitoringDescription("LogMonitoring"), DefaultValue(false)]
        public bool EnableRaisingEvents
        {
            get
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
                return this.boolFlags[8];
            }
            set
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
                if (base.DesignMode)
                {
                    this.boolFlags[8] = value;
                }
                else if (value)
                {
                    this.StartRaisingEvents(machineName, this.GetLogName(machineName));
                }
                else
                {
                    this.StopRaisingEvents(this.GetLogName(machineName));
                }
            }
        }

        [MonitoringDescription("LogEntries"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public EventLogEntryCollection Entries
        {
            get
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
                if (this.entriesCollection == null)
                {
                    this.entriesCollection = new EventLogEntryCollection(this);
                }
                return this.entriesCollection;
            }
        }

        internal int EntryCount
        {
            get
            {
                int num;
                if (!this.IsOpenForRead)
                {
                    this.OpenForRead(this.machineName);
                }
                if (!Microsoft.Win32.UnsafeNativeMethods.GetNumberOfEventLogRecords(this.readHandle, out num))
                {
                    throw SharedUtils.CreateSafeWin32Exception();
                }
                return num;
            }
        }

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        private bool IsOpen
        {
            get
            {
                if (this.readHandle == null)
                {
                    return (this.writeHandle != null);
                }
                return true;
            }
        }

        private bool IsOpenForRead =>
            (this.readHandle != null);

        private bool IsOpenForWrite =>
            (this.writeHandle != null);

        [RecommendedAsConfigurable(true), ReadOnly(true), TypeConverter("System.Diagnostics.Design.LogConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), MonitoringDescription("LogLog"), DefaultValue("")]
        public string Log
        {
            get => 
                this.GetLogName(this.machineName);
            set
            {
                this.SetLogName(this.machineName, value);
            }
        }

        [Browsable(false)]
        public string LogDisplayName
        {
            get
            {
                if (this.logDisplayName == null)
                {
                    string machineName = this.machineName;
                    if (this.GetLogName(machineName) != null)
                    {
                        new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
                        SharedUtils.CheckEnvironment();
                        _GetAssertPermSet().Assert();
                        RegistryKey logRegKey = null;
                        try
                        {
                            logRegKey = this.GetLogRegKey(machineName, false);
                            if (logRegKey == null)
                            {
                                throw new InvalidOperationException(SR.GetString("MissingLog", new object[] { this.GetLogName(machineName), machineName }));
                            }
                            string dllNameList = (string) logRegKey.GetValue("DisplayNameFile");
                            if (dllNameList == null)
                            {
                                this.logDisplayName = this.GetLogName(machineName);
                            }
                            else
                            {
                                int num = (int) logRegKey.GetValue("DisplayNameID");
                                this.logDisplayName = this.FormatMessageWrapper(dllNameList, (uint) num, null);
                                if (this.logDisplayName == null)
                                {
                                    this.logDisplayName = this.GetLogName(machineName);
                                }
                            }
                        }
                        finally
                        {
                            if (logRegKey != null)
                            {
                                logRegKey.Close();
                            }
                            CodeAccessPermission.RevertAssert();
                        }
                    }
                }
                return this.logDisplayName;
            }
        }

        [RecommendedAsConfigurable(true), DefaultValue("."), ReadOnly(true), MonitoringDescription("LogMachineName")]
        public string MachineName
        {
            get
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
                return machineName;
            }
            set
            {
                if (!SyntaxCheck.CheckMachineName(value))
                {
                    throw new ArgumentException(SR.GetString("InvalidProperty", new object[] { "MachineName", value }));
                }
                new EventLogPermission(EventLogPermissionAccess.Write, value).Demand();
                string machineName = this.machineName;
                if (machineName != null)
                {
                    if (string.Compare(machineName, value, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return;
                    }
                    this.boolFlags[0x20] = false;
                    if (this.IsOpen)
                    {
                        this.Close(machineName);
                    }
                }
                this.machineName = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), ComVisible(false)]
        public long MaximumKilobytes
        {
            get
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
                object logRegValue = this.GetLogRegValue(machineName, "MaxSize");
                if (logRegValue != null)
                {
                    int num = (int) logRegValue;
                    return (long) ((ulong) (num / 0x400));
                }
                return 0x200L;
            }
            set
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
                if (((value < 0x40L) || (value > 0x3fffc0L)) || ((value % 0x40L) != 0L))
                {
                    throw new ArgumentOutOfRangeException("MaximumKilobytes", SR.GetString("MaximumKilobytesOutOfRange"));
                }
                _GetAssertPermSet().Assert();
                long num = value * 0x400L;
                int num2 = (int) num;
                using (RegistryKey key = this.GetLogRegKey(machineName, true))
                {
                    key.SetValue("MaxSize", num2, RegistryValueKind.DWord);
                }
            }
        }

        internal Hashtable MessageLibraries
        {
            get
            {
                if (this.messageLibraries == null)
                {
                    this.messageLibraries = new Hashtable(StringComparer.OrdinalIgnoreCase);
                }
                return this.messageLibraries;
            }
        }

        [ComVisible(false), Browsable(false)]
        public int MinimumRetentionDays
        {
            get
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
                object logRegValue = this.GetLogRegValue(machineName, "Retention");
                if (logRegValue == null)
                {
                    return 7;
                }
                int num = (int) logRegValue;
                if ((num != 0) && (num != -1))
                {
                    return (int) (((double) num) / 86400.0);
                }
                return num;
            }
        }

        private int OldestEntryNumber
        {
            get
            {
                if (!this.IsOpenForRead)
                {
                    this.OpenForRead(this.machineName);
                }
                int[] number = new int[1];
                if (!Microsoft.Win32.UnsafeNativeMethods.GetOldestEventLogRecord(this.readHandle, number))
                {
                    throw SharedUtils.CreateSafeWin32Exception();
                }
                int num = number[0];
                if (num == 0)
                {
                    num = 1;
                }
                return num;
            }
        }

        [ComVisible(false), Browsable(false)]
        public System.Diagnostics.OverflowAction OverflowAction
        {
            get
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Administer, machineName).Demand();
                object logRegValue = this.GetLogRegValue(machineName, "Retention");
                if (logRegValue != null)
                {
                    switch (((int) logRegValue))
                    {
                        case 0:
                            return System.Diagnostics.OverflowAction.OverwriteAsNeeded;

                        case -1:
                            return System.Diagnostics.OverflowAction.DoNotOverwrite;
                    }
                }
                return System.Diagnostics.OverflowAction.OverwriteOlder;
            }
        }

        internal SafeEventLogReadHandle ReadHandle
        {
            get
            {
                if (!this.IsOpenForRead)
                {
                    this.OpenForRead(this.machineName);
                }
                return this.readHandle;
            }
        }

        private static bool SkipRegPatch
        {
            get
            {
                if (!s_CheckedOsVersion)
                {
                    OperatingSystem oSVersion = Environment.OSVersion;
                    s_SkipRegPatch = (oSVersion.Platform == PlatformID.Win32NT) && (oSVersion.Version.Major > 5);
                    s_CheckedOsVersion = true;
                }
                return s_SkipRegPatch;
            }
        }

        [RecommendedAsConfigurable(true), ReadOnly(true), TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), MonitoringDescription("LogSource"), DefaultValue("")]
        public string Source
        {
            get
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
                return this.sourceName;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                if ((value.Length + @"SYSTEM\CurrentControlSet\Services\EventLog".Length) > 0xfe)
                {
                    throw new ArgumentException(SR.GetString("ParameterTooLong", new object[] { "source", 0xfe - @"SYSTEM\CurrentControlSet\Services\EventLog".Length }));
                }
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
                if (this.sourceName == null)
                {
                    this.sourceName = value;
                }
                else if (string.Compare(this.sourceName, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    this.sourceName = value;
                    if (this.IsOpen)
                    {
                        bool enableRaisingEvents = this.EnableRaisingEvents;
                        this.Close(machineName);
                        this.EnableRaisingEvents = enableRaisingEvents;
                    }
                }
            }
        }

        [Browsable(false), MonitoringDescription("LogSynchronizingObject"), DefaultValue((string) null)]
        public ISynchronizeInvoke SynchronizingObject
        {
            [HostProtection(SecurityAction.LinkDemand, Synchronization=true)]
            get
            {
                string machineName = this.machineName;
                new EventLogPermission(EventLogPermissionAccess.Write, machineName).Demand();
                if ((this.synchronizingObject == null) && base.DesignMode)
                {
                    IDesignerHost service = (IDesignerHost) this.GetService(typeof(IDesignerHost));
                    if (service != null)
                    {
                        object rootComponent = service.RootComponent;
                        if ((rootComponent != null) && (rootComponent is ISynchronizeInvoke))
                        {
                            this.synchronizingObject = (ISynchronizeInvoke) rootComponent;
                        }
                    }
                }
                return this.synchronizingObject;
            }
            set
            {
                this.synchronizingObject = value;
            }
        }

        private class EventLogWaitHandle : WaitHandle
        {
            public EventLogWaitHandle(SafeEventHandle eventLogNativeHandle)
            {
                base.SafeWaitHandle = new SafeWaitHandle(eventLogNativeHandle.DangerousGetHandle(), true);
                eventLogNativeHandle.SetHandleAsInvalid();
            }
        }

        private class LogListeningInfo
        {
            public EventLog handleOwner;
            public ArrayList listeningComponents = new ArrayList();
            public RegisteredWaitHandle registeredWaitHandle;
            public WaitHandle waitHandle;
        }
    }
}

