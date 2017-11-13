namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal sealed class Literal : Expr
    {
        private object _computedValue;
        private static readonly char[] _dateSeparators = new char[] { '-' };
        private static readonly char[] _datetimeOffsetSeparators = new char[] { ' ', ':', '-', '.', '+', '-' };
        private static readonly char[] _datetimeSeparators = new char[] { ' ', ':', '-', '.' };
        private static readonly byte[] _emptyByteArray = new byte[0];
        private bool _isNull;
        private LiteralKind _literalKind;
        private string _originalValue;
        private static readonly char[] _timeSeparators = new char[] { ':', '.' };
        private System.Type _type;
        private bool _wasValueComputed;
        private static char[] floatTokens = new char[] { '.', 'E', 'e' };
        private static char[] numberSuffixes = new char[] { 'U', 'u', 'L', 'l', 'F', 'f', 'M', 'm', 'D', 'd' };

        private Literal(bool boolLiteral) : base(null, 0)
        {
            this._wasValueComputed = true;
            this._originalValue = string.Empty;
            this._computedValue = boolLiteral;
            this._type = typeof(bool);
        }

        internal Literal(string originalValue, LiteralKind kind, string query, int inputPos) : base(query, inputPos)
        {
            this._originalValue = originalValue;
            this._literalKind = kind;
        }

        private void ComputeValue()
        {
            if (!this._wasValueComputed)
            {
                this._wasValueComputed = true;
                switch (this._literalKind)
                {
                    case LiteralKind.Number:
                        this._computedValue = ConvertNumericLiteral(base.ErrCtx, this._originalValue);
                        break;

                    case LiteralKind.NonUnicodeString:
                        this._computedValue = GetStringLiteralValue(this._originalValue, false);
                        break;

                    case LiteralKind.UnicodeString:
                        this._computedValue = GetStringLiteralValue(this._originalValue, true);
                        break;

                    case LiteralKind.Boolean:
                        this._computedValue = ConvertBooleanLiteralValue(base.ErrCtx, this._originalValue);
                        break;

                    case LiteralKind.Binary:
                        this._computedValue = ConvertBinaryLiteralValue(base.ErrCtx, this._originalValue);
                        break;

                    case LiteralKind.DateTime:
                        this._computedValue = ConvertDateTimeLiteralValue(base.ErrCtx, this._originalValue);
                        break;

                    case LiteralKind.Time:
                        this._computedValue = ConvertTimeLiteralValue(base.ErrCtx, this._originalValue);
                        break;

                    case LiteralKind.DateTimeOffset:
                        this._computedValue = ConvertDateTimeOffsetLiteralValue(base.ErrCtx, this._originalValue);
                        break;

                    case LiteralKind.Guid:
                        this._computedValue = ConvertGuidLiteralValue(base.ErrCtx, this._originalValue);
                        break;

                    case LiteralKind.Null:
                        this._computedValue = null;
                        this._isNull = true;
                        break;

                    default:
                        throw EntityUtil.NotSupported(Strings.LiteralTypeNotSupported(this._literalKind.ToString()));
                }
                this._type = this._isNull ? null : this._computedValue.GetType();
            }
        }

        private static byte[] ConvertBinaryLiteralValue(ErrorContext errCtx, string binaryLiteralValue)
        {
            if (string.IsNullOrEmpty(binaryLiteralValue))
            {
                return _emptyByteArray;
            }
            int num = 0;
            int num2 = binaryLiteralValue.Length - 1;
            int num3 = (num2 - num) + 1;
            int num4 = num3 / 2;
            bool flag = 0 != (num3 % 2);
            if (flag)
            {
                num4++;
            }
            byte[] buffer = new byte[num4];
            int num5 = 0;
            if (flag)
            {
                buffer[num5++] = (byte) HexDigitToBinaryValue(binaryLiteralValue[num++]);
            }
            while (num < num2)
            {
                buffer[num5++] = (byte) ((HexDigitToBinaryValue(binaryLiteralValue[num++]) << 4) | HexDigitToBinaryValue(binaryLiteralValue[num++]));
            }
            return buffer;
        }

        private static bool ConvertBooleanLiteralValue(ErrorContext errCtx, string booleanLiteralValue)
        {
            bool result = false;
            if (!bool.TryParse(booleanLiteralValue, out result))
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.InvalidLiteralFormat("Boolean", booleanLiteralValue));
            }
            return result;
        }

        private static DateTime ConvertDateTimeLiteralValue(ErrorContext errCtx, string datetimeLiteralValue)
        {
            int num;
            int num2;
            int num3;
            int num4;
            int num5;
            int num6;
            int num7;
            string[] datetimeParts = datetimeLiteralValue.Split(_datetimeSeparators, StringSplitOptions.RemoveEmptyEntries);
            GetDateParts(datetimeLiteralValue, datetimeParts, out num, out num2, out num3);
            GetTimeParts(datetimeLiteralValue, datetimeParts, 3, out num4, out num5, out num6, out num7);
            DateTime time = new DateTime(num, num2, num3, num4, num5, num6, 0);
            return time.AddTicks((long) num7);
        }

        private static DateTimeOffset ConvertDateTimeOffsetLiteralValue(ErrorContext errCtx, string datetimeLiteralValue)
        {
            int num;
            int num2;
            int num3;
            int num4;
            int num5;
            int num6;
            int num7;
            DateTimeOffset offset;
            string[] datetimeParts = datetimeLiteralValue.Split(_datetimeOffsetSeparators, StringSplitOptions.RemoveEmptyEntries);
            GetDateParts(datetimeLiteralValue, datetimeParts, out num, out num2, out num3);
            string[] destinationArray = new string[datetimeParts.Length - 2];
            Array.Copy(datetimeParts, destinationArray, (int) (datetimeParts.Length - 2));
            GetTimeParts(datetimeLiteralValue, destinationArray, 3, out num4, out num5, out num6, out num7);
            int hours = int.Parse(datetimeParts[datetimeParts.Length - 2], NumberStyles.Integer, CultureInfo.InvariantCulture);
            int minutes = int.Parse(datetimeParts[datetimeParts.Length - 1], NumberStyles.Integer, CultureInfo.InvariantCulture);
            TimeSpan span = new TimeSpan(hours, minutes, 0);
            if (datetimeLiteralValue.IndexOf('+') == -1)
            {
                span = span.Negate();
            }
            DateTime dateTime = new DateTime(num, num2, num3, num4, num5, num6, 0);
            dateTime = dateTime.AddTicks((long) num7);
            try
            {
                offset = new DateTimeOffset(dateTime, span);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.InvalidDateTimeOffsetLiteral(datetimeLiteralValue), exception);
            }
            return offset;
        }

        private static Guid ConvertGuidLiteralValue(ErrorContext errCtx, string guidLiteralValue) => 
            new Guid(guidLiteralValue);

        private static object ConvertNumericLiteral(ErrorContext errCtx, string numericString)
        {
            int startIndex = numericString.IndexOfAny(numberSuffixes);
            if (-1 != startIndex)
            {
                string str = numericString.Substring(startIndex).ToUpperInvariant();
                string s = numericString.Substring(0, numericString.Length - str.Length);
                switch (str)
                {
                    case "U":
                        uint num2;
                        if (!uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
                        {
                            throw EntityUtil.EntitySqlError(errCtx, Strings.CannotConvertNumericLiteral(numericString, "unsigned int"));
                        }
                        return num2;

                    case "L":
                        long num3;
                        if (!long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out num3))
                        {
                            throw EntityUtil.EntitySqlError(errCtx, Strings.CannotConvertNumericLiteral(numericString, "long"));
                        }
                        return num3;

                    case "UL":
                    case "LU":
                        ulong num4;
                        if (!ulong.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out num4))
                        {
                            throw EntityUtil.EntitySqlError(errCtx, Strings.CannotConvertNumericLiteral(numericString, "unsigned long"));
                        }
                        return num4;

                    case "F":
                        float num5;
                        if (!float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out num5))
                        {
                            throw EntityUtil.EntitySqlError(errCtx, Strings.CannotConvertNumericLiteral(numericString, "float"));
                        }
                        return num5;

                    case "M":
                        decimal num6;
                        if (!decimal.TryParse(s, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out num6))
                        {
                            throw EntityUtil.EntitySqlError(errCtx, Strings.CannotConvertNumericLiteral(numericString, "decimal"));
                        }
                        return num6;

                    case "D":
                        double num7;
                        if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out num7))
                        {
                            throw EntityUtil.EntitySqlError(errCtx, Strings.CannotConvertNumericLiteral(numericString, "double"));
                        }
                        return num7;
                }
            }
            return DefaultNumericConversion(numericString, errCtx);
        }

        private static TimeSpan ConvertTimeLiteralValue(ErrorContext errCtx, string datetimeLiteralValue)
        {
            int num;
            int num2;
            int num3;
            int num4;
            string[] datetimeParts = datetimeLiteralValue.Split(_datetimeSeparators, StringSplitOptions.RemoveEmptyEntries);
            GetTimeParts(datetimeLiteralValue, datetimeParts, 0, out num, out num2, out num3, out num4);
            TimeSpan span = new TimeSpan(num, num2, num3);
            return span.Add(new TimeSpan((long) num4));
        }

        private static object DefaultNumericConversion(string numericString, ErrorContext errCtx)
        {
            int num2;
            long num3;
            if (-1 != numericString.IndexOfAny(floatTokens))
            {
                double num;
                if (!double.TryParse(numericString, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
                {
                    throw EntityUtil.EntitySqlError(errCtx, Strings.CannotConvertNumericLiteral(numericString, "double"));
                }
                return num;
            }
            if (int.TryParse(numericString, NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
            {
                return num2;
            }
            if (!long.TryParse(numericString, NumberStyles.Integer, CultureInfo.InvariantCulture, out num3))
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.CannotConvertNumericLiteral(numericString, "long"));
            }
            return num3;
        }

        private static void GetDateParts(string datetimeLiteralValue, string[] datetimeParts, out int year, out int month, out int day)
        {
            year = int.Parse(datetimeParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
            if ((year < 1) || (year > 0x270f))
            {
                throw EntityUtil.EntitySqlError(Strings.InvalidYear(datetimeParts[0], datetimeLiteralValue));
            }
            month = int.Parse(datetimeParts[1], NumberStyles.Integer, CultureInfo.InvariantCulture);
            if ((month < 1) || (month > 12))
            {
                throw EntityUtil.EntitySqlError(Strings.InvalidMonth(datetimeParts[1], datetimeLiteralValue));
            }
            day = int.Parse(datetimeParts[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
            if (day < 1)
            {
                throw EntityUtil.EntitySqlError(Strings.InvalidDay(datetimeParts[2], datetimeLiteralValue));
            }
            if (day > DateTime.DaysInMonth(year, month))
            {
                throw EntityUtil.EntitySqlError(Strings.InvalidDayInMonth(datetimeParts[2], datetimeParts[1], datetimeLiteralValue));
            }
        }

        private static string GetStringLiteralValue(string stringLiteralValue, bool isUnicode)
        {
            int startIndex = isUnicode ? 2 : 1;
            char c = stringLiteralValue[startIndex - 1];
            if ((c != '\'') && (c != '"'))
            {
                throw EntityUtil.EntitySqlError(Strings.MalformedStringLiteralPayload);
            }
            string str = "";
            int num2 = stringLiteralValue.Split(new char[] { c }).Length - 1;
            if ((num2 % 2) != 0)
            {
                throw EntityUtil.EntitySqlError(Strings.MalformedStringLiteralPayload);
            }
            str = stringLiteralValue.Substring(startIndex, stringLiteralValue.Length - (1 + startIndex)).Replace(new string(c, 2), new string(c, 1));
            int num3 = str.Split(new char[] { c }).Length - 1;
            if (num3 != ((num2 - 2) / 2))
            {
                throw EntityUtil.EntitySqlError(Strings.MalformedStringLiteralPayload);
            }
            return str;
        }

        private static void GetTimeParts(string datetimeLiteralValue, string[] datetimeParts, int timePartStartIndex, out int hour, out int minute, out int second, out int ticks)
        {
            hour = int.Parse(datetimeParts[timePartStartIndex], NumberStyles.Integer, CultureInfo.InvariantCulture);
            if (hour > 0x17)
            {
                throw EntityUtil.EntitySqlError(Strings.InvalidHour(datetimeParts[timePartStartIndex], datetimeLiteralValue));
            }
            minute = int.Parse(datetimeParts[++timePartStartIndex], NumberStyles.Integer, CultureInfo.InvariantCulture);
            if (minute > 0x3b)
            {
                throw EntityUtil.EntitySqlError(Strings.InvalidMinute(datetimeParts[timePartStartIndex], datetimeLiteralValue));
            }
            second = 0;
            ticks = 0;
            timePartStartIndex++;
            if (datetimeParts.Length > timePartStartIndex)
            {
                second = int.Parse(datetimeParts[timePartStartIndex], NumberStyles.Integer, CultureInfo.InvariantCulture);
                if (second > 0x3b)
                {
                    throw EntityUtil.EntitySqlError(Strings.InvalidSecond(datetimeParts[timePartStartIndex], datetimeLiteralValue));
                }
                timePartStartIndex++;
                if (datetimeParts.Length > timePartStartIndex)
                {
                    string s = datetimeParts[timePartStartIndex].PadRight(7, '0');
                    ticks = int.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
            }
        }

        private static int HexDigitToBinaryValue(char hexChar)
        {
            if ((hexChar >= '0') && (hexChar <= '9'))
            {
                return (hexChar - '0');
            }
            if ((hexChar >= 'A') && (hexChar <= 'F'))
            {
                return ((hexChar - 'A') + 10);
            }
            if ((hexChar < 'a') || (hexChar > 'f'))
            {
                throw EntityUtil.ArgumentOutOfRange("hexadecimal digit is not valid");
            }
            return ((hexChar - 'a') + 10);
        }

        internal static Literal NewBooleanLiteral(bool value) => 
            new Literal(value);

        internal void PrefixSign(string sign)
        {
            this._originalValue = sign + this._originalValue;
        }

        internal bool IsNullLiteral
        {
            get
            {
                this.ComputeValue();
                return this._isNull;
            }
        }

        internal bool IsNumberKind =>
            (this._literalKind == LiteralKind.Number);

        internal bool IsSigned
        {
            get
            {
                if (this._originalValue[0] != '-')
                {
                    return (this._originalValue[0] == '+');
                }
                return true;
            }
        }

        internal bool IsString
        {
            get
            {
                this.ComputeValue();
                return (this._computedValue is string);
            }
        }

        internal bool IsUnicodeString =>
            (this.IsString && (this._literalKind == LiteralKind.UnicodeString));

        internal string OriginalValue =>
            this._originalValue;

        internal System.Type Type
        {
            get
            {
                this.ComputeValue();
                return this._type;
            }
        }

        internal object Value
        {
            get
            {
                this.ComputeValue();
                return this._computedValue;
            }
        }
    }
}

