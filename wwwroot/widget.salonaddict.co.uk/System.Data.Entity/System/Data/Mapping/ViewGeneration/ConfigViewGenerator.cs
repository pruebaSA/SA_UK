namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Data.Common.Utils;
    using System.Diagnostics;
    using System.Text;

    internal class ConfigViewGenerator : InternalBase
    {
        private TimeSpan[] m_breakdownTimes;
        private bool m_enableValidation = true;
        private bool m_generateViewsForEachType;
        private PerfType m_singlePerfOp;
        private Stopwatch m_singleWatch = new Stopwatch();
        private ViewGenTraceLevel m_traceLevel;
        private Stopwatch m_watch = new Stopwatch();

        internal ConfigViewGenerator()
        {
            int length = Enum.GetNames(typeof(PerfType)).Length;
            this.m_breakdownTimes = new TimeSpan[length];
            this.TraceLevel = ViewGenTraceLevel.None;
            this.GenerateViewsForEachType = false;
            this.StartWatch();
        }

        internal bool IsTraceAllowed(ViewGenTraceLevel traceLevel) => 
            (this.TraceLevel >= traceLevel);

        internal void SetTimeForFinishedActivity(PerfType perfType)
        {
            TimeSpan elapsed = this.m_watch.Elapsed;
            int index = (int) perfType;
            this.BreakdownTimes[index] = this.BreakdownTimes[index].Add(elapsed);
            this.m_watch.Reset();
            this.m_watch.Start();
        }

        internal void StartSingleWatch(PerfType perfType)
        {
            this.m_singleWatch.Start();
            this.m_singlePerfOp = perfType;
        }

        private void StartWatch()
        {
            this.m_watch.Start();
        }

        internal void StopSingleWatch(PerfType perfType)
        {
            TimeSpan elapsed = this.m_singleWatch.Elapsed;
            int index = (int) perfType;
            this.m_singleWatch.Stop();
            this.m_singleWatch.Reset();
            this.BreakdownTimes[index] = this.BreakdownTimes[index].Add(elapsed);
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            StringUtil.FormatStringBuilder(builder, "Trace Switch: {0}", new object[] { this.m_traceLevel });
        }

        internal TimeSpan[] BreakdownTimes =>
            this.m_breakdownTimes;

        internal bool GenerateViewsForEachType
        {
            get => 
                this.m_generateViewsForEachType;
            set
            {
                this.m_generateViewsForEachType = value;
            }
        }

        internal bool IsNormalTracing =>
            this.IsTraceAllowed(ViewGenTraceLevel.Normal);

        internal bool IsValidationEnabled
        {
            get => 
                this.m_enableValidation;
            set
            {
                this.m_enableValidation = value;
            }
        }

        internal bool IsVerboseTracing =>
            this.IsTraceAllowed(ViewGenTraceLevel.Verbose);

        internal bool IsViewTracing =>
            this.IsTraceAllowed(ViewGenTraceLevel.ViewsOnly);

        internal ViewGenTraceLevel TraceLevel
        {
            get => 
                this.m_traceLevel;
            set
            {
                this.m_traceLevel = value;
            }
        }
    }
}

