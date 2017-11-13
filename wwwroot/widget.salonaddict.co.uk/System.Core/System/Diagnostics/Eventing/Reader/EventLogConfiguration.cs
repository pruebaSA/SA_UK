﻿namespace System.Diagnostics.Eventing.Reader
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public class EventLogConfiguration : IDisposable
    {
        private string channelName;
        private EventLogHandle handle;
        private EventLogSession session;

        public EventLogConfiguration(string logName) : this(logName, null)
        {
        }

        [SecurityCritical]
        public EventLogConfiguration(string logName, EventLogSession session)
        {
            this.handle = EventLogHandle.Zero;
            EventLogPermissionHolder.GetEventLogPermission().Demand();
            if (session == null)
            {
                session = EventLogSession.GlobalSession;
            }
            this.session = session;
            this.channelName = logName;
            this.handle = NativeWrapper.EvtOpenChannelConfig(this.session.Handle, this.channelName, 0);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SecurityTreatAsSafe, SecurityCritical]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                EventLogPermissionHolder.GetEventLogPermission().Demand();
            }
            if ((this.handle != null) && !this.handle.IsInvalid)
            {
                this.handle.Dispose();
            }
        }

        public void SaveChanges()
        {
            NativeWrapper.EvtSaveChannelConfig(this.handle, 0);
        }

        public bool IsClassicLog =>
            ((bool) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelConfigClassicEventlog));

        public bool IsEnabled
        {
            get => 
                ((bool) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelConfigEnabled));
            set
            {
                NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelConfigEnabled, value);
            }
        }

        public string LogFilePath
        {
            get => 
                ((string) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigLogFilePath));
            set
            {
                NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigLogFilePath, value);
            }
        }

        public EventLogIsolation LogIsolation =>
            ((EventLogIsolation) ((uint) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelConfigIsolation)));

        public EventLogMode LogMode
        {
            get
            {
                object obj2 = NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigRetention);
                object obj3 = NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigAutoBackup);
                bool flag = (obj2 != null) && ((bool) obj2);
                if ((obj3 != null) && ((bool) obj3))
                {
                    return EventLogMode.AutoBackup;
                }
                if (flag)
                {
                    return EventLogMode.Retain;
                }
                return EventLogMode.Circular;
            }
            set
            {
                switch (value)
                {
                    case EventLogMode.Circular:
                        NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigAutoBackup, false);
                        NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigRetention, false);
                        return;

                    case EventLogMode.AutoBackup:
                        NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigAutoBackup, true);
                        NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigRetention, true);
                        return;

                    case EventLogMode.Retain:
                        NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigAutoBackup, false);
                        NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigRetention, true);
                        return;
                }
            }
        }

        public string LogName =>
            this.channelName;

        public EventLogType LogType =>
            ((EventLogType) ((uint) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelConfigType)));

        public long MaximumSizeInBytes
        {
            get => 
                ((long) ((ulong) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigMaxSize)));
            set
            {
                NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelLoggingConfigMaxSize, value);
            }
        }

        public string OwningProviderName =>
            ((string) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelConfigOwningPublisher));

        public int? ProviderBufferSize
        {
            get
            {
                uint? nullable = (uint?) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelPublishingConfigBufferSize);
                if (!nullable.HasValue)
                {
                    return null;
                }
                return new int?(nullable.GetValueOrDefault());
            }
        }

        public Guid? ProviderControlGuid =>
            ((Guid?) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelPublishingConfigControlGuid));

        public long? ProviderKeywords
        {
            get
            {
                ulong? nullable = (ulong?) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelPublishingConfigKeywords);
                if (!nullable.HasValue)
                {
                    return null;
                }
                return new long?(nullable.GetValueOrDefault());
            }
            set
            {
                NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelPublishingConfigKeywords, value);
            }
        }

        public int? ProviderLatency
        {
            get
            {
                uint? nullable = (uint?) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelPublishingConfigLatency);
                if (!nullable.HasValue)
                {
                    return null;
                }
                return new int?(nullable.GetValueOrDefault());
            }
        }

        public int? ProviderLevel
        {
            get
            {
                uint? nullable = (uint?) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelPublishingConfigLevel);
                if (!nullable.HasValue)
                {
                    return null;
                }
                return new int?(nullable.GetValueOrDefault());
            }
            set
            {
                NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelPublishingConfigLevel, value);
            }
        }

        public int? ProviderMaximumNumberOfBuffers
        {
            get
            {
                uint? nullable = (uint?) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelPublishingConfigMaxBuffers);
                if (!nullable.HasValue)
                {
                    return null;
                }
                return new int?(nullable.GetValueOrDefault());
            }
        }

        public int? ProviderMinimumNumberOfBuffers
        {
            get
            {
                uint? nullable = (uint?) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelPublishingConfigMinBuffers);
                if (!nullable.HasValue)
                {
                    return null;
                }
                return new int?(nullable.GetValueOrDefault());
            }
        }

        public IEnumerable<string> ProviderNames =>
            ((string[]) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelPublisherList));

        public string SecurityDescriptor
        {
            get => 
                ((string) NativeWrapper.EvtGetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelConfigAccess));
            set
            {
                NativeWrapper.EvtSetChannelConfigProperty(this.handle, Microsoft.Win32.UnsafeNativeMethods.EvtChannelConfigPropertyId.EvtChannelConfigAccess, value);
            }
        }
    }
}

