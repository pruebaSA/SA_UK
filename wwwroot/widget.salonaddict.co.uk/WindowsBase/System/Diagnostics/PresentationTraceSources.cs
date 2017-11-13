namespace System.Diagnostics
{
    using MS.Internal;
    using System;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows;

    public static class PresentationTraceSources
    {
        internal static TraceSource _AnimationSource;
        internal static TraceSource _DataBindingSource;
        internal static TraceSource _DependencyPropertySource;
        internal static TraceSource _DocumentsSource;
        internal static TraceSource _FreezableSource;
        internal static TraceSource _HwndHostSource;
        internal static TraceSource _MarkupSource;
        internal static TraceSource _NameScopeSource;
        internal static TraceSource _ResourceDictionarySource;
        internal static TraceSource _RoutedEventSource;
        public static readonly DependencyProperty TraceLevelProperty = DependencyProperty.RegisterAttached("TraceLevel", typeof(PresentationTraceLevel), typeof(PresentationTraceSources));

        internal static  event TraceRefreshEventHandler TraceRefresh;

        [SecurityCritical, SecurityTreatAsSafe]
        private static TraceSource CreateTraceSource(string sourceName)
        {
            TraceSource source = new TraceSource(sourceName);
            if ((source.Switch.Level == SourceLevels.Off) && AvTrace.IsDebuggerAttached())
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
                try
                {
                    source.Switch.Level = SourceLevels.Warning;
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
            }
            return source;
        }

        public static PresentationTraceLevel GetTraceLevel(object element) => 
            TraceLevelStore.GetTraceLevel(element);

        public static void Refresh()
        {
            AvTrace.OnRefresh();
            Trace.Refresh();
            if (TraceRefresh != null)
            {
                TraceRefresh();
            }
        }

        public static void SetTraceLevel(object element, PresentationTraceLevel traceLevel)
        {
            TraceLevelStore.SetTraceLevel(element, traceLevel);
        }

        public static TraceSource AnimationSource
        {
            get
            {
                if (_AnimationSource == null)
                {
                    _AnimationSource = CreateTraceSource("System.Windows.Media.Animation");
                }
                return _AnimationSource;
            }
        }

        public static TraceSource DataBindingSource
        {
            get
            {
                if (_DataBindingSource == null)
                {
                    _DataBindingSource = CreateTraceSource("System.Windows.Data");
                }
                return _DataBindingSource;
            }
        }

        public static TraceSource DependencyPropertySource
        {
            get
            {
                if (_DependencyPropertySource == null)
                {
                    _DependencyPropertySource = CreateTraceSource("System.Windows.DependencyProperty");
                }
                return _DependencyPropertySource;
            }
        }

        public static TraceSource DocumentsSource
        {
            get
            {
                if (_DocumentsSource == null)
                {
                    _DocumentsSource = CreateTraceSource("System.Windows.Documents");
                }
                return _DocumentsSource;
            }
        }

        public static TraceSource FreezableSource
        {
            get
            {
                if (_FreezableSource == null)
                {
                    _FreezableSource = CreateTraceSource("System.Windows.Freezable");
                }
                return _FreezableSource;
            }
        }

        public static TraceSource HwndHostSource
        {
            get
            {
                if (_HwndHostSource == null)
                {
                    _HwndHostSource = CreateTraceSource("System.Windows.Interop.HwndHost");
                }
                return _HwndHostSource;
            }
        }

        public static TraceSource MarkupSource
        {
            get
            {
                if (_MarkupSource == null)
                {
                    _MarkupSource = CreateTraceSource("System.Windows.Markup");
                }
                return _MarkupSource;
            }
        }

        public static TraceSource NameScopeSource
        {
            get
            {
                if (_NameScopeSource == null)
                {
                    _NameScopeSource = CreateTraceSource("System.Windows.NameScope");
                }
                return _NameScopeSource;
            }
        }

        public static TraceSource ResourceDictionarySource
        {
            get
            {
                if (_ResourceDictionarySource == null)
                {
                    _ResourceDictionarySource = CreateTraceSource("System.Windows.ResourceDictionary");
                }
                return _ResourceDictionarySource;
            }
        }

        public static TraceSource RoutedEventSource
        {
            get
            {
                if (_RoutedEventSource == null)
                {
                    _RoutedEventSource = CreateTraceSource("System.Windows.RoutedEvent");
                }
                return _RoutedEventSource;
            }
        }
    }
}

