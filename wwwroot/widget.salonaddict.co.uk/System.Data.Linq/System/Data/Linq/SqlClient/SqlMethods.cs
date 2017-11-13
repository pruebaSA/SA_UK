namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Linq;
    using System.Reflection;

    public static class SqlMethods
    {
        public static int DateDiffDay(DateTime startDate, DateTime endDate)
        {
            TimeSpan span = (TimeSpan) (endDate.Date - startDate.Date);
            return span.Days;
        }

        public static int DateDiffDay(DateTimeOffset startDate, DateTimeOffset endDate) => 
            DateDiffDay(startDate.UtcDateTime, endDate.UtcDateTime);

        public static int? DateDiffDay(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffDay(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int? DateDiffDay(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffDay(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int DateDiffHour(DateTime startDate, DateTime endDate) => 
            (((DateDiffDay(startDate, endDate) * 0x18) + endDate.Hour) - startDate.Hour);

        public static int DateDiffHour(DateTimeOffset startDate, DateTimeOffset endDate) => 
            DateDiffHour(startDate.UtcDateTime, endDate.UtcDateTime);

        public static int? DateDiffHour(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffHour(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int? DateDiffHour(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffHour(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int DateDiffMicrosecond(DateTime startDate, DateTime endDate) => 
            ((int) ((endDate.Ticks - startDate.Ticks) / 10L));

        public static int DateDiffMicrosecond(DateTimeOffset startDate, DateTimeOffset endDate) => 
            DateDiffMicrosecond(startDate.UtcDateTime, endDate.UtcDateTime);

        public static int? DateDiffMicrosecond(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffMicrosecond(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int? DateDiffMicrosecond(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffMicrosecond(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int DateDiffMillisecond(DateTime startDate, DateTime endDate) => 
            (((DateDiffSecond(startDate, endDate) * 0x3e8) + endDate.Millisecond) - startDate.Millisecond);

        public static int DateDiffMillisecond(DateTimeOffset startDate, DateTimeOffset endDate) => 
            DateDiffMillisecond(startDate.UtcDateTime, endDate.UtcDateTime);

        public static int? DateDiffMillisecond(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffMillisecond(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int? DateDiffMillisecond(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffMillisecond(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int DateDiffMinute(DateTime startDate, DateTime endDate) => 
            (((DateDiffHour(startDate, endDate) * 60) + endDate.Minute) - startDate.Minute);

        public static int DateDiffMinute(DateTimeOffset startDate, DateTimeOffset endDate) => 
            DateDiffMinute(startDate.UtcDateTime, endDate.UtcDateTime);

        public static int? DateDiffMinute(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffMinute(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int? DateDiffMinute(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffMinute(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int DateDiffMonth(DateTime startDate, DateTime endDate) => 
            (((12 * (endDate.Year - startDate.Year)) + endDate.Month) - startDate.Month);

        public static int DateDiffMonth(DateTimeOffset startDate, DateTimeOffset endDate) => 
            DateDiffMonth(startDate.UtcDateTime, endDate.UtcDateTime);

        public static int? DateDiffMonth(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffMonth(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int? DateDiffMonth(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffMonth(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int DateDiffNanosecond(DateTime startDate, DateTime endDate) => 
            ((int) ((endDate.Ticks - startDate.Ticks) * 100L));

        public static int DateDiffNanosecond(DateTimeOffset startDate, DateTimeOffset endDate) => 
            DateDiffNanosecond(startDate.UtcDateTime, endDate.UtcDateTime);

        public static int? DateDiffNanosecond(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffNanosecond(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int? DateDiffNanosecond(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffNanosecond(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int DateDiffSecond(DateTime startDate, DateTime endDate) => 
            (((DateDiffMinute(startDate, endDate) * 60) + endDate.Second) - startDate.Second);

        public static int DateDiffSecond(DateTimeOffset startDate, DateTimeOffset endDate) => 
            DateDiffSecond(startDate.UtcDateTime, endDate.UtcDateTime);

        public static int? DateDiffSecond(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffSecond(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int? DateDiffSecond(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffSecond(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int DateDiffYear(DateTime startDate, DateTime endDate) => 
            (endDate.Year - startDate.Year);

        public static int DateDiffYear(DateTimeOffset startDate, DateTimeOffset endDate) => 
            DateDiffYear(startDate.UtcDateTime, endDate.UtcDateTime);

        public static int? DateDiffYear(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffYear(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static int? DateDiffYear(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return new int?(DateDiffYear(startDate.Value, endDate.Value));
            }
            return null;
        }

        public static bool Like(string matchExpression, string pattern)
        {
            throw System.Data.Linq.SqlClient.Error.SqlMethodOnlyForSql(MethodBase.GetCurrentMethod());
        }

        public static bool Like(string matchExpression, string pattern, char escapeCharacter)
        {
            throw System.Data.Linq.SqlClient.Error.SqlMethodOnlyForSql(MethodBase.GetCurrentMethod());
        }

        internal static int RawLength(byte[] value)
        {
            throw System.Data.Linq.SqlClient.Error.SqlMethodOnlyForSql(MethodBase.GetCurrentMethod());
        }

        internal static int RawLength(Binary value)
        {
            throw System.Data.Linq.SqlClient.Error.SqlMethodOnlyForSql(MethodBase.GetCurrentMethod());
        }

        internal static int RawLength(string value)
        {
            throw System.Data.Linq.SqlClient.Error.SqlMethodOnlyForSql(MethodBase.GetCurrentMethod());
        }
    }
}

