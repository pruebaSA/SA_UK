namespace System.Windows.Markup
{
    using System;
    using System.Globalization;
    using System.Text;

    public class DateTimeValueSerializer : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context) => 
            true;

        public override bool CanConvertToString(object value, IValueSerializerContext context) => 
            (value is DateTime);

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            if (value == null)
            {
                throw base.GetConvertFromException(value);
            }
            if (value.Length == 0)
            {
                return DateTime.MinValue;
            }
            CultureInfo englishUSCulture = TypeConverterHelper.EnglishUSCulture;
            DateTimeFormatInfo format = (DateTimeFormatInfo) englishUSCulture.GetFormat(typeof(DateTimeFormatInfo));
            DateTimeStyles styles = DateTimeStyles.RoundtripKind | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowLeadingWhite;
            if (format != null)
            {
                return DateTime.Parse(value, format, styles);
            }
            return DateTime.Parse(value, englishUSCulture, styles);
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            if ((value == null) || !(value is DateTime))
            {
                throw base.GetConvertToException(value, typeof(string));
            }
            DateTime time = (DateTime) value;
            StringBuilder builder = new StringBuilder("yyyy-MM-dd");
            if (time.TimeOfDay.TotalSeconds == 0.0)
            {
                if (time.Kind != DateTimeKind.Unspecified)
                {
                    builder.Append("'T'HH':'mm");
                }
            }
            else
            {
                builder.Append("'T'HH':'mm");
                if ((time.TimeOfDay.Seconds != 0) || (time.TimeOfDay.Milliseconds != 0))
                {
                    builder.Append("':'ss");
                    if (time.TimeOfDay.Milliseconds != 0)
                    {
                        builder.Append("'.'FFFFFFF");
                    }
                }
            }
            builder.Append("K");
            return time.ToString(builder.ToString(), TypeConverterHelper.EnglishUSCulture);
        }
    }
}

