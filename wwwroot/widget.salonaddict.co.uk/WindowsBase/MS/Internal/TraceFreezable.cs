namespace MS.Internal
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal static class TraceFreezable
    {
        private static AvTrace _avTrace = new AvTrace(() => PresentationTraceSources.FreezableSource, () => PresentationTraceSources._FreezableSource = null);
        private static AvTraceDetails _UnableToFreezeAnimatedProperties;
        private static AvTraceDetails _UnableToFreezeDispatcherObjectWithThreadAffinity;
        private static AvTraceDetails _UnableToFreezeExpression;
        private static AvTraceDetails _UnableToFreezeFreezableSubProperty;

        [CompilerGenerated]
        private static TraceSource <.cctor>b__0() => 
            PresentationTraceSources.FreezableSource;

        [CompilerGenerated]
        private static void <.cctor>b__1()
        {
            PresentationTraceSources._FreezableSource = null;
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

        public static bool IsEnabled =>
            ((_avTrace != null) && _avTrace.IsEnabled);

        public static bool IsEnabledOverride =>
            _avTrace.IsEnabledOverride;

        public static AvTraceDetails UnableToFreezeAnimatedProperties
        {
            get
            {
                if (_UnableToFreezeAnimatedProperties == null)
                {
                    _UnableToFreezeAnimatedProperties = new AvTraceDetails(4, new string[] { "CanFreeze is returning false because at least one DependencyProperty on the Freezable is animated.", "Freezable" });
                }
                return _UnableToFreezeAnimatedProperties;
            }
        }

        public static AvTraceDetails UnableToFreezeDispatcherObjectWithThreadAffinity
        {
            get
            {
                if (_UnableToFreezeDispatcherObjectWithThreadAffinity == null)
                {
                    _UnableToFreezeDispatcherObjectWithThreadAffinity = new AvTraceDetails(2, new string[] { "CanFreeze is returning false because a DependencyProperty on the Freezable has a value that is a DispatcherObject with thread affinity", "Freezable", "DP", "DpOwnerType", "Value" });
                }
                return _UnableToFreezeDispatcherObjectWithThreadAffinity;
            }
        }

        public static AvTraceDetails UnableToFreezeExpression
        {
            get
            {
                if (_UnableToFreezeExpression == null)
                {
                    _UnableToFreezeExpression = new AvTraceDetails(1, new string[] { "CanFreeze is returning false because a DependencyProperty on the Freezable has a value that is an expression", "Freezable", "DP", "DpOwnerType" });
                }
                return _UnableToFreezeExpression;
            }
        }

        public static AvTraceDetails UnableToFreezeFreezableSubProperty
        {
            get
            {
                if (_UnableToFreezeFreezableSubProperty == null)
                {
                    _UnableToFreezeFreezableSubProperty = new AvTraceDetails(3, new string[] { "CanFreeze is returning false because a DependencyProperty on the Freezable has a value that is a Freezable that has also returned false from CanFreeze", "Freezable", "DP", "DpOwnerType" });
                }
                return _UnableToFreezeFreezableSubProperty;
            }
        }
    }
}

