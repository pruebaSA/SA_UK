namespace MS.Internal
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal static class TraceDependencyProperty
    {
        private static AvTraceDetails _ApplyTemplateContent;
        private static AvTrace _avTrace = new AvTrace(() => PresentationTraceSources.DependencyPropertySource, () => PresentationTraceSources._DependencyPropertySource = null);
        private static AvTraceDetails _Register;
        private static AvTraceDetails _UpdateEffectiveValueStart;
        private static AvTraceDetails _UpdateEffectiveValueStop;

        [CompilerGenerated]
        private static TraceSource <.cctor>b__0() => 
            PresentationTraceSources.DependencyPropertySource;

        [CompilerGenerated]
        private static void <.cctor>b__1()
        {
            PresentationTraceSources._DependencyPropertySource = null;
        }

        public static void Refresh()
        {
            _avTrace.Refresh();
        }

        public static void Trace(TraceEventType type, AvTraceDetails traceDetails)
        {
            _avTrace.Trace(type, traceDetails.Id, traceDetails.Message, traceDetails.Labels, new object[0]);
        }

        public static void Trace(TraceEventType type, AvTraceDetails traceDetails, params object[] parameters)
        {
            _avTrace.Trace(type, traceDetails.Id, traceDetails.Message, traceDetails.Labels, parameters);
        }

        public static void Trace(TraceEventType type, AvTraceDetails traceDetails, object p1)
        {
            _avTrace.Trace(type, traceDetails.Id, traceDetails.Message, traceDetails.Labels, new object[] { p1 });
        }

        public static void Trace(TraceEventType type, AvTraceDetails traceDetails, object p1, object p2)
        {
            _avTrace.Trace(type, traceDetails.Id, traceDetails.Message, traceDetails.Labels, new object[] { p1, p2 });
        }

        public static void Trace(TraceEventType type, AvTraceDetails traceDetails, object p1, object p2, object p3)
        {
            _avTrace.Trace(type, traceDetails.Id, traceDetails.Message, traceDetails.Labels, new object[] { p1, p2, p3 });
        }

        public static void TraceActivityItem(AvTraceDetails traceDetails)
        {
            _avTrace.TraceStartStop(traceDetails.Id, traceDetails.Message, traceDetails.Labels, new object[0]);
        }

        public static void TraceActivityItem(AvTraceDetails traceDetails, params object[] parameters)
        {
            _avTrace.TraceStartStop(traceDetails.Id, traceDetails.Message, traceDetails.Labels, parameters);
        }

        public static void TraceActivityItem(AvTraceDetails traceDetails, object p1)
        {
            _avTrace.TraceStartStop(traceDetails.Id, traceDetails.Message, traceDetails.Labels, new object[] { p1 });
        }

        public static void TraceActivityItem(AvTraceDetails traceDetails, object p1, object p2)
        {
            _avTrace.TraceStartStop(traceDetails.Id, traceDetails.Message, traceDetails.Labels, new object[] { p1, p2 });
        }

        public static void TraceActivityItem(AvTraceDetails traceDetails, object p1, object p2, object p3)
        {
            _avTrace.TraceStartStop(traceDetails.Id, traceDetails.Message, traceDetails.Labels, new object[] { p1, p2, p3 });
        }

        public static AvTraceDetails ApplyTemplateContent
        {
            get
            {
                if (_ApplyTemplateContent == null)
                {
                    _ApplyTemplateContent = new AvTraceDetails(1, new string[] { "Apply template", "Element", "Template" });
                }
                return _ApplyTemplateContent;
            }
        }

        public static bool IsEnabled =>
            ((_avTrace != null) && _avTrace.IsEnabled);

        public static bool IsEnabledOverride =>
            _avTrace.IsEnabledOverride;

        public static AvTraceDetails Register
        {
            get
            {
                if (_Register == null)
                {
                    _Register = new AvTraceDetails(2, new string[] { "Registered DependencyProperty", "DP", "OwnerType" });
                }
                return _Register;
            }
        }

        public static AvTraceDetails UpdateEffectiveValueStart
        {
            get
            {
                if (_UpdateEffectiveValueStart == null)
                {
                    _UpdateEffectiveValueStart = new AvTraceDetails(3, new string[] { "Update effective DP value (Start)", "DependencyObject", "DP", "DpOwnerType", "Value", "ValueSource" });
                }
                return _UpdateEffectiveValueStart;
            }
        }

        public static AvTraceDetails UpdateEffectiveValueStop
        {
            get
            {
                if (_UpdateEffectiveValueStop == null)
                {
                    _UpdateEffectiveValueStop = new AvTraceDetails(4, new string[] { "Update effective DP value (Stop)", "DependencyObject", "DP", "DpOwnerType", "Value", "ValueSource" });
                }
                return _UpdateEffectiveValueStop;
            }
        }
    }
}

