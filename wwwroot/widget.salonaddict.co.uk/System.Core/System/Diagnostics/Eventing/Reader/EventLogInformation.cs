﻿namespace System.Diagnostics.Eventing.Reader
{
    using Microsoft.Win32;
    using System;
    using System.Security;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class EventLogInformation
    {
        private DateTime? creationTime;
        private int? fileAttributes;
        private long? fileSize;
        private bool? isLogFull;
        private DateTime? lastAccessTime;
        private DateTime? lastWriteTime;
        private long? oldestRecordNumber;
        private long? recordCount;

        [SecurityTreatAsSafe, SecurityCritical]
        internal EventLogInformation(EventLogSession session, string channelName, PathType pathType)
        {
            EventLogPermissionHolder.GetEventLogPermission().Demand();
            EventLogHandle handle = NativeWrapper.EvtOpenLog(session.Handle, channelName, pathType);
            using (handle)
            {
                this.creationTime = (DateTime?) NativeWrapper.EvtGetLogInfo(handle, Microsoft.Win32.UnsafeNativeMethods.EvtLogPropertyId.EvtLogCreationTime);
                this.lastAccessTime = (DateTime?) NativeWrapper.EvtGetLogInfo(handle, Microsoft.Win32.UnsafeNativeMethods.EvtLogPropertyId.EvtLogLastAccessTime);
                this.lastWriteTime = (DateTime?) NativeWrapper.EvtGetLogInfo(handle, Microsoft.Win32.UnsafeNativeMethods.EvtLogPropertyId.EvtLogLastWriteTime);
                ulong? nullable = (ulong?) NativeWrapper.EvtGetLogInfo(handle, Microsoft.Win32.UnsafeNativeMethods.EvtLogPropertyId.EvtLogFileSize);
                this.fileSize = nullable.HasValue ? new long?(nullable.GetValueOrDefault()) : null;
                uint? nullable3 = (uint?) NativeWrapper.EvtGetLogInfo(handle, Microsoft.Win32.UnsafeNativeMethods.EvtLogPropertyId.EvtLogAttributes);
                this.fileAttributes = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault()) : null;
                ulong? nullable5 = (ulong?) NativeWrapper.EvtGetLogInfo(handle, Microsoft.Win32.UnsafeNativeMethods.EvtLogPropertyId.EvtLogNumberOfLogRecords);
                this.recordCount = nullable5.HasValue ? new long?(nullable5.GetValueOrDefault()) : null;
                ulong? nullable7 = (ulong?) NativeWrapper.EvtGetLogInfo(handle, Microsoft.Win32.UnsafeNativeMethods.EvtLogPropertyId.EvtLogOldestRecordNumber);
                this.oldestRecordNumber = nullable7.HasValue ? new long?(nullable7.GetValueOrDefault()) : null;
                this.isLogFull = (bool?) NativeWrapper.EvtGetLogInfo(handle, Microsoft.Win32.UnsafeNativeMethods.EvtLogPropertyId.EvtLogFull);
            }
        }

        public int? Attributes =>
            this.fileAttributes;

        public DateTime? CreationTime =>
            this.creationTime;

        public long? FileSize =>
            this.fileSize;

        public bool? IsLogFull =>
            this.isLogFull;

        public DateTime? LastAccessTime =>
            this.lastAccessTime;

        public DateTime? LastWriteTime =>
            this.lastWriteTime;

        public long? OldestRecordNumber =>
            this.oldestRecordNumber;

        public long? RecordCount =>
            this.recordCount;
    }
}

