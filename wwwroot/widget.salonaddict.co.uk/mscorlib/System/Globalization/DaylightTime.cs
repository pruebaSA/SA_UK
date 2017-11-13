namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public class DaylightTime
    {
        internal TimeSpan m_delta;
        internal DateTime m_end;
        internal DateTime m_start;

        private DaylightTime()
        {
        }

        public DaylightTime(DateTime start, DateTime end, TimeSpan delta)
        {
            this.m_start = start;
            this.m_end = end;
            this.m_delta = delta;
        }

        public TimeSpan Delta =>
            this.m_delta;

        public DateTime End =>
            this.m_end;

        public DateTime Start =>
            this.m_start;
    }
}

