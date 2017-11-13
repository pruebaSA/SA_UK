namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack=2)]
    internal struct CalendarTableData
    {
        internal const int sizeofDataFields = 0x48;
        internal ushort iCalendar;
        internal ushort iTwoDigitYearMax;
        internal uint saShortDate;
        internal uint saYearMonth;
        internal uint saLongDate;
        internal uint saEraNames;
        internal uint waaEraRanges;
        internal uint saDayNames;
        internal uint saAbbrevDayNames;
        internal uint saMonthNames;
        internal uint saAbbrevMonthNames;
        internal ushort iCurrentEra;
        internal ushort iFormatFlags;
        internal uint sName;
        internal uint sMonthDay;
        internal uint saAbbrevEraNames;
        internal uint saAbbrevEnglishEraNames;
        internal uint saLeapYearMonthNames;
        internal uint saSuperShortDayNames;
        internal ushort _padding1;
        internal ushort _padding2;
    }
}

