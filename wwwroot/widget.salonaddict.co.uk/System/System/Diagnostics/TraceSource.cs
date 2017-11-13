﻿namespace System.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Security.Permissions;

    public class TraceSource
    {
        internal bool _initCalled;
        private StringDictionary attributes;
        private SourceSwitch internalSwitch;
        private TraceListenerCollection listeners;
        private readonly TraceEventCache manager;
        private string sourceName;
        private SourceLevels switchLevel;
        private static List<WeakReference> tracesources = new List<WeakReference>();

        public TraceSource(string name) : this(name, SourceLevels.Off)
        {
        }

        public TraceSource(string name, SourceLevels defaultLevel)
        {
            this.manager = new TraceEventCache();
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("name");
            }
            this.sourceName = name;
            this.switchLevel = defaultLevel;
            lock (tracesources)
            {
                tracesources.Add(new WeakReference(this));
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public void Close()
        {
            if (this.listeners != null)
            {
                lock (TraceInternal.critSec)
                {
                    foreach (TraceListener listener in this.listeners)
                    {
                        listener.Close();
                    }
                }
            }
        }

        private void CreateSwitch(string typename, string name)
        {
            if (!string.IsNullOrEmpty(typename))
            {
                this.internalSwitch = (SourceSwitch) TraceUtils.GetRuntimeObject(typename, typeof(SourceSwitch), name);
            }
            else
            {
                this.internalSwitch = new SourceSwitch(name, this.switchLevel.ToString());
            }
        }

        public void Flush()
        {
            if (this.listeners != null)
            {
                if (TraceInternal.UseGlobalLock)
                {
                    lock (TraceInternal.critSec)
                    {
                        foreach (TraceListener listener in this.listeners)
                        {
                            listener.Flush();
                        }
                        return;
                    }
                }
                foreach (TraceListener listener2 in this.listeners)
                {
                    if (!listener2.IsThreadSafe)
                    {
                        lock (listener2)
                        {
                            listener2.Flush();
                            continue;
                        }
                    }
                    listener2.Flush();
                }
            }
        }

        protected internal virtual string[] GetSupportedAttributes() => 
            null;

        private void Initialize()
        {
            if (!this._initCalled)
            {
                lock (this)
                {
                    if (!this._initCalled)
                    {
                        SourceElementsCollection sources = DiagnosticsConfiguration.Sources;
                        if (sources != null)
                        {
                            SourceElement element = sources[this.sourceName];
                            if (element != null)
                            {
                                if (!string.IsNullOrEmpty(element.SwitchName))
                                {
                                    this.CreateSwitch(element.SwitchType, element.SwitchName);
                                }
                                else
                                {
                                    this.CreateSwitch(element.SwitchType, this.sourceName);
                                    if (!string.IsNullOrEmpty(element.SwitchValue))
                                    {
                                        this.internalSwitch.Level = (SourceLevels) Enum.Parse(typeof(SourceLevels), element.SwitchValue);
                                    }
                                }
                                this.listeners = element.Listeners.GetRuntimeObject();
                                this.attributes = new StringDictionary();
                                TraceUtils.VerifyAttributes(element.Attributes, this.GetSupportedAttributes(), this);
                                this.attributes.contents = element.Attributes;
                            }
                            else
                            {
                                this.NoConfigInit();
                            }
                        }
                        else
                        {
                            this.NoConfigInit();
                        }
                        this._initCalled = true;
                    }
                }
            }
        }

        private void NoConfigInit()
        {
            this.internalSwitch = new SourceSwitch(this.sourceName, this.switchLevel.ToString());
            this.listeners = new TraceListenerCollection();
            this.listeners.Add(new DefaultTraceListener());
            this.attributes = null;
        }

        internal void Refresh()
        {
            if (!this._initCalled)
            {
                this.Initialize();
            }
            else
            {
                SourceElementsCollection sources = DiagnosticsConfiguration.Sources;
                if (sources != null)
                {
                    SourceElement element = sources[this.Name];
                    if (element != null)
                    {
                        if ((string.IsNullOrEmpty(element.SwitchType) && (this.internalSwitch.GetType() != typeof(SourceSwitch))) || (element.SwitchType != this.internalSwitch.GetType().AssemblyQualifiedName))
                        {
                            if (!string.IsNullOrEmpty(element.SwitchName))
                            {
                                this.CreateSwitch(element.SwitchType, element.SwitchName);
                            }
                            else
                            {
                                this.CreateSwitch(element.SwitchType, this.Name);
                                if (!string.IsNullOrEmpty(element.SwitchValue))
                                {
                                    this.internalSwitch.Level = (SourceLevels) Enum.Parse(typeof(SourceLevels), element.SwitchValue);
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(element.SwitchName))
                        {
                            if (element.SwitchName != this.internalSwitch.DisplayName)
                            {
                                this.CreateSwitch(element.SwitchType, element.SwitchName);
                            }
                            else
                            {
                                this.internalSwitch.Refresh();
                            }
                        }
                        else if (!string.IsNullOrEmpty(element.SwitchValue))
                        {
                            this.internalSwitch.Level = (SourceLevels) Enum.Parse(typeof(SourceLevels), element.SwitchValue);
                        }
                        else
                        {
                            this.internalSwitch.Level = SourceLevels.Off;
                        }
                        TraceListenerCollection listeners = new TraceListenerCollection();
                        foreach (ListenerElement element2 in element.Listeners)
                        {
                            TraceListener listener = this.listeners[element2.Name];
                            if (listener != null)
                            {
                                listeners.Add(element2.RefreshRuntimeObject(listener));
                            }
                            else
                            {
                                listeners.Add(element2.GetRuntimeObject());
                            }
                        }
                        TraceUtils.VerifyAttributes(element.Attributes, this.GetSupportedAttributes(), this);
                        this.attributes = new StringDictionary();
                        this.attributes.contents = element.Attributes;
                        this.listeners = listeners;
                    }
                    else
                    {
                        this.internalSwitch.Level = this.switchLevel;
                        this.listeners.Clear();
                        this.attributes = null;
                    }
                }
            }
        }

        internal static void RefreshAll()
        {
            lock (tracesources)
            {
                for (int i = 0; i < tracesources.Count; i++)
                {
                    TraceSource target = (TraceSource) tracesources[i].Target;
                    if (target != null)
                    {
                        target.Refresh();
                    }
                }
            }
        }

        [Conditional("TRACE")]
        public void TraceData(TraceEventType eventType, int id, object data)
        {
            this.Initialize();
            if (!this.internalSwitch.ShouldTrace(eventType) || (this.listeners == null))
            {
                return;
            }
            if (TraceInternal.UseGlobalLock)
            {
                lock (TraceInternal.critSec)
                {
                    for (int j = 0; j < this.listeners.Count; j++)
                    {
                        TraceListener listener = this.listeners[j];
                        listener.TraceData(this.manager, this.Name, eventType, id, data);
                        if (Trace.AutoFlush)
                        {
                            listener.Flush();
                        }
                    }
                    goto Label_010A;
                }
            }
            for (int i = 0; i < this.listeners.Count; i++)
            {
                TraceListener listener2 = this.listeners[i];
                if (!listener2.IsThreadSafe)
                {
                    lock (listener2)
                    {
                        listener2.TraceData(this.manager, this.Name, eventType, id, data);
                        if (Trace.AutoFlush)
                        {
                            listener2.Flush();
                        }
                        continue;
                    }
                }
                listener2.TraceData(this.manager, this.Name, eventType, id, data);
                if (Trace.AutoFlush)
                {
                    listener2.Flush();
                }
            }
        Label_010A:
            this.manager.Clear();
        }

        [Conditional("TRACE")]
        public void TraceData(TraceEventType eventType, int id, params object[] data)
        {
            this.Initialize();
            if (!this.internalSwitch.ShouldTrace(eventType) || (this.listeners == null))
            {
                return;
            }
            if (TraceInternal.UseGlobalLock)
            {
                lock (TraceInternal.critSec)
                {
                    for (int j = 0; j < this.listeners.Count; j++)
                    {
                        TraceListener listener = this.listeners[j];
                        listener.TraceData(this.manager, this.Name, eventType, id, data);
                        if (Trace.AutoFlush)
                        {
                            listener.Flush();
                        }
                    }
                    goto Label_010A;
                }
            }
            for (int i = 0; i < this.listeners.Count; i++)
            {
                TraceListener listener2 = this.listeners[i];
                if (!listener2.IsThreadSafe)
                {
                    lock (listener2)
                    {
                        listener2.TraceData(this.manager, this.Name, eventType, id, data);
                        if (Trace.AutoFlush)
                        {
                            listener2.Flush();
                        }
                        continue;
                    }
                }
                listener2.TraceData(this.manager, this.Name, eventType, id, data);
                if (Trace.AutoFlush)
                {
                    listener2.Flush();
                }
            }
        Label_010A:
            this.manager.Clear();
        }

        [Conditional("TRACE")]
        public void TraceEvent(TraceEventType eventType, int id)
        {
            this.Initialize();
            if (!this.internalSwitch.ShouldTrace(eventType) || (this.listeners == null))
            {
                return;
            }
            if (TraceInternal.UseGlobalLock)
            {
                lock (TraceInternal.critSec)
                {
                    for (int j = 0; j < this.listeners.Count; j++)
                    {
                        TraceListener listener = this.listeners[j];
                        listener.TraceEvent(this.manager, this.Name, eventType, id);
                        if (Trace.AutoFlush)
                        {
                            listener.Flush();
                        }
                    }
                    goto Label_0107;
                }
            }
            for (int i = 0; i < this.listeners.Count; i++)
            {
                TraceListener listener2 = this.listeners[i];
                if (!listener2.IsThreadSafe)
                {
                    lock (listener2)
                    {
                        listener2.TraceEvent(this.manager, this.Name, eventType, id);
                        if (Trace.AutoFlush)
                        {
                            listener2.Flush();
                        }
                        continue;
                    }
                }
                listener2.TraceEvent(this.manager, this.Name, eventType, id);
                if (Trace.AutoFlush)
                {
                    listener2.Flush();
                }
            }
        Label_0107:
            this.manager.Clear();
        }

        [Conditional("TRACE")]
        public void TraceEvent(TraceEventType eventType, int id, string message)
        {
            this.Initialize();
            if (!this.internalSwitch.ShouldTrace(eventType) || (this.listeners == null))
            {
                return;
            }
            if (TraceInternal.UseGlobalLock)
            {
                lock (TraceInternal.critSec)
                {
                    for (int j = 0; j < this.listeners.Count; j++)
                    {
                        TraceListener listener = this.listeners[j];
                        listener.TraceEvent(this.manager, this.Name, eventType, id, message);
                        if (Trace.AutoFlush)
                        {
                            listener.Flush();
                        }
                    }
                    goto Label_010A;
                }
            }
            for (int i = 0; i < this.listeners.Count; i++)
            {
                TraceListener listener2 = this.listeners[i];
                if (!listener2.IsThreadSafe)
                {
                    lock (listener2)
                    {
                        listener2.TraceEvent(this.manager, this.Name, eventType, id, message);
                        if (Trace.AutoFlush)
                        {
                            listener2.Flush();
                        }
                        continue;
                    }
                }
                listener2.TraceEvent(this.manager, this.Name, eventType, id, message);
                if (Trace.AutoFlush)
                {
                    listener2.Flush();
                }
            }
        Label_010A:
            this.manager.Clear();
        }

        [Conditional("TRACE")]
        public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
        {
            this.Initialize();
            if (!this.internalSwitch.ShouldTrace(eventType) || (this.listeners == null))
            {
                return;
            }
            if (TraceInternal.UseGlobalLock)
            {
                lock (TraceInternal.critSec)
                {
                    for (int j = 0; j < this.listeners.Count; j++)
                    {
                        TraceListener listener = this.listeners[j];
                        listener.TraceEvent(this.manager, this.Name, eventType, id, format, args);
                        if (Trace.AutoFlush)
                        {
                            listener.Flush();
                        }
                    }
                    goto Label_0113;
                }
            }
            for (int i = 0; i < this.listeners.Count; i++)
            {
                TraceListener listener2 = this.listeners[i];
                if (!listener2.IsThreadSafe)
                {
                    lock (listener2)
                    {
                        listener2.TraceEvent(this.manager, this.Name, eventType, id, format, args);
                        if (Trace.AutoFlush)
                        {
                            listener2.Flush();
                        }
                        continue;
                    }
                }
                listener2.TraceEvent(this.manager, this.Name, eventType, id, format, args);
                if (Trace.AutoFlush)
                {
                    listener2.Flush();
                }
            }
        Label_0113:
            this.manager.Clear();
        }

        [Conditional("TRACE")]
        public void TraceInformation(string message)
        {
            this.TraceEvent(TraceEventType.Information, 0, message, null);
        }

        [Conditional("TRACE")]
        public void TraceInformation(string format, params object[] args)
        {
            this.TraceEvent(TraceEventType.Information, 0, format, args);
        }

        [Conditional("TRACE")]
        public void TraceTransfer(int id, string message, Guid relatedActivityId)
        {
            this.Initialize();
            if (!this.internalSwitch.ShouldTrace(TraceEventType.Transfer) || (this.listeners == null))
            {
                return;
            }
            if (TraceInternal.UseGlobalLock)
            {
                lock (TraceInternal.critSec)
                {
                    for (int j = 0; j < this.listeners.Count; j++)
                    {
                        TraceListener listener = this.listeners[j];
                        listener.TraceTransfer(this.manager, this.Name, id, message, relatedActivityId);
                        if (Trace.AutoFlush)
                        {
                            listener.Flush();
                        }
                    }
                    goto Label_010E;
                }
            }
            for (int i = 0; i < this.listeners.Count; i++)
            {
                TraceListener listener2 = this.listeners[i];
                if (!listener2.IsThreadSafe)
                {
                    lock (listener2)
                    {
                        listener2.TraceTransfer(this.manager, this.Name, id, message, relatedActivityId);
                        if (Trace.AutoFlush)
                        {
                            listener2.Flush();
                        }
                        continue;
                    }
                }
                listener2.TraceTransfer(this.manager, this.Name, id, message, relatedActivityId);
                if (Trace.AutoFlush)
                {
                    listener2.Flush();
                }
            }
        Label_010E:
            this.manager.Clear();
        }

        public StringDictionary Attributes
        {
            get
            {
                this.Initialize();
                if (this.attributes == null)
                {
                    this.attributes = new StringDictionary();
                }
                return this.attributes;
            }
        }

        public TraceListenerCollection Listeners
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                this.Initialize();
                return this.listeners;
            }
        }

        public string Name =>
            this.sourceName;

        public SourceSwitch Switch
        {
            get
            {
                this.Initialize();
                return this.internalSwitch;
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Switch");
                }
                this.Initialize();
                this.internalSwitch = value;
            }
        }
    }
}

