namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public class TaiwanCalendar : Calendar
    {
        internal static readonly DateTime calendarMinValue = new DateTime(0x778, 1, 1);
        private const int DEFAULT_TWO_DIGIT_YEAR_MAX = 0x63;
        internal GregorianCalendarHelper helper;
        internal static Calendar m_defaultInstance;
        internal static EraInfo[] m_EraInfo = GregorianCalendarHelper.InitEraInfo(4);

        public TaiwanCalendar()
        {
            this.helper = new GregorianCalendarHelper(this, m_EraInfo);
        }

        public override DateTime AddMonths(DateTime time, int months) => 
            this.helper.AddMonths(time, months);

        public override DateTime AddYears(DateTime time, int years) => 
            this.helper.AddYears(time, years);

        public override int GetDayOfMonth(DateTime time) => 
            this.helper.GetDayOfMonth(time);

        public override DayOfWeek GetDayOfWeek(DateTime time) => 
            this.helper.GetDayOfWeek(time);

        public override int GetDayOfYear(DateTime time) => 
            this.helper.GetDayOfYear(time);

        public override int GetDaysInMonth(int year, int month, int era) => 
            this.helper.GetDaysInMonth(year, month, era);

        public override int GetDaysInYear(int year, int era) => 
            this.helper.GetDaysInYear(year, era);

        internal static Calendar GetDefaultInstance()
        {
            if (m_defaultInstance == null)
            {
                m_defaultInstance = new TaiwanCalendar();
            }
            return m_defaultInstance;
        }

        public override int GetEra(DateTime time) => 
            this.helper.GetEra(time);

        [ComVisible(false)]
        public override int GetLeapMonth(int year, int era) => 
            this.helper.GetLeapMonth(year, era);

        public override int GetMonth(DateTime time) => 
            this.helper.GetMonth(time);

        public override int GetMonthsInYear(int year, int era) => 
            this.helper.GetMonthsInYear(year, era);

        [ComVisible(false)]
        public override int GetWeekOfYear(DateTime time, CalendarWeekRule rule, DayOfWeek firstDayOfWeek) => 
            this.helper.GetWeekOfYear(time, rule, firstDayOfWeek);

        public override int GetYear(DateTime time) => 
            this.helper.GetYear(time);

        public override bool IsLeapDay(int year, int month, int day, int era) => 
            this.helper.IsLeapDay(year, month, day, era);

        public override bool IsLeapMonth(int year, int month, int era) => 
            this.helper.IsLeapMonth(year, month, era);

        public override bool IsLeapYear(int year, int era) => 
            this.helper.IsLeapYear(year, era);

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) => 
            this.helper.ToDateTime(year, month, day, hour, minute, second, millisecond, era);

        public override int ToFourDigitYear(int year)
        {
            if (year <= 0)
            {
                throw new ArgumentOutOfRangeException("year", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
            }
            if (year > this.helper.MaxYear)
            {
                throw new ArgumentOutOfRangeException("year", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 1, this.helper.MaxYear }));
            }
            return year;
        }

        [ComVisible(false)]
        public override CalendarAlgorithmType AlgorithmType =>
            CalendarAlgorithmType.SolarCalendar;

        public override int[] Eras =>
            this.helper.Eras;

        internal override int ID =>
            4;

        [ComVisible(false)]
        public override DateTime MaxSupportedDateTime =>
            DateTime.MaxValue;

        [ComVisible(false)]
        public override DateTime MinSupportedDateTime =>
            calendarMinValue;

        public override int TwoDigitYearMax
        {
            get
            {
                if (base.twoDigitYearMax == -1)
                {
                    base.twoDigitYearMax = Calendar.GetSystemTwoDigitYearSetting(this.ID, 0x63);
                }
                return base.twoDigitYearMax;
            }
            set
            {
                base.VerifyWritable();
                if ((value < 0x63) || (value > this.helper.MaxYear))
                {
                    throw new ArgumentOutOfRangeException("year", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 0x63, this.helper.MaxYear }));
                }
                base.twoDigitYearMax = value;
            }
        }
    }
}

