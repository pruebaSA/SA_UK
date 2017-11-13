namespace MS.Internal
{
    using Microsoft.Win32;
    using MS.Internal.WindowsBase;
    using MS.Win32;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Windows;

    internal class AvTrace
    {
        private ClearTraceSourceDelegate _clearTraceSourceDelegate;
        private bool _enabledByDebugger;
        private static bool? _enabledInRegistry = null;
        private GetTraceSourceDelegate _getTraceSourceDelegate;
        private static bool _hasBeenRefreshed = false;
        private bool _isEnabled;
        private bool _suppressGeneratedParameters;
        private TraceSource _traceSource;

        public event AvTraceEventHandler TraceExtraMessages;

        public AvTrace(GetTraceSourceDelegate getTraceSourceDelegate, ClearTraceSourceDelegate clearTraceSourceDelegate)
        {
            this._getTraceSourceDelegate = getTraceSourceDelegate;
            this._clearTraceSourceDelegate = clearTraceSourceDelegate;
            PresentationTraceSources.TraceRefresh += new TraceRefreshEventHandler(this.Refresh);
            this.Initialize();
        }

        public static int GetHashCodeHelper(object value)
        {
            try
            {
                return ((value != null) ? value.GetHashCode() : 0);
            }
            catch (Exception exception)
            {
                if (CriticalExceptions.IsCriticalException(exception) && !(exception is NullReferenceException))
                {
                    throw;
                }
                return 0;
            }
        }

        public static Type GetTypeHelper(object value) => 
            value?.GetType();

        private void Initialize()
        {
            if (ShouldCreateTraceSources())
            {
                this._traceSource = this._getTraceSourceDelegate();
                this._isEnabled = (IsWpfTracingEnabledInRegistry() || _hasBeenRefreshed) || this._enabledByDebugger;
            }
            else
            {
                this._clearTraceSourceDelegate();
                this._traceSource = null;
                this._isEnabled = false;
            }
        }

        internal static bool IsDebuggerAttached()
        {
            if (!Debugger.IsAttached)
            {
                return MS.Win32.SafeNativeMethods.IsDebuggerPresent();
            }
            return true;
        }

        [FriendAccessAllowed, SecurityCritical, SecurityTreatAsSafe]
        internal static bool IsWpfTracingEnabledInRegistry()
        {
            if (!_enabledInRegistry.HasValue)
            {
                bool flag = false;
                object obj2 = SecurityHelper.ReadRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Tracing\WPF", "ManagedTracing");
                if ((obj2 is int) && (((int) obj2) == 1))
                {
                    flag = true;
                }
                _enabledInRegistry = new bool?(flag);
            }
            return _enabledInRegistry.Value;
        }

        public static void OnRefresh()
        {
            _hasBeenRefreshed = true;
        }

        public void Refresh()
        {
            _enabledInRegistry = null;
            this.Initialize();
        }

        private static bool ShouldCreateTraceSources()
        {
            if ((!IsWpfTracingEnabledInRegistry() && !IsDebuggerAttached()) && !_hasBeenRefreshed)
            {
                return false;
            }
            return true;
        }

        public static string ToStringHelper(object value)
        {
            if (value == null)
            {
                return "<null>";
            }
            try
            {
                return value.ToString();
            }
            catch
            {
                return "<unprintable>";
            }
        }

        public void Trace(TraceEventType type, int eventId, string message, string[] labels, object[] parameters)
        {
            if ((this._traceSource != null) && this._traceSource.Switch.ShouldTrace(type))
            {
                AvTraceBuilder traceBuilder = new AvTraceBuilder(message);
                ArrayList list = new ArrayList();
                int num = 0;
                if (((parameters != null) && (labels != null)) && (labels.Length > 0))
                {
                    int index = 1;
                    int num3 = 0;
                    while ((index < labels.Length) && (num3 < parameters.Length))
                    {
                        string[] strArray = new string[] { "; {", num++.ToString(), "}='{", num++.ToString(), "}'" };
                        traceBuilder.Append(string.Concat(strArray));
                        if (parameters[num3] == null)
                        {
                            parameters[num3] = "<null>";
                        }
                        else if (((!this.SuppressGeneratedParameters && (parameters[num3].GetType() != typeof(string))) && (!(parameters[num3] is ValueType) && !(parameters[num3] is Type))) && !(parameters[num3] is DependencyProperty))
                        {
                            traceBuilder.Append("; " + labels[index].ToString() + ".HashCode='" + GetHashCodeHelper(parameters[num3]).ToString() + "'");
                            traceBuilder.Append("; " + labels[index].ToString() + ".Type='" + GetTypeHelper(parameters[num3]).ToString() + "'");
                        }
                        list.Add(labels[index]);
                        list.Add(parameters[num3]);
                        index++;
                        num3++;
                    }
                    if ((this.TraceExtraMessages != null) && (num3 < parameters.Length))
                    {
                        this.TraceExtraMessages(traceBuilder, parameters, num3);
                    }
                }
                this._traceSource.TraceEvent(type, eventId, traceBuilder.ToString(), list.ToArray());
                if (IsDebuggerAttached())
                {
                    this._traceSource.Flush();
                }
            }
        }

        public void TraceStartStop(int eventID, string message, string[] labels, object[] parameters)
        {
            this.Trace(TraceEventType.Start, eventID, message, labels, parameters);
            this._traceSource.TraceEvent(TraceEventType.Stop, eventID);
        }

        public static string TypeName(object value) => 
            value?.GetType().Name;

        public bool EnabledByDebugger
        {
            get => 
                this._enabledByDebugger;
            set
            {
                this._enabledByDebugger = value;
                if (this._enabledByDebugger)
                {
                    if (!this.IsEnabled && IsDebuggerAttached())
                    {
                        this._isEnabled = true;
                    }
                }
                else if ((this.IsEnabled && !IsWpfTracingEnabledInRegistry()) && !_hasBeenRefreshed)
                {
                    this._isEnabled = false;
                }
            }
        }

        public bool IsEnabled =>
            this._isEnabled;

        public bool IsEnabledOverride =>
            (this._traceSource != null);

        public bool SuppressGeneratedParameters
        {
            get => 
                this._suppressGeneratedParameters;
            set
            {
                this._suppressGeneratedParameters = value;
            }
        }
    }
}

