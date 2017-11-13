namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack=2)]
    internal struct DTFIUserOverrideValues
    {
        internal string shortDatePattern;
        internal string longDatePattern;
        internal string yearMonthPattern;
        internal string amDesignator;
        internal string pmDesignator;
        internal string longTimePattern;
        internal int firstDayOfWeek;
        internal int padding1;
        internal int calendarWeekRule;
        internal int padding2;
    }
}

